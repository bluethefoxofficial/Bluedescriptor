using Bluedescriptor_Rewritten.Classes;
using Bluedescriptor_Rewritten.UISYSTEM;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;


[assembly: MelonInfo(typeof(Bluedescriptor_Rewritten.Main), "Blue Descriptor", "1.0.0", "Bluethefox")]
namespace Bluedescriptor_Rewritten
{
   
    public class Main : MelonMod
    {
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
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "vrcnameplate", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "rainbowhud", false, null, null, false, false, null);

            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "playerlist", true, null, null, false, false, null);
            MelonPreferences.CreateEntry<int>("Bluedescriptor", "nameplate", 0, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "at_autopanic", true, null, null, false, false, null);
            MelonPreferences.CreateEntry<int>("Bluedescriptor", "at_autopanic_fps", 10, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "memorycleanup", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "joinsound", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "networkeula", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<string>("Bluedescriptor", "appliedtheme", "", null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "alarm", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "experimental-features", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<bool>("Bluedescriptor", "uisoundfx", false, null, null, false, false, null);
            MelonPreferences.CreateEntry<int>("Bluedescriptor", "alarmhour", 0, null, null, false, false, null);
            MelonPreferences.CreateEntry<int>("Bluedescriptor", "alarmminute", 0, null, null, false, false, null);
            MelonPreferences.CreateEntry<float>("Bluedescriptor", "nameplate-speed", 5f, null, null, false, false, null);
            EmbedExtract.ExtractResource("Bluedescriptor_Rewritten.res.ffmpeg.zip", Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.zip");
            var flag = ZipExtractor.ExtractZip(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg\\ffmpeg.zip", Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\executables\\ffmpeg");

            if (flag)
            {
                MelonLogger.Warning("it worked");
            }
            else
            {
                MelonLogger.Error("no ffmpeg :(");
            }

            var flag2 = !Directory.Exists(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");

            if (flag2)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");
            }

            new FontLogger().LogAllFonts();
            new FontLogger().LogAllTMPFontsAndMaterials();
            new Icons().iconsinit();
            uisystem.uiinit();
            new MemoryCleaner().ClearRAM();
        }

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
            var gameObject = new GameObject("CameraCuller");
            cullingScript = gameObject.AddComponent<AdvancedCameraRaycastCulling>();
            cullingScript.StartCoroutine(cullingScript.Start());
            cullingScript.StartCoroutine(cullingScript.CheckForCameraMovement());
            cullingScript.rescancam();
            var flag = buildIndex == 1;

            if (flag)
            {
                var gameObject2 = new GameObject("AudioManagerObject");
                var audioManager = gameObject2.AddComponent<AudioManager>();
                var entryValue = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "uisounds");
                var flag2 = entryValue;

                if (flag2)
                {
                    audioManager.PlayAudio("welcome.wav");
                }

                var flag3 = entryValue;

                if (flag3)
                {
                    audioManager.PlayAudio("music.wav");
                }

                uisystem.musicplayerinit();
            }

            new MemoryCleaner().ClearRAM();
        }

        UI uisystem = new UI();

        AdvancedCameraRaycastCulling cullingScript;
    }
}