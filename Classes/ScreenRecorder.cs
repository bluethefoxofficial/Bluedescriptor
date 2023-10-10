using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using ABI_RC.Core.UI;
using MelonLoader;
using UnityEngine;

public class ScreenRecorder
{
    private bool isRecording = false;
    private string framesDirectory;
    private string audioFilePath;
    private string outputFilePath;
    public Process ffmpegproc;
    private int frameCount = 0;
    private AudioClip microphoneClip;

    public ScreenRecorder()
    {
        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string directoryPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\";
        Directory.CreateDirectory(directoryPath);

        framesDirectory = Path.Combine(directoryPath, UnityEngine.Random.Range(100000, 999999).ToString());
        Directory.CreateDirectory(framesDirectory);

        audioFilePath = Path.Combine(directoryPath, currentDateTime + ".wav");
        outputFilePath = Path.Combine(directoryPath, currentDateTime + ".mp4");
    }

    public IEnumerator StartRecording()
    {
        switch(isRecording){
            case true:
            MelonLogger.Msg("Recording is already in progress.");
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording in progress, stop to stop recording.");
            yield break;
        }

        isRecording = true;
        if(framesDirectory != null)
        {
            Directory.CreateDirectory(framesDirectory);
        }
        microphoneClip = Microphone.Start(null, true, 10, 44100);
        CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording started");

        while (isRecording)
        {
            yield return CaptureFrame();
        }
    }

    private IEnumerator CaptureFrame()
    {
        Texture2D currentFrame = ScreenCapture.CaptureScreenshotAsTexture();
        string path = Path.Combine(framesDirectory, frameCount + ".jpg");
        frameCount++;

        // Yield for one frame before encoding and writing to disk
        yield return null;

        ThreadPool.QueueUserWorkItem(_ =>
        {
            byte[] bytes = currentFrame.EncodeToJPG();
            File.WriteAllBytes(path, bytes);
            UnityEngine.Object.Destroy(currentFrame);
        });
    }


    public IEnumerator StopRecording()
    {
        if (!isRecording)
        {
            MelonLogger.Msg("No recording in progress.");
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "no recording in progress");
            yield break;
        }

        isRecording = false;
        Microphone.End(null);

        SavWav saveWavUtility = new SavWav();
        saveWavUtility.Save(audioFilePath, microphoneClip);

        yield return CombineWithFFmpeg();
    }

    private IEnumerator CombineWithFFmpeg()
    {
        string ffmpegPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.exe";
        string args = $"-thread_queue_size 512 -framerate 60 -i \"{framesDirectory}/%d.jpg\" -thread_queue_size 512 -i \"{audioFilePath}\" -c:v libx264 -vf \"pad=ceil(iw/2)*2:ceil(ih/2)*2\" -pix_fmt yuv420p \"{outputFilePath}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (ffmpegproc = new Process { StartInfo = startInfo })
        {
            ffmpegproc.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    MelonLogger.Msg($"FFmpeg Output: {e.Data}");
                }
            };

            ffmpegproc.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    MelonLogger.Msg($"FFmpeg Error: {e.Data}");
                }
            };

            ffmpegproc.Start();
            ffmpegproc.BeginOutputReadLine();
            ffmpegproc.BeginErrorReadLine();

            while (!ffmpegproc.HasExited)
            {
                yield return null;
            }
        }

        //   Directory.Delete(framesDirectory, true);
        //  File.Delete(audioFilePath);
        string directoryPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\";

        frameCount = 0;
        framesDirectory = Path.Combine(directoryPath, UnityEngine.Random.Range(100000, 999999).ToString());
        CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording stopped");
    }
}
