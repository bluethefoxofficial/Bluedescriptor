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

        bool uiprep = false;
        public Main()
        {
            new Memoryautoclear().CleanupMemory() ;
        }
        public override void OnInitializeMelon()
        {
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.playful-notification.ogg","noti");
            new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.join.ogg","join");
            MelonPreferences.CreateCategory("Bluedescriptor","general");
            MelonPreferences.CreateEntry("Bluedescriptor", "vrcnameplate", false);
            MelonPreferences.CreateEntry("Bluedescriptor", "rainbowhud", false);
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
            MelonPreferences.CreateEntry("Bluedescriptor", "quickmenuskin","");
            //alarm
            MelonPreferences.CreateEntry("Bluedescriptor", "alarm",false);

            //nameplate settings
            MelonPreferences.CreateEntry("Bluedescriptor", "nameplate-speed", 0.5f);
            new UISYSTEM.Icons().iconsinit();
            new UI().uiinit();
        }
    
    }
}
