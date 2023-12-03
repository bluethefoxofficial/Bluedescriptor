using System;
using System.Collections;
using System.IO;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000004 RID: 4
public class AudioManager : MonoBehaviour
{
	// Token: 0x0600000C RID: 12 RVA: 0x00002250 File Offset: 0x00000450
	public void PlayAudio(string audioFileName)
	{
		base.StartCoroutine(this.LoadAndPlayAudio(audioFileName));
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002260 File Offset: 0x00000460
	private IEnumerator LoadAndPlayAudio(string audioFileName)
	{
		string dllLocation = Assembly.GetExecutingAssembly().Location;
		string directory = Path.GetDirectoryName(dllLocation);
		string audioFilePath = Path.Combine(directory, "bluedescriptor", audioFileName);
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, 20))
		{
			yield return www.SendWebRequest();
			bool flag = www.result == 1;
			if (flag)
			{
				AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
				bool isPlaying = this.m_IsPlaying;
				if (isPlaying)
				{
					this.m_IsPlaying = false;
					yield break;
				}
				bool flag2 = GameObject.Find("AudioObject_" + audioFileName);
				if (flag2)
				{
					Object.Destroy(GameObject.Find("AudioObject_" + audioFileName));
				}
				GameObject audioObject = new GameObject("AudioObject_" + audioFileName);
				AudioSource audioSource = audioObject.AddComponent<AudioSource>();
				audioSource.clip = clip;
				audioSource.Play();
				Object.Destroy(audioObject, clip.length);
				clip = null;
				audioObject = null;
				audioSource = null;
			}
			else
			{
				MelonLogger.Error("Failed to load audio: " + www.error);
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}

	// Token: 0x04000005 RID: 5
	private bool m_IsPlaying;
}
