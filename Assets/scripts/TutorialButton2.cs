using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialButton2 : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private bool isRecording = false;
    private MicDoor micDoor;
    [SerializeField] private string targetPhrase = "안녕하세요";
    public TextMesh transcriptText;
    

    void Start(){
        micDoor = GameObject.FindWithTag("TutorialDoor2")?.GetComponent<MicDoor>();
        if (micDoor == null)
        {
            Debug.LogError("MicDoor component with tag 'TutorialDoor2' not found!");
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
                        micDoor.OpenDoor();
                    }
                    else
                    {
                        Debug.Log("Speech did not match the target phrase. Try again.");
                        micDoor.CloseDoor();
                    }
                }
                else
                {
                    Debug.LogError("Display Text is not assigned in the Inspector!");
                        micDoor.CloseDoor();
                }
            }
            else
            {
                Debug.LogError("GoogleSTTService is not found in the scene!");
                micDoor.CloseDoor();
            }
        }
        else
        {
            Debug.LogWarning("No audio samples were recorded.");
            micDoor.CloseDoor();
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
