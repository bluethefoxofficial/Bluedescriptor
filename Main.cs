using ABI_RC.Core.Newton.NewtonEditor.Dependencies;
using Bluedescriptor_Rewritten.Classes;
using Bluedescriptor_Rewritten.UISYSTEM;
using BTKUILib.UIObjects;
using MelonLoader;
using RTG;
using System;
using System.Configuration.Assemblies;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Bluedescriptor_Rewritten.Main), "Blue Descriptor", "1.0.0", "Bluethefox")]

namespace Bluedescriptor_Rewritten
{   
    public class Main : MelonMod
    {
        private Page scpage;
        private Category[] scenecat;
        public TextMeshPro textToMonitor;
        private GameObject audio;

        //prefrences
        private MelonPreferences_Category Uipref;

        private bool prepared = false;

        public override void OnInitializeMelon()
        {      
            Uipref = MelonPreferences.CreateCategory("UI");

            LoggerInstance.Msg("Hello, youre running blue descriptor.\nBlue descriptor cant be used in game worlds there is a check in place to ensure that the world is not going to affect the mod.");
            var ui = new UI();
            ui.menuinit();
            scpage = ui.scenespage;
        }
        public override async void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("[BD] Scene loaded: "+sceneName);

            switch (sceneName)
            {
                case "Login":
                    MelonLogger.Msg("[BD] Login scene detected named: " + sceneName + " starting process for ui conversion.");
                    new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.bgmsc.ogg","bgmsc");
                    var login_img = GameObject.Find("CVRLogo");
                    byte[] img = null;
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.BLUEDESCRIPTOR.png"))
                    {
                        if (stream != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                 img = memoryStream.ToArray();

                            }
                        }
                    }

                    Texture2D texture = new Texture2D(600, 600);
                    texture.LoadImage(img);

                    login_img.GetComponent<Image>().material.mainTexture = texture;
                    GameObject audiogo = new GameObject();


                    Audio aud = new Audio();

                    
                    string audioFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location,"bluedescriptor/"+"bgmsc.ogg");
                    AudioClip currentClip = await aud.LoadClip(audioFilePath);
                    audiogo.GetComponent<AudioSource>().clip = currentClip;
                    audiogo.GetComponent<AudioSource>().Play();
                    break;
            }
        }
    }
}
 