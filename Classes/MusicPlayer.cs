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
    public AudioSource audioSource;
    public List<AudioClip> playlist = new List<AudioClip>();
    public int currentTrackIndex;
    public delegate void PlaybackTimeUpdate(float time);
    public event PlaybackTimeUpdate OnPlaybackTimeUpdate;
    public void Awake()
    {
        // Prevent this object from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
        gameObject.name = "BLUEDESCRIPTOR MUSIC PLAYER";

        audioSource = gameObject.AddComponent<AudioSource>();

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlaybackTimeUpdateLoop());

        if (audioSource == null)
        {
            // Log an error if no AudioSource is found
            MelonLogger.Error("MusicPlayer: No AudioSource component found on the GameObject, BD wont load correctly.");
        }
    }

    IEnumerator PlaybackTimeUpdateLoop()
    {
        while (true)
        {
            // Fetch current playback time
            var currentPlaybackTime = GetCurrentPlaybackTime();

            // Notify other components
            OnPlaybackTimeUpdate?.Invoke(currentPlaybackTime);

            // Wait until the next frame
            yield return null;
        }
    }

    public void SetVolume(float volume)
    {
        // Set the volume, assuming volume is given as a float between 0.0f and 1.0f
        audioSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }

    public void AddSongToPlaylist(AudioClip song)
    {
        try
        {
            playlist.Add(song);
        }
        catch (Exception e)
        {
            MelonLogger.Error(e);

            CohtmlHud.Instance
                .ViewDropTextImmediate($"Music+", $"Failed to add song to playlist check melon console", "");
        }
    }

    public void RemoveSongFromPlaylist(AudioClip song) => playlist.Remove(song);

    public void Play()
    {
        if (playlist.Count > 0 && audioSource != null)
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
        var musicFiles = new List<string>();
        var musicFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        try
        {
            // Get all .mp3 files
            musicFiles.AddRange(Directory.GetFiles(musicFolderPath, "*.mp3", SearchOption.AllDirectories));
            // Get all .wav files
            musicFiles.AddRange(Directory.GetFiles(musicFolderPath, "*.wav", SearchOption.AllDirectories));
            // Get all .ogg files
            musicFiles.AddRange(Directory.GetFiles(musicFolderPath, "*.ogg", SearchOption.AllDirectories));
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., access denied to the path)
            MelonLogger.Error("An error occurred while accessing the music folder: " + ex.Message);
        }

        return musicFiles;
    }

    public void Pause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void PlayNext()
    {
        if (playlist.Count > 0)
        {
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
            audioSource.Stop();
            Play();
        }
    }

    public void PlayPrevious()
    {
        if (playlist.Count > 0)
        {
            if (currentTrackIndex == 0)
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

    public AudioClip[] GetAllSongsInPlaylist() => playlist.ToArray();

    public float GetCurrentPlaybackTime()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            return audioSource.time;
        }
        else
        {
            return 0f;
        }
    }

    public float GetTotalDuration()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            return audioSource.clip.length;
        }
        else
        {
            return 0f;
        }
    }

    public void SetPlaybackPosition(float time)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // Clamp the time to make sure it's within the bounds of the audio clip
            time = Mathf.Clamp(time, 0f, audioSource.clip.length);
            audioSource.time = time;
        }
    }

    // Method to return both current playback time and total duration as a formatted string
    public string GetCurrentPlaybackDuration()
    {
        var currentPlaybackTime = GetCurrentPlaybackTime();
        var totalDuration = GetTotalDuration();

        // Format the string to show minutes and seconds
        var formattedCurrentTime = FormatTime(currentPlaybackTime);
        var formattedTotalDuration = FormatTime(totalDuration);

        return $"{formattedCurrentTime} / {formattedTotalDuration}";
    }

    // Helper method to format the time in minutes and seconds
    string FormatTime(float timeInSeconds)
    {
        var time = TimeSpan.FromSeconds(timeInSeconds);
        return time.ToString(@"mm\:ss");
    }

    public IEnumerator LoadAndAddSongToPlaylist(string path)
    {
        var audioType = GetAudioType(path);

        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                MelonLogger.Error(uwr.error);
                CohtmlHud.Instance.ViewDropTextImmediate($"Music+", $"Failed to add song {path}", "");
                yield break;
            }

            var clip = DownloadHandlerAudioClip.GetContent(uwr);
            clip.name = Path.GetFileName(path);
            AddSongToPlaylist(clip);
        }
    }

    AudioType GetAudioType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        switch (extension)
        {
            case ".ogg":
                return AudioType.OGGVORBIS;
            case ".wav":
                return AudioType.WAV;
            case ".mp3":
                return AudioType.MPEG;
            default:
                MelonLogger.Warning($"Unsupported audio type: {extension}");
                return AudioType.UNKNOWN;
        }
    }

    public string CurrentlyPlaying()
    {
        if (audioSource.isPlaying)
        {
            return audioSource.clip.name;
        }

        return "Nothing is playing right now.";
    }
}