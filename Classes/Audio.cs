using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class Audio
    {
        public AudioSource audioSource = new AudioSource();

        public void Audioprep(string res, string filename)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var str = Path.Combine(Path.GetDirectoryName(new Uri(executingAssembly.CodeBase).LocalPath), "bluedescriptor");
            Directory.CreateDirectory(str);
            var path = Path.Combine(str, filename + ".wav");

            if (File.Exists(path))
                return;

            using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(res))
            {
                if (manifestResourceStream == null)
                {
                    MelonLogger.Msg("Resource not found: " + res);
                }
                else
                {
                    using (FileStream destination = File.Create(path))
                        manifestResourceStream.CopyTo(destination);

                    MelonLogger.Msg("COMPLETE");
                }
            }
        }

        public async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, (AudioType)20))
            {
                uwr.SendWebRequest();

                try
                {
                    while (!uwr.isDone)
                        await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError)
                        MelonLogger.Log(uwr.error ?? "");
                    else
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg(ex.Message + ", " + ex.StackTrace);
                }
            }

            var audioClip = clip;
            clip = null;
            return audioClip;
        }

        public async Task<AudioClip> LoadClipFromResources(string path)
        {
            AudioClip clip = null;

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, (AudioType)20))
            {
                uwr.SendWebRequest();

                try
                {
                    while (!uwr.isDone)
                        await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError)
                        MelonLogger.Log(uwr.error ?? "");
                    else
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg(ex.Message + ", " + ex.StackTrace);
                }
            }

            var audioClip = clip;
            clip = null;
            return audioClip;
        }

        public async void PlayAudio(string path)
        {
            var clip = await LoadClipFromResources(path);
            audioSource.clip = clip;
            audioSource.Play();
            clip = null;
        }
    }
}