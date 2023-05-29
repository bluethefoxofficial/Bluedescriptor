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
     
        public BDWS webSocketClient;

        public Main()
        {
            this.webSocketClient = new BDWS();
        }


        public override void OnInitializeMelon()
        {
            
            webSocketClient.OnMessageReceived += OnMessageReceived;
            webSocketClient.OnOnlineUsersReceived += OnOnlineUsersReceived;

            var ui = new UI();
            ui.menuinit();
         
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
                break;

                default:
                    /* string localplayeruser = GameObject.Find("_PLAYERLOCAL").GetComponent<ABI_RC.Core.Player.CVRPlayerEntity>().Username;

                     webSocketClient.ConnectAsync("ws://localhost:9090", localplayeruser); */

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
