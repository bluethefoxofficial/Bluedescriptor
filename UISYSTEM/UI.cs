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
using System;
using RTG;
using BTKUILib;
using System.Linq;
using ABI_RC.Core.Networking.API.Responses;
using System.Net;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;

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
        bool memoryrepair = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
        bool rainbowhudv = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud");
        bool reward_vrshit = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrshit");
        bool reward_YOUMETBLUE = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "YOUMETBLUE");
        bool reward_blueleftyourlobby = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "blueleftyourlobby");
        bool reward_kannauwu = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "kannauwu");
        bool reward_shadowthehorny = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "shadowthehorny");
        bool reward_shadowthehornycummykeyboard = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "shadowthehornycummykeyboard");
        bool reward_gangmonkey = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "gangmonkey");
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            plist.Clear();
            
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            rainbowhud();
            new Classes.Memoryautoclear().OnFixedUpdate();

           new UIfunctions().quickmenyinitstyler(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin"));
        }

        //rewards system

        public void loadrewards()
        {
            var rewards = acheivements.AddCategory("Rewards");

            try
            {
                acheivements.ClearChildren();
            }
            catch { }


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
                rewards.AddButton("SHADOW", "", "You entered a lobby with Shadow.EXE");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
            if (reward_gangmonkey)
            {
                rewards.AddButton("GANG MONKEY CONVO", "", "You entered a lobby with 2 or more known gang monkey's.");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
            if (reward_blueleftyourlobby)
            {
                rewards.AddButton("YOU SAW BLUE LEFT", "", "Blue left your lobby");
            }
            else
            {
                rewards.AddButton("???", "", "???");
            }
        }

      public void  uiinit()
        {
            if (MelonLoader.MelonMod.RegisteredMelons.FirstOrDefault(m => m.Info.Name == "BTKUILib") != null)
                menuinit();
        }
        public void menuinit()
        {
                bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage",true, "bd_logo");
            var profilecat =  bluedescriptorpage.AddCategory("Your Profile");
            acheivements = profilecat.AddPage("achievements", "bd_rewards", "Your achievements from doing certain things in game.","Bluedescriptor");
          

            webSocketClient = new BDWS();

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


            /* skins
             * 
             * 
             * 
             */

            List<SkinInfo> list;
            var Skinselection = general.AddPage("Skins", "bd_themes", "","Bluedescriptor");
            var skinselection_list_settings = Skinselection.AddCategory("Skin settings");
            var skinselection_list_dld = Skinselection.AddCategory("Downloadable");
            var skinrldbtn = skinselection_list_settings.AddButton("Reload", "bd_reconnect","reload skin list");

            var skinreset = skinselection_list_settings.AddButton("Reset to default", "bd_trash", "reset the theme to the default (requires restarting)");
            skinreset.OnPress += () =>
            {
                MelonPreferences.SetEntryValue("Bluedescriptor", "quickmenuskin", "");
                MelonPreferences.Save();
                BTKUILib.QuickMenuAPI.ShowAlertToast("Skin reset, restart to apply.");
            };
            skinrldbtn.OnPress += () =>
            {
                list = skins.GetSkinInfo();
                try
                {
                    skinselection_list_dld.ClearChildren();
                }
                catch { }
                list.ForEach(s =>
                {

                    var btn = skinselection_list_dld.AddButton(s.SkinName, "bd_download", "Version: " + s.SkinVersion + " Author: " + s.SkinAuthor);
                    btn.OnPress += () =>
                    {

                        try
                        {
                            //download .zip from https://raw.githubusercontent.com/bluethefoxofficial/Bluedescriptor-themedatabase/main/<zip file here>

                            //extract to the path
                            string path = Path.GetFullPath(Assembly.GetExecutingAssembly().Location + "\\bluedescriptor\\skins\\" + s.SkinName);
                            if (Directory.Exists(path))
                            {
                                BTKUILib.QuickMenuAPI.ShowAlertToast("Skin already exists");
                                return;
                            }
                            else
                            {
                                Directory.CreateDirectory(path);



                                //download zip file
                                WebClient webClient = new WebClient();
                                string zipFilePath = path + "\\" + s.SkinName + ".zip";
                                webClient.DownloadFile("https://raw.githubusercontent.com/bluethefoxofficial/Bluedescriptor-themedatabase/main/" + s.SkinName + ".zip", zipFilePath);

                                //extract zip file
                                try
                                {
                                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, path);
                                }
                                catch (Exception ex)
                                {
                                    BTKUILib.QuickMenuAPI.ShowAlertToast("Failed to extract skin: " + ex.Message);
                                    return;
                                }

                                //delete zip file
                                File.Delete(zipFilePath);

                            }
                            //apply skin
                            //apply skin to the setting in melon preferences
                            MelonPreferences.SetEntryValue("Bluedescriptor", "quickmenuskin", s.SkinName);
                            MelonPreferences.Save();
                            new UIfunctions().quickmenyinitstyler(s.SkinName);

                            BTKUILib.QuickMenuAPI.ShowAlertToast("Skin downloaded");
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error(ex);
                        }
                    };
                    });
            
            };
            var reconnect = general.AddButton("Reconnect", "bd_reconnect", "Reconnect to blue descriptor network system");
            reconnect.OnPress += () => {
                try
                {
                    webSocketClient.ConnectAsync("ws://localhost:9090", new ABI_RC.Core.InteractionSystem.CVR_Menu_Data_Core().username);
                }
                catch(Exception ex)
                {
                    MelonLogger.Error(ex);
                }
            };
            var vrcnp = general.AddToggle("Classic nameplate", "Bring back the VRC 2019 nameplates", vrcplate);
            vrcnp.OnValueUpdated += val =>
            {
                vrcnameplate= val;
                MelonPreferences.SetEntryValue("Bluedescriptor", "vrcnameplate", val);
                vrcplate = val;
                if (val)
                {
                    if (!MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrshit"))
                    {
                        MelonPreferences.SetEntryValue("Bluedescriptor", "vrshit", true);
                        CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor REWARDS", "NEW REWARD EARNED.");
                    }
                    loadrewards();
                }
                MelonPreferences.Save();
            };
            var panicbutton = general.AddButton("Panic","bd_warn","removes all shaders from all avatars");
            panicbutton.OnPress += () =>
            {
                new UIfunctions().panic();
            };


            var Memoryclear = general.AddToggle("Memory repair", "Resolve memory issues and clean up memory overtime", memoryrepair);
            Memoryclear.OnValueUpdated += togler =>
            {

                switch (togler)
                {
                    case true:
                        MelonPreferences.SetEntryValue("Bluedescriptor", "memorycleanup", true);
                        break;

                    case false:
                        MelonPreferences.SetEntryValue("Bluedescriptor", "memorycleanup", false);
                        break;
                }

                MelonPreferences.Save();
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
                
                if (pl != null)
                {
                    new UIfunctions().OnPlayerJoin(pl);
                    MelonLogger.Msg(pl.Username + " Joined your lobby");
                    OnLateUpdate();
                }
            };
            BTKUILib.QuickMenuAPI.UserLeave += pl =>
            {
                if (pl != null)
                {
                    MelonLogger.Msg(pl.Username + " Left your lobby");
                    OnLateUpdate();
                }
            };
            //server connections
            webSocketClient.OnMessageReceived += OnMessageReceived;
            webSocketClient.OnOnlineUsersReceived += OnOnlineUsersReceived;
            webSocketClient.ConnectAsync("ws://localhost:9090", new ABI_RC.Core.InteractionSystem.CVR_Menu_Data_Core().username);
            loadrewards();

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
            //this is useless :3
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
            int x = new CVRPlayer().localplayerposition()[0];
            int y = new CVRPlayer().localplayerposition()[1];
            int z = new CVRPlayer().localplayerposition()[2];
            GUI.Label(new Rect(20, 40, 1000, 200), $"<b><size=20>Player Position: <color=red>{x}</color>, <color=green>{y}</color>, <color=blue>{z}</color></size></b>");
        }
    }
}
