using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

public class TutorialButton1 : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private bool isRecording = false;
    private PhotonView doorPhotonView;
    private MicDoor micDoor;
    private GameObject doorObject;
    [SerializeField] private string targetPhrase = "안녕하세요";
    public TextMeshPro transcriptText;

    public GameObject micStatusIcon;

    void Start(){
        doorObject = GameObject.FindWithTag("TutorialDoor1");
        if (doorObject != null)
        {
            micDoor = doorObject.GetComponent<MicDoor>();
            doorPhotonView = doorObject.GetComponent<PhotonView>();  // Door 오브젝트의 PhotonView 가져오기

            if (micDoor == null)
            {
                Debug.LogError("MicDoor component not found in doorObject!");
            }
            if (doorPhotonView == null)
            {
                Debug.LogError("PhotonView component not found in doorObject!");
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

    private void StartRecording(){
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

    private async void StopRecording(){
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

                    if (similarity >= 0.85f) // 유사도가 85% 이상
                    {
                        Debug.Log("Speech matched the target phrase! Success!");
                        // Door 오브젝트에 있는 PhotonView를 통해 네트워크 동기화
                        doorPhotonView.RPC("OpenDoorNetwork", RpcTarget.All);
                    }
                    else
                    {
                        Debug.Log("Speech did not match the target phrase. Try again.");
                        doorPhotonView.RPC("CloseDoorNetwork", RpcTarget.All);
                    }
                }
                else
                {
                    Debug.LogError("Display Text is not assigned in the Inspector!");
                    doorPhotonView.RPC("CloseDoorNetwork", RpcTarget.All);
                }
            }
            else
            {
                Debug.LogError("GoogleSTTService is not found in the scene!");
                doorPhotonView.RPC("CloseDoorNetwork", RpcTarget.All);
            }
        }
        else
        {
            Debug.LogWarning("No audio samples were recorded.");
            doorPhotonView.RPC("CloseDoorNetwork", RpcTarget.All);
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
