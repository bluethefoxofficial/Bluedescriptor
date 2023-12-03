using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ABI_RC.Core.UI;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000008 RID: 8
public class MusicPlayer : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000017 RID: 23 RVA: 0x00002500 File Offset: 0x00000700
	// (remove) Token: 0x06000018 RID: 24 RVA: 0x00002538 File Offset: 0x00000738
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event MusicPlayer.PlaybackTimeUpdate OnPlaybackTimeUpdate;

	// Token: 0x06000019 RID: 25 RVA: 0x00002570 File Offset: 0x00000770
	public void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.name = "BLUEDESCRIPTOR MUSIC PLAYER";
		this.audioSource = base.gameObject.AddComponent<AudioSource>();
		this.audioSource = base.GetComponent<AudioSource>();
		base.StartCoroutine(this.PlaybackTimeUpdateLoop());
		bool flag = this.audioSource == null;
		if (flag)
		{
			MelonLogger.Error("MusicPlayer: No AudioSource component found on the GameObject, BD wont load correctly.");
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000025E2 File Offset: 0x000007E2
	private IEnumerator PlaybackTimeUpdateLoop()
	{
		for (;;)
		{
			float currentPlaybackTime = this.GetCurrentPlaybackTime();
			MusicPlayer.PlaybackTimeUpdate onPlaybackTimeUpdate = this.OnPlaybackTimeUpdate;
			if (onPlaybackTimeUpdate != null)
			{
				onPlaybackTimeUpdate(currentPlaybackTime);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600001B RID: 27 RVA: 0x000025F1 File Offset: 0x000007F1
	public void SetVolume(float volume)
	{
		this.audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002610 File Offset: 0x00000810
	public void AddSongToPlaylist(AudioClip song)
	{
		try
		{
			this.playlist.Add(song);
		}
		catch (Exception ex)
		{
			MelonLogger.Error(ex);
			CohtmlHud.Instance.ViewDropTextImmediate("Music+", "Failed to add song to playlist check melon console", "");
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002668 File Offset: 0x00000868
	public void RemoveSongFromPlaylist(AudioClip song)
	{
		this.playlist.Remove(song);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002678 File Offset: 0x00000878
	public void Play()
	{
		bool flag = this.playlist.Count > 0 && this.audioSource != null;
		if (flag)
		{
			this.audioSource.clip = this.playlist[this.currentTrackIndex];
			this.audioSource.Play();
		}
		else
		{
			MelonLogger.Error("Failed to play");
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000026E4 File Offset: 0x000008E4
	public List<string> GetMusicFiles()
	{
		List<string> list = new List<string>();
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		try
		{
			list.AddRange(Directory.GetFiles(folderPath, "*.mp3", SearchOption.AllDirectories));
			list.AddRange(Directory.GetFiles(folderPath, "*.wav", SearchOption.AllDirectories));
			list.AddRange(Directory.GetFiles(folderPath, "*.ogg", SearchOption.AllDirectories));
		}
		catch (Exception ex)
		{
			MelonLogger.Error("An error occurred while accessing the music folder: " + ex.Message);
		}
		return list;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002770 File Offset: 0x00000970
	public void Pause()
	{
		bool isPlaying = this.audioSource.isPlaying;
		if (isPlaying)
		{
			this.audioSource.Pause();
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000279C File Offset: 0x0000099C
	public void PlayNext()
	{
		bool flag = this.playlist.Count > 0;
		if (flag)
		{
			this.currentTrackIndex = (this.currentTrackIndex + 1) % this.playlist.Count;
			this.audioSource.Stop();
			this.Play();
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000027EC File Offset: 0x000009EC
	public void PlayPrevious()
	{
		bool flag = this.playlist.Count > 0;
		if (flag)
		{
			bool flag2 = this.currentTrackIndex == 0;
			if (flag2)
			{
				this.currentTrackIndex = this.playlist.Count - 1;
			}
			else
			{
				this.currentTrackIndex--;
			}
			this.audioSource.Stop();
			this.Play();
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002855 File Offset: 0x00000A55
	public AudioClip[] GetAllSongsInPlaylist()
	{
		return this.playlist.ToArray();
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002864 File Offset: 0x00000A64
	public float GetCurrentPlaybackTime()
	{
		bool flag = this.audioSource != null && this.audioSource.clip != null;
		float result;
		if (flag)
		{
			result = this.audioSource.time;
		}
		else
		{
			result = 0f;
		}
		return result;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000028B4 File Offset: 0x00000AB4
	public float GetTotalDuration()
	{
		bool flag = this.audioSource != null && this.audioSource.clip != null;
		float result;
		if (flag)
		{
			result = this.audioSource.clip.length;
		}
		else
		{
			result = 0f;
		}
		return result;
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002908 File Offset: 0x00000B08
	public void SetPlaybackPosition(float time)
	{
		bool flag = this.audioSource != null && this.audioSource.clip != null;
		if (flag)
		{
			time = Mathf.Clamp(time, 0f, this.audioSource.clip.length);
			this.audioSource.time = time;
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002968 File Offset: 0x00000B68
	public string GetCurrentPlaybackDuration()
	{
		float currentPlaybackTime = this.GetCurrentPlaybackTime();
		float totalDuration = this.GetTotalDuration();
		string str = this.FormatTime(currentPlaybackTime);
		string str2 = this.FormatTime(totalDuration);
		return str + " / " + str2;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x000029A8 File Offset: 0x00000BA8
	private string FormatTime(float timeInSeconds)
	{
		return TimeSpan.FromSeconds((double)timeInSeconds).ToString("mm\\:ss");
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000029CE File Offset: 0x00000BCE
	public IEnumerator LoadAndAddSongToPlaylist(string path)
	{
		AudioType audioType = this.GetAudioType(path);
		using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
		{
			yield return uwr.SendWebRequest();
			bool flag = uwr.result == 2 || uwr.result == 3;
			if (flag)
			{
				MelonLogger.Error(uwr.error);
				CohtmlHud.Instance.ViewDropTextImmediate("Music+", "Failed to add song " + path, "");
				yield break;
			}
			AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
			clip.name = Path.GetFileName(path);
			this.AddSongToPlaylist(clip);
			clip = null;
		}
		UnityWebRequest uwr = null;
		yield break;
		yield break;
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000029E4 File Offset: 0x00000BE4
	private AudioType GetAudioType(string path)
	{
		string text = Path.GetExtension(path).ToLowerInvariant();
		string text2 = text;
		string a = text2;
		AudioType result;
		if (!(a == ".ogg"))
		{
			if (!(a == ".wav"))
			{
				if (!(a == ".mp3"))
				{
					MelonLogger.Warning("Unsupported audio type: " + text);
					result = 0;
				}
				else
				{
					result = 13;
				}
			}
			else
			{
				result = 20;
			}
		}
		else
		{
			result = 14;
		}
		return result;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002A50 File Offset: 0x00000C50
	public string CurrentlyPlaying()
	{
		bool isPlaying = this.audioSource.isPlaying;
		string result;
		if (isPlaying)
		{
			result = this.audioSource.clip.name;
		}
		else
		{
			result = "Nothing is playing right now.";
		}
		return result;
	}

	// Token: 0x04000007 RID: 7
	public AudioSource audioSource;

	// Token: 0x04000008 RID: 8
	public List<AudioClip> playlist = new List<AudioClip>();

	// Token: 0x04000009 RID: 9
	public int currentTrackIndex;

	// Token: 0x02000025 RID: 37
	// (Invoke) Token: 0x060000CA RID: 202
	public delegate void PlaybackTimeUpdate(float time);
}
