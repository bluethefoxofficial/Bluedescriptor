using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavWav
{
    const int HEADER_SIZE = 44;
    struct ClipData
    {
        public int samples;
        public int channels;
        public float[] samplesData;
    }

    public bool Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        var filepath = filename;

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        var clipdata = new ClipData();
        clipdata.samples = clip.samples;
        clipdata.channels = clip.channels;
        var dataFloat = new float[clip.samples * clip.channels];
        clip.GetData(dataFloat, 0);
        clipdata.samplesData = dataFloat;

        using (var fileStream = CreateEmpty(filepath))
        {
            var memstrm = new MemoryStream();
            ConvertAndWrite(memstrm, clipdata);
            memstrm.WriteTo(fileStream);
            WriteHeader(fileStream, clip);
        }

        return true; // TODO: return false if there's a failure saving the file
    }

    public AudioClip TrimSilence(AudioClip clip, float min)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
    {
        return TrimSilence(samples, min, channels, hz, false, false);
    }

    public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
    {
        int i;

        for (i = 0; i < samples.Count; i++)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(i, samples.Count - i);

        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);

        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        var emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
            fileStream.WriteByte(emptyByte);

        return fileStream;
    }

    void ConvertAndWrite(MemoryStream memStream, ClipData clipData)
    {
        var samples = new float[clipData.samples * clipData.channels];

        samples = clipData.samplesData;

        var intData = new short[samples.Length];

        var bytesData = new byte[samples.Length * 2];

        const float rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
            intData[i] = (short)(samples[i] * rescaleFactor);
        // MelonLogger.Log (samples [i]);

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        memStream.Write(bytesData, 0, bytesData.Length);
    }

    void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        var chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        var subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        ushort two = 2;
        ushort one = 1;

        var audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        var numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        var sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        var byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);

        var blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        ushort bps = 16;
        var bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        var datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        var subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);

        fileStream.Close();
    }
}