using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClickAudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isAudioLoaded = false;
    private AudioClip loadedClip;
    public GameObject micStatusIcon;

    void Start(){
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing!");
            return;
        }

        if (micStatusIcon != null)
        {
            micStatusIcon.SetActive(false);  // 기본적으로 비활성화
        }

        audioSource.volume = 1.0f;
        audioSource.mute = false;
        audioSource.spatialBlend = 0.0f;

        string audioFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "MonkeySound.wav");
        Debug.Log($"File path: {audioFilePath}");
        Debug.Log($"File exists: {System.IO.File.Exists(audioFilePath)}");

        StartCoroutine(LoadAudio(audioFilePath));
    }

    private IEnumerator LoadAudio(string path)
    {
        Debug.Log("Starting audio load...");
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                loadedClip = DownloadHandlerAudioClip.GetContent(www);
                if (loadedClip != null)
                {
                    audioSource.clip = loadedClip;
                    isAudioLoaded = true;
                    Debug.Log($"Audio loaded successfully. Length: {loadedClip.length}s");
                }
                else
                {
                    Debug.LogError("Failed to get AudioClip from downloaded data.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load audio: {www.error}");
            }
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"isAudioLoaded: {isAudioLoaded}");
        if (isAudioLoaded)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                micStatusIcon.SetActive(false);
                Debug.Log("Audio stopped.");
            }
            else
            {
                audioSource.Play();
                micStatusIcon.SetActive(true);
                Debug.Log("Audio playing...");

                StartCoroutine(DisableMicStatusIconAfterDelay(5.8f));
            }
        }
        else
        {
            Debug.LogWarning("Audio is not loaded yet.");
        }
    }

    IEnumerator DisableMicStatusIconAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // delay 시간 만큼 대기
        micStatusIcon.SetActive(false);          // 5초 후 아이콘을 비활성화
    }
}
