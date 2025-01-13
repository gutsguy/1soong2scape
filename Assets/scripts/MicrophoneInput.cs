using System;
using UnityEngine;
using UniRx;

public class MicrophoneInput : MonoBehaviour
{
    private const int SampleWindow = 128; // 샘플링 윈도우 크기
    private const float VoiceThreshold = 0.25f; // 음성 감지 임계값
    private const float VADTimeout = 1.0f; // 음성 종료 감지 시간 (1초)

    private AudioClip microphoneClip; // 마이크 녹음 데이터 저장
    private float lastVoiceDetectedTime; // 마지막 음성 감지 시간

    public ReactiveCommand<byte[]> OnMaxLevelChangeCommand = new(); // 이벤트

    private void Start()
    {
        // 마이크 시작
        microphoneClip = Microphone.Start(null, true, 10, 16000); // 16kHz 샘플링
        lastVoiceDetectedTime = Time.time;
    }

    private void FixedUpdate()
    {
        CheckMaxLevel();

        if (Time.time - lastVoiceDetectedTime > VADTimeout)
        {
            var microphoneData = GetMicrophoneData();
            if (microphoneData != null)
            {
                OnMaxLevelChangeCommand.Execute(microphoneData); // 데이터 전달
            }
            lastVoiceDetectedTime = Time.time;
        }
    }

    private void CheckMaxLevel()
    {
        float maxLevel = 0f;
        float[] samples = new float[SampleWindow];
        int startPosition = Microphone.GetPosition(null) - SampleWindow + 1;

        if (startPosition > 0)
        {
            microphoneClip.GetData(samples, startPosition);
            foreach (var sample in samples)
            {
                if (Mathf.Abs(sample) > maxLevel)
                {
                    maxLevel = Mathf.Abs(sample);
                }
            }

            if (maxLevel > VoiceThreshold)
            {
                lastVoiceDetectedTime = Time.time; // 음성 감지
            }
        }
    }

    private byte[] GetMicrophoneData()
    {
        float[] samples = new float[microphoneClip.samples * microphoneClip.channels];
        microphoneClip.GetData(samples, 0);

        byte[] audioData = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * short.MaxValue);
            byte[] sampleBytes = BitConverter.GetBytes(sample);
            audioData[i * 2] = sampleBytes[0];
            audioData[i * 2 + 1] = sampleBytes[1];
        }
        return audioData;
    }
}
