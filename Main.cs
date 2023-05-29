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
     
     

        public Main()
        {
            

        }

        public static BDWS GetBDWS()
        {
            return new BDWS();
        }


        public override void OnInitializeMelon()
        {
            
           

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
       
         
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
           
            switch (sceneName)
            {
                case "Init":
                case "Headquarters":
                case "Preperation":
                    break;
                case "Login":
                    var ui = new UI();
                    ui.menuinit();
                    break;

                default:

     


                    break;
            }
        }


    }
}
