
using MelonLoader;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
  private bool m_IsPlaying;

  public void PlayAudio(string audioFileName) => StartCoroutine(LoadAndPlayAudio(audioFileName));

  private IEnumerator LoadAndPlayAudio(string audioFileName)
  {
    string dllLocation = Assembly.GetExecutingAssembly().Location;
    string directory = Path.GetDirectoryName(dllLocation);
    string audioFilePath = Path.Combine(directory, "bluedescriptor", audioFileName);
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, (AudioType) 20))
    {
      yield return  www.SendWebRequest();
      if (www.result == UnityWebRequest.Result.Success)
      {
        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        if (m_IsPlaying)
        {
          m_IsPlaying = false;
        }
        else
        {
          if ( GameObject.Find("AudioObject_" + audioFileName))
            GameObject.Destroy( GameObject.Find("AudioObject_" + audioFileName));
          GameObject audioObject = new GameObject("AudioObject_" + audioFileName);
          AudioSource audioSource = audioObject.AddComponent<AudioSource>();
          audioSource.clip = clip;
          audioSource.Play();
          GameObject.Destroy( audioObject, clip.length);
          clip = null;
          audioObject = null;
          audioSource = null;
        }
      }
      else
        MelonLogger.Error("Failed to load audio: " + www.error);
    }
  }
}
