using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using System.IO;
using ABI_RC.Core.UI;
using cohtml;
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using BTKUILib;
using ABI_RC.Core.Networking.IO.Self;
using System.Diagnostics;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UI  : MelonMod
    {
        private Page bluedescriptorpage;
        private Alarm alrm;
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
                rewards.AddButton("VRSHIT!!!!!", "", "You turned on custom nameplates.");
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
            acheivements = profilecat.AddPage("achievements", "bd_rewards", "Your achievements from doing certain things in game.", "Bluedescriptor");


            webSocketClient = new BDWS();

      

 

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
            /*
             * 
             * 
             * Alarm clock
             * 
             * 
             */
            new UIfunctions().alarmsettings(general);

            /*
             * 
             * 
             * rainbow hud
             * 
             * 
             */

            var RHUD = general.AddToggle("Rainbow HUD", "Enable a rainbow HUD interface", false);
            RHUD.OnValueUpdated += togler =>
            {

                switch (togler)
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


            List<SkinInfo> list = skins.GetSkinInfo();
            var Skinselection = general.AddPage("Themes", "bd_themes", "manage theme settings", "Bluedescriptor");
            var skinselection_list_settings = Skinselection.AddCategory("Theme settings");
            var installed = skinselection_list_settings.AddPage("Installed themes", "bd_themes", "select themes", "Bluedescriptor");
  
            var skinselection_list_dld = Skinselection.AddCategory("Downloadable");

            skinselection_list_dld?.ClearChildren();

            if (list.Count == 0)
            {
                BTKUILib.QuickMenuAPI.ShowAlertToast("No skins found");
            }
            else
            {
                foreach (var s in list)
                {
                    var btn = skinselection_list_dld.AddButton(s.SkinName, "bd_download", "Version: " + s.SkinVersion + " Author: " + s.SkinAuthor);
                    btn.OnPress += () =>
                    {
                        try
                        {
                            string path = Path.GetFullPath(Assembly.Location + "\\bluedescriptor\\skins\\" + s.SkinName);
                            if (Directory.Exists(path))
                            {
                                BTKUILib.QuickMenuAPI.ShowAlertToast("Theme already exists");
                                return;
                            }

                            Directory.CreateDirectory(path);
                            MelonPreferences.SetEntryValue("Bluedescriptor", "quickmenuskin", s.SkinName);
                            MelonPreferences.Save();
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error(ex);
                        }
                    };
                }
            }

            var skinreset = skinselection_list_settings.AddButton("Reset to default", "bd_trash", "Reset the theme to the default (requires restarting)");
            skinreset.OnPress += () =>
            {
                MelonPreferences.SetEntryValue("Bluedescriptor", "quickmenuskin", "");
                MelonPreferences.Save();
                BTKUILib.QuickMenuAPI.ShowAlertToast("Theme reset, restart to apply.");
            };

            var skinrldbtn = skinselection_list_settings.AddButton("Reload", "bd_reconnect", "Reload theme list");
            skinrldbtn.OnPress += () =>
            {
                list = skins.GetSkinInfo();
                skinselection_list_dld?.ClearChildren();

                if (list.Count == 0)
                {
                    BTKUILib.QuickMenuAPI.ShowAlertToast("No skins found");
                }
                else
                {
                    foreach (var s in list)
                    {
                        var btn = skinselection_list_dld.AddButton(s.SkinName, "bd_download", "Version: " + s.SkinVersion + " Author: " + s.SkinAuthor);
                        btn.OnPress += () =>
                        {
                            new skins().Installskin($"https://github.com/bluethefoxofficial/Bluedescriptor-themedatabase/raw/main/{s}.zip", new utils().GetAssemblyDirectory()+"\\bluedescriptor\\skins\\"+s+"\\");
                        };
                    }
                }
            };



            /*
             * 
             *  messaging system (experimental)
             * 
             *  Global chat
             */


            var globalchatbtn = general.AddButton("Global message", "", "Send a message to everyone using Blue Descriptor");

            globalchatbtn.OnPress += () =>
            {
                BTKUILib.QuickMenuAPI.OpenKeyboard("", (message) =>
                {
                    // Send the global message to the server
                    webSocketClient.SendGlobalMessageAsync(message);
                });
            };

            var users = general.AddPage("Message a user","","","Bluedescriptor");

            /*
             * 
             * reconnect
             */
            var reconnect = general.AddButton("Reconnect", "bd_reconnect", "Reconnect to blue descriptor network system");
            reconnect.OnPress += () => {
                try
                {
                    webSocketClient.DisconnectAsync();
                    MelonLogger.Msg("DEBUG USERNAME: "+ ABI_RC.Core.Savior.MetaPort.Instance.username);
                    string usrn = ABI_RC.Core.Savior.MetaPort.Instance.username;
                    webSocketClient.ConnectAsync("ws://localhost:9090", usrn);
                }
                catch(Exception ex)
                {
                    MelonLogger.Error(ex);
                }
            };
            var customnameplate = general.AddToggle("Custom nameplate", "Bring back vrc nameplates and add new custom nameplates", vrcplate);
            customnameplate.OnValueUpdated += val =>
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
            general.AddButton("memory fix on click","","fixes memory issues on click").OnPress += ()=>{
                new Memoryautoclear().CleanupMemory();
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
                    new UIfunctions(). OnPlayerJoin(pl);
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
            if (CohtmlHud.Instance.gameObject != null)
            {
                if (rainbowhudv)
                {
                    var animationCss = "var style = document.createElement('style');" +
                              "style.innerHTML = `" +
                              ".hud-area-left-bottom .hex-hub, " +
                              ".hud-area-left-bottom .shard-infos, " +
                              ".hud-area-left-bottom .shard-friends, " +
                              ".hud-area-left-bottom .shard-notifications, " +
                              ".hud-area-left-bottom .shard-chats, " +
                              ".hud-area-left-bottom .shard-votes {" +
                              "  animation: rainbowOpacity 5s infinite;" +
                              "}`;" +
                              "document.head.appendChild(style);";

                    CohtmlHud.Instance.gameObject.GetComponent<CohtmlView>().View.ExecuteScript(animationCss);
                }
                else
                {
                    var removeAnimationCss = "var style = document.querySelector('style');" +
                                "if (style) {" +
                                "  style.remove();" +
                                "}";
                    CohtmlHud.Instance.gameObject.GetComponent<CohtmlView>().View.ExecuteScript(removeAnimationCss);
                }
            }
        }
    }
}
