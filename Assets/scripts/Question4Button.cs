using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Photon.Pun;
using TMPro;

public class Question4Button : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private bool isRecording = false;
    private PhotonView[] doorPhotonViews;
    private MicDoor[] micDoors;
    private GameObject[] doorObjects;
    private float recordingDuration = 5f; // 최대 녹음 시간 (5초)
    private const string ReferenceAudioPath = "Assets/MonkeySound.wav";
    private float elapsedTime = 0f;
    public GameObject micStatusIcon;
    

    void Start(){
        doorObjects = GameObject.FindGameObjectsWithTag("Question4Door");
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


        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            Debug.Log("Available Microphones:");
            foreach (var device in devices)
            {
                Debug.Log($"- {device}");
            }
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    // Start is called before the first frame update
    private void OnMouseDown()
    {
        // Cube 클릭 시 녹음 시작/중지 전환
        isRecording = !isRecording;

        if (isRecording)
        {
            elapsedTime = 0f;
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void StartRecording(){
        micStatusIcon.SetActive(true);
        Debug.Log("Recoding Started");
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

    private void StopRecording(){
        micStatusIcon.SetActive(false);
        isRecording = false;

        Debug.Log("Recording stopped...");
        Microphone.End(null);

        if (micClip == null)
        {
            Debug.LogError("No microphone clip available.");
            return;
        }

        string recordedPath = Path.Combine(Application.persistentDataPath, "RecordedAudio.wav");
        SaveAudioClipToWav(micClip, recordedPath);

        CompareWithReference(recordedPath, ReferenceAudioPath);
    }

    private void Update()
    {
        if (isRecording)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= recordingDuration)
            {
                isRecording = !isRecording;
                StopRecording();
            }
        }
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

        int histogramBin = 1000;
        int[] histogram1 = ComputeHistogram(ConvertPCMToFloat(recordedPCM), histogramBin);
        int[] histogram2 = ComputeHistogram(ConvertPCMToFloat(referencePCM), histogramBin);

        float difference = CompareHistograms(histogram1, histogram2);
        // // 유사도 계산
        // float similarity = CalculateSimilarity(recordedPCM, referencePCM);

        // 결과 출력
        Debug.Log($"difference: {difference}");
        if (difference < 600000) // 임계값 30%
        {
            Debug.Log("Match successful! Speech is similar.");
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
            Debug.Log("Match failed. Speech is not similar.");
            foreach (MicDoor door in micDoors)
            {
                foreach (PhotonView doorView in doorPhotonViews)
                {
                    if (doorView != null)
                    {
                        doorView.RPC("CloseDoorNetwork", RpcTarget.All);
                    }
                }
            }
        }
    }

    private float[] ConvertPCMToFloat(byte[] pcmData)
    {
        // PCM 데이터 유효성 검사
        if (pcmData == null || pcmData.Length % 2 != 0)
        {
            Debug.LogError("Invalid PCM data: Length must be even.");
            return null;
        }

        int sampleCount = pcmData.Length / 2; // 2바이트 = 16비트 샘플
        float[] floatSamples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            // PCM 데이터는 리틀 엔디안 포맷이므로 Int16으로 변환
            short sample = BitConverter.ToInt16(pcmData, i * 2);

            // -32768~32767 범위를 -1.0f~1.0f로 정규화
            floatSamples[i] = sample / 32768.0f;
        }

        return floatSamples;
    }

    private int[] ComputeHistogram(float[] signal, int numBins = 10)
    {
        int[] histogram = new int[numBins];
        float min = Mathf.Min(signal);
        float max = Mathf.Max(signal);
        float binSize = (max - min) / numBins;

        foreach (float sample in signal)
        {
            int binIndex = Mathf.Clamp((int)((sample - min) / binSize), 0, numBins - 1);
            histogram[binIndex]++;
        }

        return histogram;
    }

    private float CompareHistograms(int[] hist1, int[] hist2)
    {
        float diff = 0;
        for (int i = 0; i < Mathf.Min(hist1.Length, hist2.Length); i++)
        {
            diff += Mathf.Abs(hist1[i] - hist2[i]);
        }
        return diff;
    }

}