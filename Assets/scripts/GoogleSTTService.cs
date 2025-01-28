using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GoogleSTTService : MonoBehaviour
{
    private const string API_KEY = "AIzaSyBHFQmCj1tgVSoSNbDzLWOBDhwL54TDQr8"; // Google Cloud API 키
    private const string URL = "https://speech.googleapis.com/v1/speech:recognize?key=";
    private const string Locale = "ko-KR"; // 언어 설정

    [SerializeField] private Text transcriptText; // 변환된 텍스트를 표시할 UI
    [SerializeField] private string targetPhrase = "끼끼끼우끼우우끼끼";

    public async UniTaskVoid RecognizeSpeech(byte[] audioData)
    {
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogWarning("Audio data is null or empty. Cannot send to Google STT.");
            return;
        }

        string audioContent = Convert.ToBase64String(audioData);
        string requestJson = $"{{\"config\": {{\"encoding\":\"LINEAR16\",\"sampleRateHertz\":16000,\"languageCode\":\"{Locale}\"}},\"audio\":{{\"content\":\"{audioContent}\"}}}}";
        string fullUrl = URL + API_KEY;

        using var request = new UnityWebRequest(fullUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending request to Google STT...");
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Google STT Request Failed: {request.error}");
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log($"Google STT Response: {responseText}");

            try
            {
                var json = JObject.Parse(responseText);
                string transcript = json["results"]?[0]?["alternatives"]?[0]?["transcript"]?.ToString();

                if (!string.IsNullOrEmpty(transcript))
                {
                    Debug.Log($"Recognized Speech: {transcript}");

                    // 텍스트를 UI에 표시
                    if (transcriptText != null)
                    {
                        transcriptText.text = transcript;
                    }

                    // VoiceComparer를 사용하여 유사도 계산
                    float Levenshtein_similarity = Levenshtein.CalculateSimilarity(transcript, targetPhrase);

                    // 결과 출력
                    Debug.Log($"Levenshtein Similarity: {Levenshtein_similarity * 100}%");

                    float LCS_similarity = LCS.CalculateSimilarity(transcript, targetPhrase);
                    Debug.Log($"LCS Similarity: {LCS_similarity * 100}%");

                    float similarity = MathF.Max(Levenshtein_similarity, LCS_similarity);

                    if (similarity >= 0.85f) // 유사도가 85% 이상
                    {
                        Debug.Log("Speech matched the target phrase! Success!");
                    }
                    else
                    {
                        Debug.Log("Speech did not match the target phrase. Try again.");
                    }
                }
                else
                {
                    Debug.LogWarning("No transcript found in Google STT response.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse Google STT response: {e.Message}");
            }
        }
    }

    public async Task<string> GetTextFromSpeech(byte[] audioData)
    {
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogWarning("Audio data is null or empty. Cannot send to Google STT.");
            return null;
        }

        string audioContent = Convert.ToBase64String(audioData);
        string requestJson = $"{{\"config\": {{\"encoding\":\"LINEAR16\",\"sampleRateHertz\":16000,\"languageCode\":\"{Locale}\"}},\"audio\":{{\"content\":\"{audioContent}\"}}}}";
        string fullUrl = URL + API_KEY;

        using var request = new UnityWebRequest(fullUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending request to Google STT...");
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Google STT Request Failed: {request.error}");
            return null; // 에러 발생 시 null 반환
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log($"Google STT Response: {responseText}");
            try
            {
                // JSON 파싱
                var json = JObject.Parse(responseText);
                string transcript = json["results"]?[0]?["alternatives"]?[0]?["transcript"]?.ToString();
                float confidence = json["results"]?[0]?["alternatives"]?[0]?["confidence"]?.ToObject<float>() ?? 0;

                // 화면에 표시
                if (!string.IsNullOrEmpty(transcript))
                {
                    Debug.Log($"Transcript: {transcript}");
                    Debug.Log($"Confidence: {confidence * 100:F2}%");

                    // 텍스트 UI에 표시
                    if (transcriptText != null)
                    {
                        transcriptText.text = $"Transcript: {transcript}\nConfidence: {confidence * 100:F2}%";
                    }

                    return transcript; // transcript 반환
                }
                else
                {
                    Debug.LogWarning("No transcript found in STT response.");
                    if (transcriptText != null)
                    {
                        transcriptText.text = "No transcript found.";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse JSON: {ex.Message}");
            }
        }

        return null; // 예외 상황에서 null 반환

    }
}
