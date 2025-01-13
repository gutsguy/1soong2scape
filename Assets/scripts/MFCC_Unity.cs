using UnityEngine;

public class MFCCProcessor : MonoBehaviour
{
    public static float[] ExtractMFCC(AudioClip clip)
    {
        int sampleCount = clip.samples;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        // Apply FFT (간단한 FFT)
        float[] fftData = ApplyFFT(samples);

        // Apply Mel Scale (멜 스케일 적용)
        float[] melCoefficients = ApplyMelScale(fftData);

        // Log Transformation (로그 변환)
        for (int i = 0; i < melCoefficients.Length; i++)
        {
            melCoefficients[i] = Mathf.Log10(melCoefficients[i] + 1);
        }

        return melCoefficients;
    }

    public static float[] ExtractMFCC(float[] samples)
    {
        // Apply FFT (간단한 FFT)
        float[] fftData = ApplyFFT(samples);

        // Apply Mel Scale (멜 스케일 적용)
        float[] melCoefficients = ApplyMelScale(fftData);

        // Log Transformation (로그 변환)
        for (int i = 0; i < melCoefficients.Length; i++)
        {
            melCoefficients[i] = Mathf.Log10(melCoefficients[i] + 1);
        }

        return melCoefficients;
    }

    private static float[] ApplyFFT(float[] samples)
    {
        int n = samples.Length;
        float[] fftData = new float[n / 2];

        // 간단한 FFT 예제 (복잡한 구현은 생략)
        for (int i = 0; i < fftData.Length; i++)
        {
            fftData[i] = Mathf.Abs(samples[i]);
        }

        return fftData;
    }

    private static float[] ApplyMelScale(float[] fftData)
    {
        int melBins = 13; // 멜 스케일 계수의 수 (13개로 제한)
        float[] melCoefficients = new float[melBins];

        // 간단히 멜 스케일 값 생성
        for (int i = 0; i < melBins; i++)
        {
            melCoefficients[i] = fftData[i % fftData.Length];
        }

        return melCoefficients;
    }
}
