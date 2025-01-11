using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;

public class VoiceController : MonoBehaviour
{
    private Recorder recorder;
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
        if (Input.GetKeyDown(KeyCode.V))  // "V" 키를 누르면
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;  // 마이크 ON/OFF 전환
            Debug.Log(recorder.TransmitEnabled ? "Microphone ON" : "Microphone OFF");
            micIcon.enabled = recorder.TransmitEnabled;
        }
    }
}
