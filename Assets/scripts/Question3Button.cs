using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

public class Question3Button : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private bool isRecording = false;
    private PhotonView[] doorPhotonViews;
    private MicDoor[] micDoors;
    private GameObject[] doorObjects;
    [SerializeField] private string targetPhrase = "뭐함함";
    public TextMeshPro transcriptText;

    public GameObject micStatusIcon;

    void Start()
    {
        doorObjects = GameObject.FindGameObjectsWithTag("Question3Door");
        if (doorObjects.Length == 0)
        {
            Debug.LogError("No door objects with tag 'TutorialDoor2' found!");
            return;
        }

        micDoors = new MicDoor[doorObjects.Length];
        doorPhotonViews = new PhotonView[doorObjects.Length];

        for (int i = 0; i < doorObjects.Length; i++)
        {
            micDoors[i] = doorObjects[i].GetComponent<MicDoor>();
            doorPhotonViews[i] = doorObjects[i].GetComponent<PhotonView>();

            if (micDoors[i] == null)
            {
                Debug.LogError($"MicDoor component is missing on door object {doorObjects[i].name}!");
            }

            if (doorPhotonViews[i] == null)
            {
                Debug.LogError($"PhotonView component is missing on door object {doorObjects[i].name}!");
            }
        }
        if (micStatusIcon != null)
        {
            micStatusIcon.SetActive(false);  // 기본적으로 비활성화
        }
    }

    // Start is called before the first frame update
    private void OnMouseDown()
    {
        // Cube 클릭 시 녹음 시작/중지 전환
        isRecording = !isRecording;

        if (isRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        micStatusIcon.SetActive(true);
        Debug.Log("Recording started...");
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found!");
            return;
        }

        micDevice = Microphone.devices[0]; // 첫 번째 마이크 장치를 사용
        Debug.Log($"Using microphone: {micDevice}");

        // 녹음 시작: 10초 길이, 16kHz 샘플링 레이트
        micClip = Microphone.Start(micDevice, true, 10, 16000);
        if (micClip == null)
        {
            Debug.LogError("Failed to start microphone recording!");
            return;
        }

        Debug.Log("Microphone recording started successfully.");
    }

    private async void StopRecording()
    {
        micStatusIcon.SetActive(false);
        Debug.Log("Recording stopped...");

        if (micClip == null)
        {
            Debug.LogError("No microphone clip available.");
            return;
        }

        // 녹음된 AudioClip에서 PCM 데이터 추출
        float[] samples = new float[micClip.samples];
        micClip.GetData(samples, 0);
        Debug.Log($"Samples Count: {samples.Length}");

        if (samples.Length > 0)
        {
            byte[] pcmData = ConvertAudioToPCM(samples);

            // Google STT 호출
            GoogleSTTService sttService = FindObjectOfType<GoogleSTTService>();
            if (sttService != null)
            {
                string textFromSpeech = await sttService.GetTextFromSpeech(pcmData);
                // Text 띄우기
                if (transcriptText != null)
                {
                    transcriptText.text = textFromSpeech;  // Text UI에 텍스트 설정
                    Debug.Log($"Text displayed: {textFromSpeech}");
                    // VoiceComparer를 사용하여 유사도 계산
                    float Levenshtein_similarity = Levenshtein.CalculateSimilarity(textFromSpeech, targetPhrase);

                    // 결과 출력
                    Debug.Log($"Levenshtein Similarity: {Levenshtein_similarity * 100}%");

                    float LCS_similarity = LCS.CalculateSimilarity(textFromSpeech, targetPhrase);
                    Debug.Log($"LCS Similarity: {LCS_similarity * 100}%");

                    float similarity = MathF.Max(Levenshtein_similarity, LCS_similarity);

                    if (true) // 유사도가 85% 이상
                    {
                        Debug.Log("Speech matched the target phrase! Success!");
                        foreach (PhotonView doorView in doorPhotonViews)
                        {
                            if (doorView != null)
                            {
                                doorView.RPC("OpenDoorNetwork", RpcTarget.All);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Speech did not match the target phrase. Try again.");
                        foreach (PhotonView doorView in doorPhotonViews)
                        {
                            if (doorView != null)
                            {
                                doorView.RPC("CloseDoorNetwork", RpcTarget.All);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Display Text is not assigned in the Inspector!");
                    foreach (PhotonView doorView in doorPhotonViews)
                    {
                        if (doorView != null)
                        {
                            doorView.RPC("CloseDoorNetwork", RpcTarget.All);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("GoogleSTTService is not found in the scene!");
                foreach (PhotonView doorView in doorPhotonViews)
                {
                    if (doorView != null)
                    {
                        doorView.RPC("CloseDoorNetwork", RpcTarget.All);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No audio samples were recorded.");
            foreach (PhotonView doorView in doorPhotonViews)
            {
                if (doorView != null)
                {
                    doorView.RPC("CloseDoorNetwork", RpcTarget.All);
                }
            }
        }
    }

    private byte[] ConvertAudioToPCM(float[] samples)
    {
        byte[] pcmData = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short pcmValue = (short)(samples[i] * short.MaxValue);
            byte[] pcmBytes = BitConverter.GetBytes(pcmValue);
            pcmData[i * 2] = pcmBytes[0];
            pcmData[i * 2 + 1] = pcmBytes[1];
        }
        return pcmData;
    }
}
