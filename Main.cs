using Bluedescriptor_Rewritten.Classes;
using Bluedescriptor_Rewritten.UISYSTEM;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(Bluedescriptor_Rewritten.Main), "Blue Descriptor", "2.1.5", "Bluethefox")]

namespace Bluedescriptor_Rewritten
{
    public class Main : MelonMod
    {
        UI uisystem = new UI();

        public Main()
        {
        }
        public override void OnFixedUpdate() => new AntiToxin().MonitorFPS();

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("done");

            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.join.wav", "join");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.leave.wav", "leave");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.welcome.wav", "welcome");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.uimusic.wav", "music");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.uiopen.wav", "open");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.close.wav", "close");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.autherror.wav", "conerror");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.noti.wav", "noti");

            MelonPreferences.CreateCategory("Bluedescriptor", "Blue Descriptor (client)");
            MelonPreferences.CreateEntry("Bluedescriptor", "vrcnameplate", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "rainbowhud", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "uisounds", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "playerlist", true);
            MelonPreferences.CreateEntry("Bluedescriptor", "nameplate", 0);
            // Antitoxin
            MelonPreferences.CreateEntry("Bluedescriptor", "at_autopanic", true);
            MelonPreferences.CreateEntry("Bluedescriptor", "at_autopanic_fps", 10);
            // memory management
            MelonPreferences.CreateEntry("Bluedescriptor", "memorycleanup", false);
            // player join sound
            MelonPreferences.CreateEntry("Bluedescriptor", "joinsound", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "networkeula", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "appliedtheme", "");
            // alarm
            MelonPreferences.CreateEntry("Bluedescriptor", "alarm", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "alarmhour", 0);
            MelonPreferences.CreateEntry("Bluedescriptor", "alarmminute", 0);

            // nameplate settings
            MelonPreferences.CreateEntry("Bluedescriptor", "nameplate-speed", 5.0f);

            // internals
            EmbedExtract.ExtractResource("Bluedescriptor_Rewritten.res.ffmpeg.zip", Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.zip");

            if (ZipExtractor.ExtractZip(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.zip", Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg"))
            {
                MelonLogger.Warning("it worked");
            }
            else
            {
                MelonLogger.Error("no ffmpeg :(");
            }

            if (!Directory.Exists(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");
            }

            new FontLogger().LogAllFonts();
            new FontLogger().LogAllTMPFontsAndMaterials();
            // icons
            new UISYSTEM.Icons().iconsinit();
            uisystem.uiinit();

            new MemoryCleaner().ClearRAM();
        }

        AdvancedCameraRaycastCulling cullingScript;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            try
            {
                cullingScript.reset();
                new MemoryCleaner().ClearRAM();
            }
            catch
            {
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            new MemoryCleaner().ClearRAM();

         

            var cameraCullerObject = new GameObject("CameraCuller");

            // Attach the AdvancedCameraRaycastCulling script to it
            cullingScript = cameraCullerObject.AddComponent<AdvancedCameraRaycastCulling>();

            // Assuming that the Start method is also a coroutine now
            cullingScript.StartCoroutine(cullingScript.Start());

            // Assuming you have a method that starts the check for camera movement coroutine
            cullingScript.StartCoroutine(cullingScript.CheckForCameraMovement());
            cullingScript.rescancam();

            if (buildIndex == 1)
            {
                // Create a new GameObject and attach the AudioManager
                var audioManagerObject = new GameObject("AudioManagerObject");
                var audioManager = audioManagerObject.AddComponent<AudioManager>();
                bool uisounds = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "uisounds");
                if (uisounds)  audioManager.PlayAudio("welcome.wav");
                 if(uisounds)  audioManager.PlayAudio("music.wav");

                uisystem.musicplayerinit();
            }

            new MemoryCleaner().ClearRAM();
        }
    }
}