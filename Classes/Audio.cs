using MelonLoader;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class Audio
    {
        /*
         * 
         * this is a work in progress no where near ready for production
         * 
         */
        public void Audioprep(string res, string filename)
        {
            // Get the current assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the directory of the DLL file
            string assemblyDirectory = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);

            // Get the stream of the embedded resource
            using (Stream resourceStream = assembly.GetManifestResourceStream(res))
            {
                if (resourceStream == null)
                {
                    // Resource not found
                    MelonLogger.Msg("Resource not found: " + res);
                    return;
                }
                // Create a directory to save the resource if it doesn't exist
                string saveDirectory = Path.Combine(assemblyDirectory, "bluedescriptor");
                Directory.CreateDirectory(saveDirectory);

                // Create a file stream to save the resource
                string filePath = Path.Combine(saveDirectory, filename + ".ogg");
                using (FileStream fileStream = File.Create(filePath))
                {
                    // Copy the resource stream to the file stream
                    resourceStream.CopyTo(fileStream);
                }

                MelonLogger.Msg("COMPLETE");
            }
        }
        public AudioSource audioSource;

        // Function to load audio data from a file

       public async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                uwr.SendWebRequest();
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    MelonLogger.Msg($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }
    }
 
}
