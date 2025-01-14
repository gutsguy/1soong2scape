using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Question1Button : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private bool isRecording = false;
    private MicDoor[] micDoors;
    [SerializeField] private string targetPhrase = "보라";
    public TextMeshPro transcriptText;

    public GameObject micStatusIcon;

    void Start(){
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Question1Door");

        if (doorObjects.Length == 0)
        {
            Debug.LogError("No doors with tag 'Question1Door' found!");
        }

        // MicDoor 컴포넌트를 배열에 저장
        micDoors = new MicDoor[doorObjects.Length];
        for (int i = 0; i < doorObjects.Length; i++)
        {
            micDoors[i] = doorObjects[i].GetComponent<MicDoor>();
            if (micDoors[i] == null)
            {
                Debug.LogError($"GameObject {doorObjects[i].name} does not have a MicDoor component!");
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
                        foreach (MicDoor door in micDoors)
                        {
                            door?.OpenDoor(); // 모든 문 열기
                        }
                    }
                    else
                    {
                        Debug.Log("Speech did not match the target phrase. Try again.");
                        foreach (MicDoor door in micDoors)
                        {
                            door?.CloseDoor(); // 모든 문 닫기
                        }
                    }
                }
                else
                {
                    Debug.LogError("Display Text is not assigned in the Inspector!");
                        foreach (MicDoor door in micDoors)
                        {
                            door?.CloseDoor(); // 모든 문 닫기
                        }
                }
            }
            else
            {
                Debug.LogError("GoogleSTTService is not found in the scene!");
                foreach (MicDoor door in micDoors)
                {
                    door?.CloseDoor(); // 모든 문 닫기
                }
            }
        }
        else
        {
            Debug.LogWarning("No audio samples were recorded.");
            foreach (MicDoor door in micDoors)
            {
                door?.CloseDoor(); // 모든 문 닫기
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
