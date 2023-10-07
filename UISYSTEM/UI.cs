using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using System.Linq;
using System.Collections.Generic;
using BTKUILib;
using System.Reflection;
using UnityEngine.Networking;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Systems.ModNetwork;
using MenuManager = ABI_RC.Core.InteractionSystem.CVR_MenuManager;
using UnityEngine.UI;
using System.Diagnostics;
using System.IO;
using System;
using ABI_RC.Systems.GameEventSystem;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UI  : MelonMod
    {
        private Page bluedescriptorpage;
        private Alarm alrm;
        GameObject audioManagerObject;
        AudioManager audioManager;
        UIListCreator lis;
    
        public List<string[]> plist = new List<string[]>();
        bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
        bool memoryrepair = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "memorycleanup");
        bool rainbowhudv = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud");
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            plist.Clear();
            
        }
        bool firstload = false;

      public void  uiinit()
        {
            if (MelonLoader.MelonMod.RegisteredMelons.FirstOrDefault(m => m.Info.Name == "BTKUILib") != null)
                menuinit();


            CVRGameEventSystem.World.OnUnload.AddListener(wi =>
            {
                plist.Clear();
                lis.applylist(plist);

            });

            /* UI EVENTS */
            //open ui
            CVRGameEventSystem.QuickMenu.OnOpen.AddListener(() =>
            {
              
           
                audioManager.PlayAudio("leave.wav");
              

               
                lis.Show();
                lis.resetlistui();

            });
            //close ui
            CVRGameEventSystem.QuickMenu.OnClose.AddListener(() =>
            {
          

                audioManager.PlayAudio("close.wav");
                lis.resetlistui();
                lis.Hide();
                

            });
            /* ither */
            CVRGameEventSystem.Instance.OnConnectionLost.AddListener(ins =>
            {
                audioManager.PlayAudio("conerror.wav");
     
            });

            CVRGameEventSystem.Instance.OnConnected.AddListener(ins => {
                audioManagerObject = new GameObject("AudioManagerObject");
                audioManager = audioManagerObject.AddComponent<AudioManager>();
               

                if (firstload) return;
                lis.GenerateUICanvas();

                firstload = true;
            
            });
            CVRGameEventSystem.Home.OnJoin.AddListener(() =>
            {
             audioManagerObject = new GameObject("AudioManagerObject");
            audioManager = audioManagerObject.AddComponent<AudioManager>();
                if (firstload) return;
                lis.GenerateUICanvas();

                firstload = true;

            });

        }
        GameObject corountinemgr = new GameObject();

       

        public void menuinit()
        {
            corountinemgr.AddComponent<Classes.CoroutineManager>();
            
            bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage", true, "bd_logo");

            new EULA().eulacheck();

            QuickMenuAPI.OnOpenedPage += (string arg1, string arg2) =>
            {
                new EULA().eulacheck();
            };
            var profilecat = bluedescriptorpage.AddCategory("Your Profile");

             lis = new UIListCreator();
         


            bluedescriptorpage.MenuTitle = "Blue descriptor properties";
            bluedescriptorpage.MenuSubtitle = "Properties to change how blue decriptor behaves.";
            /*
             * 
             * 
             * General settings and features 
             * 
             */
            var general = bluedescriptorpage.AddCategory("General settings and features");
          

            new UIfunctions().nameplatesettings(general);
            var customnameplate = general.AddToggle("Custom nameplate", "Bring back vrc nameplates and add new custom nameplates", vrcplate);
            customnameplate.OnValueUpdated += val =>
            {
         
                MelonPreferences.SetEntryValue("Bluedescriptor", "vrcnameplate", val);
                vrcplate = val;
              
                MelonPreferences.Save();
            };
            var panicbutton = general.AddButton("Panic","bd_warn","removes all shaders from all avatars");
            panicbutton.OnPress += () =>
            {
                new UIfunctions().panic();
            };
      
            var antitoxic = general.AddButton("Antitoxin","bd_antitoxic","disconnect when you lag");
            antitoxic.OnPress += () =>
            {
             
            };
            var recordsystem = new ScreenRecorder();
            var recorder = general.AddPage("recorder","bd_ir","screen recorder","bluedescriptor");

            var recordercat = recorder.AddCategory("recorder");

           var recordbtn =recordercat.AddButton("record", "bd_ir","start a recording");
           var stoprecordbtn =recordercat.AddButton("stop", "bd_stoprec","stop a recording");
            var openrecordingsfolder = recordercat.AddButton("open recordings folder", "bd_recfol","open recordings folder in explorer");

            recordbtn.OnPress += () =>
            {
                recordsystem.StartRecording();
            };
            stoprecordbtn.OnPress += () =>
            {
                recordsystem.StopRecording();
            };
            openrecordingsfolder.OnPress += () =>
            {                                            
                Process.Start(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");
            };



            lis.applylist(plist);
     

            /*
             * 
             * 
             * Player events
             * 
             * 
             * 
             */
       
            BTKUILib.QuickMenuAPI.UserJoin += pl =>
            {
                plist.Add(new string[] { pl.Username });

                lis.applylist(plist);
                //   OnLateUpdate();
                lis.resetlistui();
                new UIfunctions().OnPlayerJoin(pl);
            MelonLogger.Msg(pl.Username + " Joined your lobby");


             audioManager.PlayAudio("join.wav");
              
            };
          
            BTKUILib.QuickMenuAPI.UserLeave += pl =>
            {
               
               

                        MelonLogger.Msg(pl.Username + " left your lobby");



                        audioManager.PlayAudio("leave.wav");

                        // Optionally, destroy the AudioManagerObject after some time if needed.

                        plist.Remove(new string[] { pl.Username });
                        lis.applylist(plist);
           
            
        };
        }
    }
}
