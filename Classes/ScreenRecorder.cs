using ABI_RC.Core.UI;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class ScreenRecorder
{
    bool isRecording;
    string framesDirectory;
    string audioFilePath;
    string outputFilePath;
    Process ffmpegProcess;
    int frameCount;
    float startTime;
    AudioSource audioSource;
    AudioClip recordingClip;
    List<float> frameratesOverTime = new List<float>();
    public ScreenRecorder()
    {
        var currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var directoryPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "bluedescriptor", "recordings");
        Directory.CreateDirectory(directoryPath);

        framesDirectory = Path.Combine(directoryPath, UnityEngine.Random.Range(100000, 999999).ToString());
        Directory.CreateDirectory(framesDirectory);

        audioFilePath = Path.Combine(directoryPath, currentDateTime + ".wav");
        outputFilePath = Path.Combine(directoryPath, currentDateTime + ".mp4");
    }

    public IEnumerator StartRecording()
    {
        if (isRecording)
        {
            MelonLogger.Msg("Recording is already in progress.");
            yield break;
        }

        CohtmlHud.Instance
            .ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording started");

        isRecording = true;
        startTime = Time.time;
        audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = Microphone.Start(null, true, 10, 44100);

        while (Microphone.GetPosition(null) <= 0)
            yield return null;
        

        audioSource.Play();
        MelonLogger.Msg("Recording started.");

        while (isRecording)
            yield return CaptureFrame();
        
    }

    IEnumerator CaptureFrame()
    {
        frameratesOverTime.Add(1f / Time.deltaTime);
        var currentFrame = ScreenCapture.CaptureScreenshotAsTexture();
        var path = Path.Combine(framesDirectory, frameCount.ToString("D6") + ".jpg");
        frameCount++;

        yield return null;

        ThreadPool.QueueUserWorkItem(_ =>
        {
            var bytes = currentFrame.EncodeToJPG();
            File.WriteAllBytes(path, bytes);
            UnityEngine.Object.Destroy(currentFrame);
        });
    }

    public IEnumerator StopRecording()
    {
        if (!isRecording)
        {
            MelonLogger.Msg("No recording in progress.");
            yield break;
        }

        CohtmlHud.Instance
            .ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording being processed.");
        isRecording = false;
        var recordedTime = Time.time - startTime;
        audioSource.Stop();
        Microphone.End(null);

        SaveAudioClip(audioSource.clip, audioFilePath);
        UnityEngine.Object.Destroy(audioSource.gameObject);

        var frameDurationFilePath = WriteFrameDurationsToFile(frameratesOverTime);
        yield return CombineWithFFmpeg(frameDurationFilePath);
        MelonLogger.Msg("Recording stopped.");
    }

    string WriteFrameDurationsToFile(List<float> frameRates)
    {
        var filePath = Path.Combine(framesDirectory, "frameDurations.txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < frameRates.Count; i++)
            {
                var frameDuration = 1f / frameRates[i];
                var frameFileName = Path.Combine(framesDirectory, $"{i:D6}.jpg");

                writer.WriteLine($"file '{frameFileName}'");
                writer.WriteLine($"duration {frameDuration}");
            }

            // Add the last file without a duration to signal the end of the list
            if (frameRates.Count > 0)
            {
                var lastFrameFileName = Path.Combine(framesDirectory, $"{frameRates.Count - 1:D6}.jpg");
                writer.WriteLine($"file '{lastFrameFileName}'");
            }
        }

        return filePath;
    }

    void SaveAudioClip(AudioClip clip, string path)
    {
        // Create a temporary buffer to store the audio data
        var data = new float[clip.samples * clip.channels];

        // Get the data from the audio clip
        clip.GetData(data, 0);

        // Convert the data to WAV format
        var wavData = ConvertToWav(data, clip.samples, clip.channels, clip.frequency);

        // Write the WAV data to a file
        File.WriteAllBytes(path, wavData);
    }

    // Convert audio data to WAV format
    static byte[] ConvertToWav(float[] audioData, int samples, int channels, int frequency)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(memoryStream))
        {
            // Write WAV header
            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + samples * channels * 2);
            writer.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(frequency);
            writer.Write(frequency * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(samples * channels * 2);

            // Convert and write the audio data
            foreach (var sample in audioData)
                writer.Write((short)(sample * short.MaxValue));
            

            return memoryStream.ToArray();
        }
    }

    IEnumerator CombineWithFFmpeg(string frameDurationFilePath)
    {
        var ffmpegPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "bluedescriptor", "executables", "ffmpeg", "ffmpeg.exe");
        var tempVideoPath = Path.Combine(framesDirectory, "tempVideo.mp4");

        // First, create a video-only file
        yield return RunFFmpegProcess(ffmpegPath, $"-f concat -safe 0 -i \"{frameDurationFilePath}\" -vsync vfr -c:v libx264 -vf \"pad=ceil(iw/2)*2:ceil(ih/2)*2\" -pix_fmt yuv420p \"{tempVideoPath}\"");

        // Then, combine the video with the audio
        yield return RunFFmpegProcess(ffmpegPath, $"-i \"{tempVideoPath}\" -i \"{audioFilePath}\" -c:v copy -c:a aac -strict experimental \"{outputFilePath}\"");
    }

    IEnumerator RunFFmpegProcess(string ffmpegPath, string args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process ffmpegProcess = new Process { StartInfo = startInfo })
        {
            ffmpegProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    MelonLogger.Msg($"FFmpeg Output: {e.Data}");
                }
            };

            ffmpegProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    MelonLogger.Msg($"FFmpeg Error: {e.Data}");
                }
            };

            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();

            while (!ffmpegProcess.HasExited)
                yield return null;
            
        }
    }

    void Cleanup()
    {
        // Cleanup logic: Delete temporary files and directories
    }
}