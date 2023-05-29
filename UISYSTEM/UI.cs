using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using ABI_RC.Core.UI;
using cohtml;
using cohtml.Net;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UI  : MelonMod
    {
        private Page bluedescriptorpage;
        public BDWS webSocketClient;

        private Page acheivements;
        private bool vrcnameplate = false;
        private List<CVRPlayerEntity> plist = new List<CVRPlayerEntity>();
        bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
        bool rainbowhudv = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud");
        bool reward_vrshit = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrshit");
        bool reward_YOUMETBLUE = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "YOUMETBLUE");
        bool reward_blueleftyourlobby = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "blueleftyourlobby");
        bool reward_kannauwu = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "kannauwu");
        bool reward_shadowthehorny = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "shadowthehorny");
        bool reward_shadowthehornycummykeyboard = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "shadowthehornycummykeyboard");
       


        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            plist.Clear();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            rainbowhud();
        }
        void OnPlayerJoin(CVRPlayerEntity player)
        {
            /*
             * 
             * 
             * vrchat nameplate system
             * 
             */
            if (vrcplate)
            {

                Assembly asm = Assembly.GetExecutingAssembly();
                byte[] buffer = new byte[255];

                using (Stream stream = asm.GetManifestResourceStream("Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor.png"))
                {
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                }

                Texture2D tex = new Texture2D(256, 256);
                Texture2D texs = new Texture2D(256, 256);
                tex.LoadImage(buffer);

                Sprite spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                player.PlayerNameplate.nameplateBackground.sprite = spr;
                player.PlayerNameplate.nameplateBackground.gameObject.GetComponent<Image>().color = Color.white;
                player.PlayerNameplate.nameplateBackground.canvasRenderer.SetColor(Color.white);
                player.PlayerNameplate.nameplateBackground.canvasRenderer.SetColor(new Color32(255, 255, 255, 90));
                player.PlayerNameplate.nameplateBackground.canvasRenderer.SetAlpha(30 * 4);
                player.PlayerNameplate.usrNameText.color = new Color(1, 1, 1, 1);
                player.PlayerNameplate.gameObject.transform.position = new Vector3(player.PlayerNameplate.gameObject.transform.position.x,
                player.PlayerNameplate.gameObject.transform.position.y + 60,
                player.PlayerNameplate.gameObject.transform.position.z);
                player.PlayerNameplate.friendsImage.canvasRenderer.SetColor(Color.white);
                RainbowColorChange colorChanger = player.PlayerNameplate.gameObject.AddComponent<RainbowColorChange>();
                colorChanger.imageToChange = player.PlayerNameplate.friendsImage;
                colorChanger.textToChange = player.PlayerNameplate.usrNameText;
                colorChanger.StartChange();
                colorChanger.StartChangeTextOutline();


            }
        
}

        public void loadrewards()
        {
            var rewards = acheivements.AddCategory("Rewards");

            rewards.ClearChildren();


            //if else if else if else if else. eh who gives a fuck

            if (reward_vrshit)
            {
                rewards.AddButton("VRSHIT!!!!!", "", "You turned on classic nameplates.");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
            if (reward_YOUMETBLUE)
            {
                rewards.AddButton("BLUE!!!", "", "You entered a lobby with Bluethefox.");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
            if (reward_kannauwu)
            {
                rewards.AddButton("KANNA", "", "You entered a lobby with Kanna Kobayashi.");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
            if (reward_shadowthehornycummykeyboard)
            {
                rewards.AddButton("KANNA", "", "You entered a lobby with Shadow.EXE");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
        }
        public void menuinit()
        {
            bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage",true, "bd_logo");

      
            var profilecat =  bluedescriptorpage.AddCategory("Your Profile");
            acheivements = profilecat.AddPage("achievements", "bd_logo", "Your achievements from doing certain things in game.","Bluedescriptor");

            bluedescriptorpage.MenuTitle = "Blue descriptor properties";
            bluedescriptorpage.MenuSubtitle = "Properties to change how blue decriptor behaves.";
            var desktopuisettings = bluedescriptorpage.AddCategory("Desktop settings");
            var positionbutton = desktopuisettings.AddToggle("Show my position","show/hide position of the player",false);
            positionbutton.OnValueUpdated += b =>
            {
                if (GameObject.Find("BD_GW"))
                {
                    BTKUILib.QuickMenuAPI.ShowAlertToast("Cant use this feature in this world.");
                    MelonLogger.Msg("Game World detected");
                    positionbutton.ToggleValue = false;
                    return;
                }
                switch (positionbutton.ToggleValue)
                {
                    case true:
                        MelonEvents.OnGUI.Subscribe(onpositionui, 100);
                        break;
                    case false:
                        MelonEvents.OnGUI.Unsubscribe(onpositionui);
                        break;
                }
            };
            /*
             * 
             * 
             * General settings and features 
             * 
             */
            var general = bluedescriptorpage.AddCategory("General settings and features");
           
            //Rainbow HUD
            var RHUD = general.AddToggle("Rainbow HUD", "Enable a rainbow HUD interface", false);
            RHUD.OnValueUpdated += togler =>
            {

                switch(togler)
                {
                case true:
                        MelonPreferences.SetEntryValue("Bluedescriptor", "rainbowhud", true);

                        rainbowhudv = true;
                        
                break;

                case false:
                        MelonPreferences.SetEntryValue("Bluedescriptor", "rainbowhud", false);
                        rainbowhudv = false;
                        break;
                }
                rainbowhud();
                MelonPreferences.Save();
            };
            var reconnect = general.AddButton("Reconnect", "bd_reconnect", "Reconnect to blue descriptor network system");
            reconnect.OnPress += () => {
                webSocketClient.ConnectAsync("ws://localhost:9090", "");
            };
            var vrcnp = general.AddToggle("Classic nameplate", "Bring back the VRC 2019 nameplates", vrcplate);
            vrcnp.OnValueUpdated += val =>
            {
                vrcnameplate= val;
                MelonPreferences.SetEntryValue("Bluedescriptor", "vrcnameplate", val);
                vrcplate = val;
                if (val)
                {
                    MelonPreferences.SetEntryValue("Bluedescriptor", "vrshit", true);

                    BTKUILib.QuickMenuAPI.ShowAlertToast("NEW REWARD EARNED.");
                }
                MelonPreferences.Save();
            };
            var panicbutton = general.AddButton("Panic","bd_warn","removes all shaders from all avatars");
            panicbutton.OnPress += () =>
            {
                new UIfunctions().panic();
            };
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
                if(pl.Username == "bluethefox")
                {
                    MelonPreferences.SetEntryValue("Bluedescriptor", "YOUMETBLUE", true);

                }
                OnPlayerJoin(pl);
                MelonLogger.Msg(pl.Username);
                OnLateUpdate();

               
           
            };
            BTKUILib.QuickMenuAPI.UserLeave += pl =>
            {
                plist.Remove(pl);
                MelonLogger.Msg(pl.Username + " Left");
                OnLateUpdate();
            };


            //server connections
            webSocketClient.OnMessageReceived += OnMessageReceived;
            webSocketClient.OnOnlineUsersReceived += OnOnlineUsersReceived;

            webSocketClient.ConnectAsync("ws://localhost:9090", "");

        }

        private void OnMessageReceived(string message)
        {
            MelonLogger.Msg(message);

            //on heartbeat received
            if (message == "heartbeat")
            {

                webSocketClient.SendMessageAsync("heartbeat");

            }



        }

        private void OnOnlineUsersReceived(System.Collections.Generic.List<string> usernames)
        {

        }
        public void rainbowhud()
        {
            var animationCss = "var style = document.createElement('style');" +
                      "style.innerHTML = `" +
                      "@keyframes rainbowOpacity {" +
                      "  0% {opacity: 1;}" +
                      "  50% {opacity: 0.5;}" +
                      "  100% {opacity: 1;}" +
                      "}`;" +
                      "document.head.appendChild(style);";

            CohtmlHud.Instance.gameObject.GetComponent<CohtmlView>().View.ExecuteScript(animationCss);

            if (rainbowhudv)
            {

                var startAnimationCss = "var elements = document.querySelectorAll('.hex-hub');" +
                            "elements.forEach(function(element) {" +
                            "  element.style.animation = 'rainbowOpacity 2s infinite';" +
                            "});";

                CohtmlHud.Instance.gameObject.GetComponent<CohtmlView>().View.ExecuteScript(startAnimationCss);
     
            }
            else
            {


                CohtmlHud.Instance.gameObject.GetComponent<CohtmlView>().View.Reload();
            }
        }

        private void onpositionui()
        {
            float x = new CVRPlayer().localplayerposition()[0];
            float y = new CVRPlayer().localplayerposition()[1];
            float z = new CVRPlayer().localplayerposition()[2];
            GUI.Label(new Rect(20, 40, 1000, 200), $"<b><size=20>Player Position: <color=red>{x}</color>, <color=green>{y}</color>, <color=blue>{z}</color></size></b>");
        }
    }
}
