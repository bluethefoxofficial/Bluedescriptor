using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;
using Bluedescriptor_Rewritten.UISYSTEM;
using BTKUILib.UIObjects;
using MelonLoader;
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
        private Page scpage;
        private MelonPreferences_Category Uipref;
        public BDWS webSocketClient;

        public Main()
        {
            this.webSocketClient = new BDWS();
        }


        public override void OnApplicationStart()
        {
            webSocketClient.OnMessageReceived += OnMessageReceived;
            webSocketClient.OnOnlineUsersReceived += OnOnlineUsersReceived;

            var ui = new UI();
            ui.menuinit();
            scpage = ui.scenespage;
        }
        

     

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
           
            switch (sceneName)
            {
                case "Init" :


                    string localplayeruser = GameObject.Find("_PLAYERLOCAL").GetComponent<ABI_RC.Core.Player.PlayerDescriptor>().userName;

                    webSocketClient.ConnectAsync("ws://localhost:9090",localplayeruser);

             

                    break;
                case "Headquarters":
                    break;
                case "Preperation":
                //skip this scene
                MelonLogger.Msg("[BD] Preperation scene detected named: " + sceneName + " skipping scene.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login",LoadSceneMode.Single);
                    break;
                case "Login":
                   // webSocketClient.ConnectAsync("ws://localhost:9090");
                    MelonLogger.Msg("[BD] Login scene detected named: " + sceneName + " starting process for UI conversion.");
                    new Audio().Audioprep("Bluedescriptor_Rewritten.res.Audio.bgmsc.ogg", "bgmsc");
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

                    /* Audio aud = new Audio();

                     string audioFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "bluedescriptor", "bgmsc.ogg");
                     AudioClip currentClip = await aud.LoadClip(audioFilePath);
                     audiogo.GetComponent<AudioSource>().clip = currentClip;
                     audiogo.GetComponent<AudioSource>().Play(); 
                   

                     */
                    break;
            }
        }

        private void OnMessageReceived(string message)
        {
            // Handle received message from WebSocket server
            // ...
        }

        private void OnOnlineUsersReceived(System.Collections.Generic.List<string> usernames)
        {
            // Handle received online user list from WebSocket server
            // ...
        }
    }
}
