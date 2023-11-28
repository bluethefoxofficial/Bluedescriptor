using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    bool m_IsPlaying;
    public void PlayAudio(string audioFileName) => StartCoroutine(LoadAndPlayAudio(audioFileName));

    IEnumerator LoadAndPlayAudio(string audioFileName)
    {
        var dllLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directory = System.IO.Path.GetDirectoryName(dllLocation);
        var audioFilePath = System.IO.Path.Combine(directory, "bluedescriptor", audioFileName);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var clip = DownloadHandlerAudioClip.GetContent(www);
                if (m_IsPlaying) { m_IsPlaying = false; yield break; }
                if (GameObject.Find("AudioObject_" + audioFileName)) { Destroy(GameObject.Find("AudioObject_" + audioFileName)); }
                var audioObject = new GameObject("AudioObject_" + audioFileName);
                var audioSource = audioObject.AddComponent<AudioSource>();

                audioSource.clip = clip;

                audioSource.Play();

                Destroy(audioObject, clip.length);
            }
            else
            {
                MelonLogger.Error($"Failed to load audio: {www.error}");
            }
        }
    }
}