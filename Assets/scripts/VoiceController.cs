using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        if (clip == null) throw new ArgumentNullException(nameof(clip));

        MemoryStream stream = new MemoryStream();
        WriteWavFile(stream, clip);
        return stream.ToArray();
    }

    private static void WriteWavFile(Stream stream, AudioClip clip)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int frequency = clip.frequency;
            int channels = clip.channels;
            float[] samples = new float[clip.samples * channels];
            clip.GetData(samples, 0);

            // WAV Header
            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + samples.Length * 2); // 파일 전체 크기
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write(16); // fmt 청크 크기
            writer.Write((short)1); // 오디오 포맷 (1 = PCM)
            writer.Write((short)channels);
            writer.Write(frequency);
            writer.Write(frequency * channels * 2); // ByteRate
            writer.Write((short)(channels * 2)); // BlockAlign
            writer.Write((short)16); // BitsPerSample

            // Data Chunk
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(samples.Length * 2); // 데이터 크기
            foreach (var sample in samples)
            {
                short intSample = (short)(sample * short.MaxValue);
                writer.Write(intSample);
            }
        }
    }
}


public class VoiceController : MonoBehaviour
{
    private Recorder recorder;
    private AudioClip micClip;
    private const int SamplingRate = 16000;
    private const string ReferenceAudioPath = "Assets/m_365.wav";
    private string micDevice;

    private bool isTalking = false;  // 대화 기능 상태
    private bool isRecording = false; // 녹음 기능 상태
    private bool isTRecording = false;

    public Image micIcon;

    void Start()
    {
        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            Debug.LogError("Recorder component is missing!");
            return;
        }

        micIcon = GameObject.Find("MicIcon")?.GetComponent<Image>();
        if (micIcon == null)
        {
            Debug.LogError("MicIcon Image not found in the scene!");
            return;
        }
        recorder.TransmitEnabled = false;  // 기본적으로 음성 전송 OFF
        micIcon.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // "V" 키로 대화 시작/중단
        {
            ToggleTalking();
        }

        if (Input.GetKeyDown(KeyCode.R)) // "R" 키로 녹음 시작/중단
        {
            ToggleRecording();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTRecording();
        }
    }

    private void ToggleTalking()
    {
        recorder.TransmitEnabled = !recorder.TransmitEnabled;  // 마이크 ON/OFF 전환
        Debug.Log(recorder.TransmitEnabled ? "Microphone ON" : "Microphone OFF");
        micIcon.enabled = recorder.TransmitEnabled;
    }

    private void ToggleRecording()
    {
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

    private void StopRecording()
    {
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
                sttService.RecognizeSpeech(pcmData).Forget();
            }
            else
            {
                Debug.LogError("GoogleSTTService is not found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning("No audio samples were recorded.");
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

    private void ToggleTRecording()
    {
        isTRecording = !isTRecording;

        if (isTRecording)
        {
            StartTRecording();
        }
        else
        {
            StopTRecordingAndCompare();
        }
    }

    private void StartTRecording()
    {
        Debug.Log("T Recoding Started");
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

        Debug.Log("T Microphone recording started successfully.");
    }

    private void StopTRecordingAndCompare()
    {
        Debug.Log("T Recording stopped...");
        Microphone.End(null);
        isTRecording = false;

        if (micClip == null)
        {
            Debug.LogError("No microphone clip available.");
            return;
        }

        string recordedPath = Path.Combine(Application.persistentDataPath, "RecordedAudio.wav");
        SaveAudioClipToWav(micClip, recordedPath);

        // 기존 파일과 비교
        CompareWithReference(recordedPath, ReferenceAudioPath);
    }

    private void SaveAudioClipToWav(AudioClip clip, string filePath)
    {
        byte[] wavData = WavUtility.FromAudioClip(clip);
        File.WriteAllBytes(filePath, wavData);
        Debug.Log($"Saved WAV to: {filePath}");
    }

    private void CompareWithReference(string recordedPath, string referencePath)
    {
        if (!File.Exists(referencePath))
        {
            Debug.LogError($"Reference file not found at: {referencePath}");
            return;
        }

        // PCM 데이터 추출
        byte[] recordedPCM = File.ReadAllBytes(recordedPath);
        byte[] referencePCM = File.ReadAllBytes(referencePath);

        // 유사도 계산
        float similarity = CalculateSimilarity(recordedPCM, referencePCM);

        // 결과 출력
        Debug.Log($"Similarity: {similarity * 100:F2}%");
        if (similarity >= 0.85f) // 임계값 85%
        {
            Debug.Log("Match successful! Speech is similar.");
        }
        else
        {
            Debug.Log("Match failed. Speech is not similar.");
        }
    }

    private float CalculateSimilarity(byte[] pcm1, byte[] pcm2)
    {
        // PCM 데이터를 비교하여 유사도를 계산하는 로직
        // 예: 코사인 유사도 또는 DTW (간단한 코사인 유사도 예제)
        int minLength = Mathf.Min(pcm1.Length, pcm2.Length);
        float dotProduct = 0f;
        float magnitude1 = 0f;
        float magnitude2 = 0f;

        for (int i = 0; i < minLength; i++)
        {
            dotProduct += pcm1[i] * pcm2[i];
            magnitude1 += pcm1[i] * pcm1[i];
            magnitude2 += pcm2[i] * pcm2[i];
        }

        return dotProduct / (Mathf.Sqrt(magnitude1) * Mathf.Sqrt(magnitude2));
    }
}
