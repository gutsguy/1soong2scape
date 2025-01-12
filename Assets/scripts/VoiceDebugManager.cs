using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public class VoiceDebugManager : MonoBehaviour
{

    void Awake()
    {
        Debug.Log("[VoiceDebug] Awake() 호출됨 - VoiceDebugManager 초기화");
    }
    void Start()
    {
        Debug.Log("[VoiceDebug] Checking PunVoiceClient status...");

        if (PunVoiceClient.Instance == null)
        {
            Debug.LogError("[VoiceDebug] PunVoiceClient.Instance is null!");
            return;
        }

        if (!PunVoiceClient.Instance.Client.IsConnected)
        {
            Debug.LogError("[VoiceDebug] Not connected to Photon Voice Server!");
        }
        else
        {
            Debug.Log("[VoiceDebug] Connected to Photon Voice Server.");
        }

        if (!PunVoiceClient.Instance.Client.InRoom)
        {
            Debug.LogError("[VoiceDebug] Not in a room!");
        }
        else
        {
            Debug.Log("[VoiceDebug] Successfully joined the room.");
        }

        Debug.Log("[VoiceDebug] Checking Primary Recorder...");

        if (PunVoiceClient.Instance.PrimaryRecorder == null)
        {
            Debug.LogError("[VoiceDebug] PrimaryRecorder is null!");
        }
        else
        {
            Debug.Log($"[VoiceDebug] PrimaryRecorder is initialized. TransmitEnabled: {PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled}");
            Debug.Log($"[VoiceDebug] Debug Echo Mode: {PunVoiceClient.Instance.PrimaryRecorder.DebugEchoMode}");
            Debug.Log($"[VoiceDebug] Input Source Type: {PunVoiceClient.Instance.PrimaryRecorder.SourceType}");
        }
    }
}
