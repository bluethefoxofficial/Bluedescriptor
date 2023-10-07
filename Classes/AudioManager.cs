using UnityEngine;
using UnityEngine.Networking;
using MelonLoader;
using System.Collections;
using System.Diagnostics.Eventing.Reader;

public class AudioManager : MonoBehaviour
{

    private bool m_IsPlaying = false;
    public void PlayAudio(string audioFileName)
    {
        StartCoroutine(LoadAndPlayAudio(audioFileName));
    }

    private IEnumerator LoadAndPlayAudio(string audioFileName)
    {
        string dllLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string directory = System.IO.Path.GetDirectoryName(dllLocation);
        string audioFilePath = System.IO.Path.Combine(directory, "bluedescriptor", audioFileName);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (m_IsPlaying) { m_IsPlaying = false; yield break; }
                if (GameObject.Find("AudioObject_" + audioFileName)) { Destroy(GameObject.Find("AudioObject_" + audioFileName));  }
                GameObject audioObject = new GameObject("AudioObject_" + audioFileName);
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();

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
