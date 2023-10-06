using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ABI_RC.Core.UI;
using MelonLoader;  // For MelonLogger
using UnityEngine;

public class ScreenRecorder
{
    private Process ffmpegProcess;
    private string outputFilePath;
    private int screenWidth = Screen.width;
    private int screenHeight = Screen.height;

    public ScreenRecorder()
    {
        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string directoryPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\";
        Directory.CreateDirectory(directoryPath);  // Ensure the directory exists
        outputFilePath = Path.Combine(directoryPath, currentDateTime + ".mp4");
    }

    // Start recording
    public void StartRecording()
    {
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            MelonLogger.Msg("Recording is already in progress.");
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording in progress, stop to stop recording.");
            return;
        }

        string ffmpegPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.exe";  // Update with your FFmpeg binary path
                                                                                                                                                                       // Using gdigrab for video and dshow for audio
        string args = $"-f gdigrab -framerate 60 -i title=ChilloutVR \"{outputFilePath}\"";

        ffmpegProcess = new Process
        {
            StartInfo = {
            FileName = ffmpegPath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,  // Redirect the standard output
            RedirectStandardError = true,   // Redirect the standard error
            RedirectStandardInput = true,
            CreateNoWindow = false
        }
        };

        // Attach event handlers
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
                if (e.Data.ToLower().Contains("error"))
                {
                    MelonLogger.Msg($"FFmpeg Error: {e.Data}");
                }
            }
        };

        ffmpegProcess.Start();

        // Begin reading from the redirected outputs
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();

        CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording started");
    }


    // Stop recording
    public void StopRecording()
    {
        if (ffmpegProcess == null || ffmpegProcess.HasExited)
        {
            MelonLogger.Msg("No recording in progress.");
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "no recording in progress");

            return;
        }
        CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor", "recording stopped");
        // Send 'q' to gracefully stop FFmpeg recording
        StreamWriter inputWriter = ffmpegProcess.StandardInput;
        inputWriter.WriteLine("q");
        ffmpegProcess.WaitForExit();
        ffmpegProcess.Close();
        inputWriter.Close();
        ffmpegProcess = null;
      
    }
}
