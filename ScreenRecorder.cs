using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using ABI_RC.Core.UI;
using MelonLoader;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class ScreenRecorder
{
	// Token: 0x06000035 RID: 53 RVA: 0x00002EBC File Offset: 0x000010BC
	public ScreenRecorder()
	{
		string str = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		string text = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "bluedescriptor", "recordings");
		Directory.CreateDirectory(text);
		this.framesDirectory = Path.Combine(text, Random.Range(100000, 999999).ToString());
		Directory.CreateDirectory(this.framesDirectory);
		this.audioFilePath = Path.Combine(text, str + ".wav");
		this.outputFilePath = Path.Combine(text, str + ".mp4");
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002F7C File Offset: 0x0000117C
	public IEnumerator StartRecording()
	{
		bool flag = this.isRecording;
		if (flag)
		{
			MelonLogger.Msg("Recording is already in progress.");
			yield break;
		}
		CohtmlHud.Instance.ViewDropTextImmediate("<color=blue>[BD]</color>", "Blue Descriptor", "recording started");
		this.isRecording = true;
		this.startTime = Time.time;
		this.audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
		this.audioSource.loop = false;
		this.audioSource.playOnAwake = false;
		this.audioSource.clip = Microphone.Start(null, true, 10, 44100);
		while (Microphone.GetPosition(null) <= 0)
		{
			yield return null;
		}
		this.audioSource.Play();
		MelonLogger.Msg("Recording started.");
		while (this.isRecording)
		{
			yield return this.CaptureFrame();
		}
		yield break;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002F8B File Offset: 0x0000118B
	private IEnumerator CaptureFrame()
	{
		this.frameratesOverTime.Add(1f / Time.deltaTime);
		Texture2D currentFrame = ScreenCapture.CaptureScreenshotAsTexture();
		string path = Path.Combine(this.framesDirectory, this.frameCount.ToString("D6") + ".jpg");
		this.frameCount++;
		yield return null;
		ThreadPool.QueueUserWorkItem(delegate(object _)
		{
			byte[] bytes = ImageConversion.EncodeToJPG(currentFrame);
			File.WriteAllBytes(path, bytes);
			Object.Destroy(currentFrame);
		});
		yield break;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002F9A File Offset: 0x0000119A
	public IEnumerator StopRecording()
	{
		bool flag = !this.isRecording;
		if (flag)
		{
			MelonLogger.Msg("No recording in progress.");
			yield break;
		}
		CohtmlHud.Instance.ViewDropTextImmediate("<color=blue>[BD]</color>", "Blue Descriptor", "recording being processed.");
		this.isRecording = false;
		float recordedTime = Time.time - this.startTime;
		this.audioSource.Stop();
		Microphone.End(null);
		this.SaveAudioClip(this.audioSource.clip, this.audioFilePath);
		Object.Destroy(this.audioSource.gameObject);
		string frameDurationFilePath = this.WriteFrameDurationsToFile(this.frameratesOverTime);
		yield return this.CombineWithFFmpeg(frameDurationFilePath);
		MelonLogger.Msg("Recording stopped.");
		yield break;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002FAC File Offset: 0x000011AC
	private string WriteFrameDurationsToFile(List<float> frameRates)
	{
		string text = Path.Combine(this.framesDirectory, "frameDurations.txt");
		using (StreamWriter streamWriter = new StreamWriter(text))
		{
			for (int i = 0; i < frameRates.Count; i++)
			{
				float num = 1f / frameRates[i];
				string str = Path.Combine(this.framesDirectory, string.Format("{0:D6}.jpg", i));
				streamWriter.WriteLine("file '" + str + "'");
				streamWriter.WriteLine(string.Format("duration {0}", num));
			}
			bool flag = frameRates.Count > 0;
			if (flag)
			{
				string str2 = Path.Combine(this.framesDirectory, string.Format("{0:D6}.jpg", frameRates.Count - 1));
				streamWriter.WriteLine("file '" + str2 + "'");
			}
		}
		return text;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x000030B8 File Offset: 0x000012B8
	private void SaveAudioClip(AudioClip clip, string path)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		byte[] bytes = ScreenRecorder.ConvertToWav(array, clip.samples, clip.channels, clip.frequency);
		File.WriteAllBytes(path, bytes);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003104 File Offset: 0x00001304
	private static byte[] ConvertToWav(float[] audioData, int samples, int channels, int frequency)
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(new char[]
				{
					'R',
					'I',
					'F',
					'F'
				});
				binaryWriter.Write(36 + samples * channels * 2);
				binaryWriter.Write(new char[]
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
				binaryWriter.Write(1);
				binaryWriter.Write((short)channels);
				binaryWriter.Write(frequency);
				binaryWriter.Write(frequency * channels * 2);
				binaryWriter.Write((short)(channels * 2));
				binaryWriter.Write(16);
				binaryWriter.Write(new char[]
				{
					'd',
					'a',
					't',
					'a'
				});
				binaryWriter.Write(samples * channels * 2);
				foreach (float num in audioData)
				{
					binaryWriter.Write((short)(num * 32767f));
				}
				result = memoryStream.ToArray();
			}
		}
		return result;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x0000322C File Offset: 0x0000142C
	private IEnumerator CombineWithFFmpeg(string frameDurationFilePath)
	{
		string ffmpegPath = Path.Combine(new string[]
		{
			Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
			"bluedescriptor",
			"executables",
			"ffmpeg",
			"ffmpeg.exe"
		});
		string tempVideoPath = Path.Combine(this.framesDirectory, "tempVideo.mp4");
		yield return this.RunFFmpegProcess(ffmpegPath, string.Concat(new string[]
		{
			"-f concat -safe 0 -i \"",
			frameDurationFilePath,
			"\" -vsync vfr -c:v libx264 -vf \"pad=ceil(iw/2)*2:ceil(ih/2)*2\" -pix_fmt yuv420p \"",
			tempVideoPath,
			"\""
		}));
		yield return this.RunFFmpegProcess(ffmpegPath, string.Concat(new string[]
		{
			"-i \"",
			tempVideoPath,
			"\" -i \"",
			this.audioFilePath,
			"\" -c:v copy -c:a aac -strict experimental \"",
			this.outputFilePath,
			"\""
		}));
		yield break;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003242 File Offset: 0x00001442
	private IEnumerator RunFFmpegProcess(string ffmpegPath, string args)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = ffmpegPath,
			Arguments = args,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		};
		using (Process ffmpegProcess = new Process
		{
			StartInfo = startInfo
		})
		{
			ffmpegProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				bool flag = !string.IsNullOrEmpty(e.Data);
				if (flag)
				{
					MelonLogger.Msg("FFmpeg Output: " + e.Data);
				}
			};
			ffmpegProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				bool flag = !string.IsNullOrEmpty(e.Data);
				if (flag)
				{
					MelonLogger.Msg("FFmpeg Error: " + e.Data);
				}
			};
			ffmpegProcess.Start();
			ffmpegProcess.BeginOutputReadLine();
			ffmpegProcess.BeginErrorReadLine();
			while (!ffmpegProcess.HasExited)
			{
				yield return null;
			}
		}
		Process ffmpegProcess = null;
		yield break;
		yield break;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x0000325F File Offset: 0x0000145F
	private void Cleanup()
	{
	}

	// Token: 0x0400000C RID: 12
	private bool isRecording;

	// Token: 0x0400000D RID: 13
	private string framesDirectory;

	// Token: 0x0400000E RID: 14
	private string audioFilePath;

	// Token: 0x0400000F RID: 15
	private string outputFilePath;

	// Token: 0x04000010 RID: 16
	private Process ffmpegProcess;

	// Token: 0x04000011 RID: 17
	private int frameCount;

	// Token: 0x04000012 RID: 18
	private float startTime;

	// Token: 0x04000013 RID: 19
	private AudioSource audioSource;

	// Token: 0x04000014 RID: 20
	private AudioClip recordingClip;

	// Token: 0x04000015 RID: 21
	private List<float> frameratesOverTime = new List<float>();
}
