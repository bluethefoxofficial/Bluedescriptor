using ABI_RC.Core.UI;
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

[assembly: MelonInfo(typeof(Bluedescriptor_Rewritten.Main), "Blue Descriptor", "1.0.0", "Bluethefox")]

namespace Bluedescriptor_Rewritten
{
    public class Main : MelonMod
    {

        UI uisystem = new UI();

    
        public Main()
        {
            new Memoryautoclear().CleanupMemory() ;
        }

        public override void OnEarlyInitializeMelon()
        {
           
        }
        public override void OnInitializeMelon()
        {
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.playful-notification.ogg","noti");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.join.ogg","join");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.eula.ogg","eula");
            MelonPreferences.CreateCategory("Bluedescriptor","general");
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
            new UISYSTEM.Icons().iconsinit();
            uisystem.uiinit();
        }



        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            uisystem.rainbowhud();
            new Classes.Memoryautoclear().OnFixedUpdate();


          

            new UIfunctions().quickmenyinitstyler(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin"));


            MelonLogger.Msg("Buld index: " + buildIndex + " | Scene name: " + sceneName);


            if (sceneName == "Headquarters")
            {
                if (ABI_RC.Core.Savior.MetaPort.Instance.username != null)
                {
                    uisystem.webSocketClient.ConnectAsync("ws://localhost:9090", ABI_RC.Core.Savior.MetaPort.Instance.username);

                    uisystem.webSocketClient.OnMessageReceived += s =>
                    {

                        MelonLogger.Msg($"Received message: {s}");

                    };
                    uisystem.webSocketClient.OnConnected += () =>
                    {

                        MelonLogger.Msg("connected to blue descriptor network system");

                    };
                }
            }
        }



    }
}
