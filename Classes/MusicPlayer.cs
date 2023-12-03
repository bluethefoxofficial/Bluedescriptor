using ABI_RC.Core.UI;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class MusicPlayer : MonoBehaviour
{

    public event MusicPlayer.PlaybackTimeUpdate OnPlaybackTimeUpdate;


    public void Awake()
    {
      DontDestroyOnLoad(base.gameObject);
        base.gameObject.name = "BLUEDESCRIPTOR MUSIC PLAYER";
        audioSource = base.gameObject.AddComponent<AudioSource>();
        audioSource = base.GetComponent<AudioSource>();
        base.StartCoroutine(PlaybackTimeUpdateLoop());
        bool flag = audioSource == null;
        if (flag)
        {
            MelonLogger.Error("MusicPlayer: No AudioSource component found on the GameObject, BD wont load correctly.");
        }
    }

    private IEnumerator PlaybackTimeUpdateLoop()
    {
        while (true)
        {
            float currentPlaybackTime = GetCurrentPlaybackTime();
            this.OnPlaybackTimeUpdate?.Invoke(currentPlaybackTime);
            yield return null;
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void AddSongToPlaylist(AudioClip song)
    {
        try
        {
            playlist.Add(song);
        }
        catch (Exception ex)
        {
            MelonLogger.Error(ex);
            CohtmlHud.Instance.ViewDropTextImmediate("Music+", "Failed to add song to playlist check melon console", "");
        }
    }

    public void RemoveSongFromPlaylist(AudioClip song)
    {
        playlist.Remove(song);
    }

    public void Play()
    {
        bool flag = playlist.Count > 0 && audioSource != null;
        if (flag)
        {
            audioSource.clip = playlist[currentTrackIndex];
            audioSource.Play();
        }
        else
        {
            MelonLogger.Error("Failed to play");
        }
    }

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

    public void Pause()
    {
        bool isPlaying = audioSource.isPlaying;
        if (isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void PlayNext()
    {
        bool flag = playlist.Count > 0;
        if (flag)
        {
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
            audioSource.Stop();
            Play();
        }
    }

    public void PlayPrevious()
    {
        bool flag = playlist.Count > 0;
        if (flag)
        {
            bool flag2 = currentTrackIndex == 0;
            if (flag2)
            {
                currentTrackIndex = playlist.Count - 1;
            }
            else
            {
                currentTrackIndex--;
            }
            audioSource.Stop();
            Play();
        }
    }

    public AudioClip[] GetAllSongsInPlaylist()
    {
        return playlist.ToArray();
    }

    public float GetCurrentPlaybackTime()
    {
        bool flag = audioSource != null && audioSource.clip != null;
        float result;
  
            result = audioSource.time;
        
   
        return result;
    }

    public float GetTotalDuration()
    {
        bool flag = audioSource != null && audioSource.clip != null;
        float result;
        if (flag)
        {
            result = audioSource.clip.length;
        }
        else
        {
            result = 0f;
        }
        return result;
    }

    public void SetPlaybackPosition(float time)
    {
        bool flag = audioSource != null && audioSource.clip != null;
        if (flag)
        {
            time = Mathf.Clamp(time, 0f, audioSource.clip.length);
            audioSource.time = time;
        }
    }

    public string GetCurrentPlaybackDuration()
    {
        float currentPlaybackTime = GetCurrentPlaybackTime();
        float totalDuration = GetTotalDuration();
        string str = FormatTime(currentPlaybackTime);
        string str2 = FormatTime(totalDuration);
        return str + " / " + str2;
    }


    private string FormatTime(float timeInSeconds)
    {
        return TimeSpan.FromSeconds((double)timeInSeconds).ToString("mm\\:ss");
    }


    public IEnumerator LoadAndAddSongToPlaylist(string path)
    {
        AudioType audioType = GetAudioType(path);
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                MelonLogger.Error(uwr.error);
                CohtmlHud.Instance.ViewDropTextImmediate("Music+", "Failed to add song " + path, "");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            clip.name = Path.GetFileName(path);
            AddSongToPlaylist(clip);
        }
    }

    private AudioType GetAudioType(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        switch (extension)
        {
            case ".ogg":
                return AudioType.OGGVORBIS; // Assuming AudioType.OGGVORBIS is the correct enum value for .ogg

            case ".wav":
                return AudioType.WAV; // Assuming AudioType.WAV is the correct enum value for .wav

            case ".mp3":
                return AudioType.MPEG; // Assuming AudioType.MPEG is the correct enum value for .mp3

            default:
                MelonLogger.Warning("Unsupported audio type: " + extension);
                return 0; // Assuming 0 is the default or 'unsupported' value
        }
    }


    public string CurrentlyPlaying()
    {
        bool isPlaying = audioSource.isPlaying;
        string result;
        if (isPlaying)
        {
            result = audioSource.clip.name;
        }
        else
        {
            result = "Nothing is playing right now.";
        }
        return result;
    }

    public AudioSource audioSource;

    public List<AudioClip> playlist = new List<AudioClip>();

    public int currentTrackIndex;

    public delegate void PlaybackTimeUpdate(float time);
}
