﻿
using Bluedescriptor_Rewritten.Classes;
using Bluedescriptor_Rewritten.UISYSTEM;
using BTKUILib.UIObjects;
using MelonLoader;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Harmony;
using UnityEngine.XR;
using HarmonyLib;
using System;

[assembly: MelonInfo(typeof(Bluedescriptor_Rewritten.Main), "Blue Descriptor", "2.1.5", "Bluethefox")]

namespace Bluedescriptor_Rewritten
{   

    public class Main : MelonMod
    {
        
        UI uisystem = new UI();

    
        public Main()
        {
         }
        public override void OnFixedUpdate()
        {
           new AntiToxin().MonitorFPS();
        }

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("done");
      
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.join.wav","join");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.leave.wav","leave");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.welcome.wav","welcome");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.uimusic.wav","music");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.uiopen.wav","open");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.close.wav","close");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.autherror.wav","conerror");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.noti.wav","noti");

            MelonPreferences.CreateCategory("Bluedescriptor","Blue Descriptor (client)");
            MelonPreferences.CreateEntry("Bluedescriptor", "vrcnameplate", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "rainbowhud", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "nameplate", 0);
            //list of rewards
            MelonPreferences.CreateEntry("Bluedescriptor", "vrshit",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "YOUMETBLUE",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "blueleftyourlobby",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "kannauwu",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "shadowthehorny",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "shadowthehornycummykeyboard",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "gangmonkey",false);
            //memory management
            MelonPreferences.CreateEntry("Bluedescriptor", "memorycleanup",false);
            //player join sound
            MelonPreferences.CreateEntry("Bluedescriptor", "joinsound",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "networkeula",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "quickmenuskin","");
            //alarm
            MelonPreferences.CreateEntry("Bluedescriptor", "alarm",false);
            MelonPreferences.CreateEntry("Bluedescriptor", "alarmhour",0);
            MelonPreferences.CreateEntry("Bluedescriptor", "alarmminute",0);

            //nameplate settings
            MelonPreferences.CreateEntry("Bluedescriptor", "nameplate-speed", 0.5f);

            //internals
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
            //icons
            new UISYSTEM.Icons().iconsinit();
            uisystem.uiinit();

            //MOD NETWORK


        }
     


        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
   
            new UIfunctions().quickmenyinitstyler(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin"));
           
            MelonLogger.Msg("Buld index: " + buildIndex + " | Scene name: " + sceneName);

            if(buildIndex == 1)
            {
               
             

                    // Create a new GameObject and attach the AudioManager
                    GameObject audioManagerObject = new GameObject("AudioManagerObject");
                    AudioManager audioManager = audioManagerObject.AddComponent<AudioManager>();

                    audioManager.PlayAudio("welcome.wav");
                    audioManager.PlayAudio("music.wav");

                
            }
        
        }

    
    }
}
