
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SavWav
{
    const int HEADER_SIZE = 44;

    public bool Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
            filename += ".wav";

        var str = filename;
        Directory.CreateDirectory(Path.GetDirectoryName(str));
        var clipData = new SavWav.ClipData();
        clipData.samples = clip.samples;
        clipData.channels = clip.channels;
        var numArray = new float[clip.samples * clip.channels];
        clip.GetData(numArray, 0);
        clipData.samplesData = numArray;

        using (FileStream empty = CreateEmpty(str))
        {
            var memStream = new MemoryStream();
            ConvertAndWrite(memStream, clipData);
            memStream.WriteTo(empty);
            WriteHeader(empty, clip);
        }

        return true;
    }

    public AudioClip TrimSilence(AudioClip clip, float min)
    {
        var collection = new float[clip.samples];
        clip.GetData(collection, 0);
        return TrimSilence(new List<float>(collection), min, clip.channels, clip.frequency);
    }

    public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) => TrimSilence(samples, min, channels, hz, false, false);

    public AudioClip TrimSilence(
      List<float> samples,
      float min,
      int channels,
      int hz,
      bool _3D,
      bool stream)
    {
        var num = 0;

        while (num < samples.Count && (double)Mathf.Abs(samples[num]) <= (double)min)
            ++num;

        samples.RemoveRange(0, num);
        var index = samples.Count - 1;

        while (index > 0 && (double)Mathf.Abs(samples[index]) <= (double)min)
            --index;

        samples.RemoveRange(index, samples.Count - index);
        var audioClip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
        audioClip.SetData(samples.ToArray(), 0);
        return audioClip;
    }

    FileStream CreateEmpty(string filepath)
    {
        var empty = new FileStream(filepath, FileMode.Create);
        byte num = 0;

        for (int index = 0; index < 44; ++index)
            empty.WriteByte(num);

        return empty;
    }

    void ConvertAndWrite(MemoryStream memStream, SavWav.ClipData clipData)
    {
        var numArray1 = new float[clipData.samples * clipData.channels];
        var samplesData = clipData.samplesData;
        var src = new short[samplesData.Length];
        var numArray2 = new byte[samplesData.Length * 2];

        for (int index = 0; index < samplesData.Length; ++index)
            src[index] = (short)(samplesData[index] * (double)short.MaxValue);

        Buffer.BlockCopy(src, 0, numArray2, 0, numArray2.Length);
        memStream.Write(numArray2, 0, numArray2.Length);
    }

    void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var frequency = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;
        fileStream.Seek(0L, SeekOrigin.Begin);
        var bytes1 = Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(bytes1, 0, 4);
        var bytes2 = BitConverter.GetBytes(fileStream.Length - 8L);
        fileStream.Write(bytes2, 0, 4);
        var bytes3 = Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(bytes3, 0, 4);
        var bytes4 = Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(bytes4, 0, 4);
        var bytes5 = BitConverter.GetBytes(16);
        fileStream.Write(bytes5, 0, 4);
        var bytes6 = BitConverter.GetBytes((ushort)1);
        fileStream.Write(bytes6, 0, 2);
        var bytes7 = BitConverter.GetBytes(channels);
        fileStream.Write(bytes7, 0, 2);
        var bytes8 = BitConverter.GetBytes(frequency);
        fileStream.Write(bytes8, 0, 4);
        var bytes9 = BitConverter.GetBytes(frequency * channels * 2);
        fileStream.Write(bytes9, 0, 4);
        var num = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(num), 0, 2);
        var bytes10 = BitConverter.GetBytes((ushort)16);
        fileStream.Write(bytes10, 0, 2);
        var bytes11 = Encoding.UTF8.GetBytes("data");
        fileStream.Write(bytes11, 0, 4);
        var bytes12 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(bytes12, 0, 4);
        fileStream.Close();
    }

    private struct ClipData
    {

        public int samples;
        public int channels;
        public float[] samplesData;
    }
}