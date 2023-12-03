
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
  private bool isRecording;
  private string framesDirectory;
  private string audioFilePath;
  private string outputFilePath;
  private Process ffmpegProcess;
  private int frameCount;
  private float startTime;
  private AudioSource audioSource;
  private AudioClip recordingClip;
  private List<float> frameratesOverTime = new List<float>();

  public ScreenRecorder()
  {
    string str1 = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    string str2 = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "bluedescriptor", "recordings");
    Directory.CreateDirectory(str2);
    framesDirectory = Path.Combine(str2, UnityEngine.Random.Range(100000, 999999).ToString());
    Directory.CreateDirectory(framesDirectory);
    audioFilePath = Path.Combine(str2, str1 + ".wav");
    outputFilePath = Path.Combine(str2, str1 + ".mp4");
  }

  public IEnumerator StartRecording()
  {
    if (isRecording)
    {
      MelonLogger.Msg("Recording is already in progress.");
    }
    else
    {
      CohtmlHud.Instance.ViewDropTextImmediate("<color=blue>[BD]</color>", "Blue Descriptor", "recording started");
      isRecording = true;
      startTime = Time.time;
      audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
      audioSource.loop = false;
      audioSource.playOnAwake = false;
      audioSource.clip = Microphone.Start(null, true, 10, 44100);
      while (Microphone.GetPosition(null) <= 0)
        yield return  null;
      audioSource.Play();
      MelonLogger.Msg("Recording started.");
      while (isRecording)
        yield return  CaptureFrame();
    }
  }

  private IEnumerator CaptureFrame()
  {
    frameratesOverTime.Add(1f / Time.deltaTime);
    Texture2D currentFrame = ScreenCapture.CaptureScreenshotAsTexture();
    string path = Path.Combine(framesDirectory, frameCount.ToString("D6") + ".jpg");
    ++frameCount;
    yield return  null;
    ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
    {
      File.WriteAllBytes(path, ImageConversion.EncodeToJPG(currentFrame));
      GameObject.Destroy( currentFrame);
    }));
  }

  public IEnumerator StopRecording()
  {
    if (!isRecording)
    {
      MelonLogger.Msg("No recording in progress.");
    }
    else
    {
      CohtmlHud.Instance
          .ViewDropTextImmediate("<color=blue>[BD]</color>", "Blue Descriptor", "recording being processed.");
      isRecording = false;
      float recordedTime = Time.time - startTime;
      audioSource.Stop();
      Microphone.End(null);
      SaveAudioClip(audioSource.clip, audioFilePath);
     GameObject.Destroy(audioSource.gameObject);
      string frameDurationFilePath = WriteFrameDurationsToFile(frameratesOverTime);
      yield return  CombineWithFFmpeg(frameDurationFilePath);
      MelonLogger.Msg("Recording stopped.");
    }
  }

  private string WriteFrameDurationsToFile(List<float> frameRates)
  {
    string path = Path.Combine(framesDirectory, "frameDurations.txt");
    using (StreamWriter streamWriter = new StreamWriter(path))
    {
      for (int index = 0; index < frameRates.Count; ++index)
      {
        float num = 1f / frameRates[index];
        string str = Path.Combine(framesDirectory, string.Format("{0:D6}.jpg",  index));
        streamWriter.WriteLine("file '" + str + "'");
        streamWriter.WriteLine(string.Format("duration {0}",  num));
      }
      if (frameRates.Count > 0)
      {
        string str = Path.Combine(framesDirectory, string.Format("{0:D6}.jpg",  (frameRates.Count - 1)));
        streamWriter.WriteLine("file '" + str + "'");
      }
    }
    return path;
  }

  private void SaveAudioClip(AudioClip clip, string path)
  {
    float[] audioData = new float[clip.samples * clip.channels];
    clip.GetData(audioData, 0);
    byte[] wav = ScreenRecorder.ConvertToWav(audioData, clip.samples, clip.channels, clip.frequency);
    File.WriteAllBytes(path, wav);
  }

  private static byte[] ConvertToWav(float[] audioData, int samples, int channels, int frequency)
  {
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter binaryWriter = new BinaryWriter(output))
      {
        binaryWriter.Write(new char[4]{ 'R', 'I', 'F', 'F' });
        binaryWriter.Write(36 + samples * channels * 2);
        binaryWriter.Write(new char[8]
        {
          'W',
          'A',
          'V',
          'E',
          'f',
          'm',
          't',
          ' '
        });
        binaryWriter.Write(16);
        binaryWriter.Write((short) 1);
        binaryWriter.Write((short) channels);
        binaryWriter.Write(frequency);
        binaryWriter.Write(frequency * channels * 2);
        binaryWriter.Write((short) (channels * 2));
        binaryWriter.Write((short) 16);
        binaryWriter.Write(new char[4]{ 'd', 'a', 't', 'a' });
        binaryWriter.Write(samples * channels * 2);
        foreach (float num in audioData)
          binaryWriter.Write((short) ((double) num * short.MaxValue));
        return output.ToArray();
      }
    }
  }

  private IEnumerator CombineWithFFmpeg(string frameDurationFilePath)
  {
    string ffmpegPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "bluedescriptor", "executables", "ffmpeg", "ffmpeg.exe");
    string tempVideoPath = Path.Combine(framesDirectory, "tempVideo.mp4");
    yield return  RunFFmpegProcess(ffmpegPath, "-f concat -safe 0 -i \"" + frameDurationFilePath + "\" -vsync vfr -c:v libx264 -vf \"pad=ceil(iw/2)*2:ceil(ih/2)*2\" -pix_fmt yuv420p \"" + tempVideoPath + "\"");
    yield return  RunFFmpegProcess(ffmpegPath, "-i \"" + tempVideoPath + "\" -i \"" + audioFilePath + "\" -c:v copy -c:a aac -strict experimental \"" + outputFilePath + "\"");
  }

  private IEnumerator RunFFmpegProcess(string ffmpegPath, string args)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo()
    {
      FileName = ffmpegPath,
      Arguments = args,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true
    };
    using (Process ffmpegProcess = new Process()
    {
      StartInfo = startInfo
    })
    {
      ffmpegProcess.OutputDataReceived += (sender, e) =>
      {
          if (string.IsNullOrEmpty(e.Data))
              return;
          MelonLogger.Msg("FFmpeg Output: " + e.Data);
      };
      ffmpegProcess.ErrorDataReceived += (sender, e) =>
      {
          if (string.IsNullOrEmpty(e.Data))
              return;
          MelonLogger.Msg("FFmpeg Error: " + e.Data);
      };
      ffmpegProcess.Start();
      ffmpegProcess.BeginOutputReadLine();
      ffmpegProcess.BeginErrorReadLine();
      while (!ffmpegProcess.HasExited)
        yield return  null;
    }
  }

  private void Cleanup()
  {
  }
}
