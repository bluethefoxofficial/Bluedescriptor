using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class SavWav
{
	// Token: 0x0600002D RID: 45 RVA: 0x00002AA0 File Offset: 0x00000CA0
	public bool Save(string filename, AudioClip clip)
	{
		bool flag = !filename.ToLower().EndsWith(".wav");
		if (flag)
		{
			filename += ".wav";
		}
		string text = filename;
		Directory.CreateDirectory(Path.GetDirectoryName(text));
		SavWav.ClipData clipData = default(SavWav.ClipData);
		clipData.samples = clip.samples;
		clipData.channels = clip.channels;
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		clipData.samplesData = array;
		using (FileStream fileStream = this.CreateEmpty(text))
		{
			MemoryStream memoryStream = new MemoryStream();
			this.ConvertAndWrite(memoryStream, clipData);
			memoryStream.WriteTo(fileStream);
			this.WriteHeader(fileStream, clip);
		}
		return true;
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002B80 File Offset: 0x00000D80
	public AudioClip TrimSilence(AudioClip clip, float min)
	{
		float[] array = new float[clip.samples];
		clip.GetData(array, 0);
		return this.TrimSilence(new List<float>(array), min, clip.channels, clip.frequency);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002BC0 File Offset: 0x00000DC0
	public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
	{
		return this.TrimSilence(samples, min, channels, hz, false, false);
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002BE0 File Offset: 0x00000DE0
	public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
	{
		int i;
		for (i = 0; i < samples.Count; i++)
		{
			bool flag = Mathf.Abs(samples[i]) > min;
			if (flag)
			{
				break;
			}
		}
		samples.RemoveRange(0, i);
		for (i = samples.Count - 1; i > 0; i--)
		{
			bool flag2 = Mathf.Abs(samples[i]) > min;
			if (flag2)
			{
				break;
			}
		}
		samples.RemoveRange(i, samples.Count - i);
		AudioClip audioClip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
		audioClip.SetData(samples.ToArray(), 0);
		return audioClip;
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002C94 File Offset: 0x00000E94
	private FileStream CreateEmpty(string filepath)
	{
		FileStream fileStream = new FileStream(filepath, FileMode.Create);
		byte value = 0;
		for (int i = 0; i < 44; i++)
		{
			fileStream.WriteByte(value);
		}
		return fileStream;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002CCC File Offset: 0x00000ECC
	private void ConvertAndWrite(MemoryStream memStream, SavWav.ClipData clipData)
	{
		float[] array = new float[clipData.samples * clipData.channels];
		array = clipData.samplesData;
		short[] array2 = new short[array.Length];
		byte[] array3 = new byte[array.Length * 2];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = (short)(array[i] * 32767f);
		}
		Buffer.BlockCopy(array2, 0, array3, 0, array3.Length);
		memStream.Write(array3, 0, array3.Length);
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002D44 File Offset: 0x00000F44
	private void WriteHeader(FileStream fileStream, AudioClip clip)
	{
		int frequency = clip.frequency;
		int channels = clip.channels;
		int samples = clip.samples;
		fileStream.Seek(0L, SeekOrigin.Begin);
		byte[] bytes = Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(bytes, 0, 4);
		byte[] bytes2 = BitConverter.GetBytes(fileStream.Length - 8L);
		fileStream.Write(bytes2, 0, 4);
		byte[] bytes3 = Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(bytes3, 0, 4);
		byte[] bytes4 = Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(bytes4, 0, 4);
		byte[] bytes5 = BitConverter.GetBytes(16);
		fileStream.Write(bytes5, 0, 4);
		ushort value = 1;
		byte[] bytes6 = BitConverter.GetBytes(value);
		fileStream.Write(bytes6, 0, 2);
		byte[] bytes7 = BitConverter.GetBytes(channels);
		fileStream.Write(bytes7, 0, 2);
		byte[] bytes8 = BitConverter.GetBytes(frequency);
		fileStream.Write(bytes8, 0, 4);
		byte[] bytes9 = BitConverter.GetBytes(frequency * channels * 2);
		fileStream.Write(bytes9, 0, 4);
		ushort value2 = (ushort)(channels * 2);
		fileStream.Write(BitConverter.GetBytes(value2), 0, 2);
		ushort value3 = 16;
		byte[] bytes10 = BitConverter.GetBytes(value3);
		fileStream.Write(bytes10, 0, 2);
		byte[] bytes11 = Encoding.UTF8.GetBytes("data");
		fileStream.Write(bytes11, 0, 4);
		byte[] bytes12 = BitConverter.GetBytes(samples * channels * 2);
		fileStream.Write(bytes12, 0, 4);
		fileStream.Close();
	}

	// Token: 0x0400000B RID: 11
	private const int HEADER_SIZE = 44;

	// Token: 0x02000028 RID: 40
	private struct ClipData
	{
		// Token: 0x0400006A RID: 106
		public int samples;

		// Token: 0x0400006B RID: 107
		public int channels;

		// Token: 0x0400006C RID: 108
		public float[] samplesData;
	}
}
