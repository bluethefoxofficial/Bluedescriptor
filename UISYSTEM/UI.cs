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

            quickmenyinitstyler(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin"));
        }
        
        void OnPlayerJoin(CVRPlayerEntity player)
        {
            try
            {
                // Load rewards
                loadrewards();
                // Vrchat nameplate system
                if (vrcplate)
                {
                    if (player == null) return;
                    Assembly asm = Assembly.GetExecutingAssembly();
                    // Load Visitor image
                    Texture2D visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor.png");
                    Sprite visitorSprite = CreateSpriteFromTexture(visitorTexture);
                    ApplyNameplateSettings(player, visitorSprite);
                    // Convert friendsImage to black and white
                    Texture2D blackAndWhiteTexture = new BlackAndWhiteConverter().ConvertToBlackAndWhite(new BlackAndWhiteConverter().TextureToTexture2D(player.PlayerNameplate.friendsImage.mainTexture));
                    Sprite blackAndWhiteSprite = CreateSpriteFromTexture(blackAndWhiteTexture);
                    player.PlayerNameplate.friendsImage.sprite = blackAndWhiteSprite;
                    player.PlayerNameplate.friendsImage.transform.localPosition = new Vector3(player.PlayerNameplate.friendsImage.transform.localPosition.x - 0.04f, player.PlayerNameplate.friendsImage.transform.localPosition.y, player.PlayerNameplate.friendsImage.transform.localPosition.z);
                    // Apply color changes
                    RainbowColorChange colorChanger = player.PlayerNameplate.gameObject.AddComponent<RainbowColorChange>();
                    colorChanger.imageToChange = player.PlayerNameplate.friendsImage;
                    colorChanger.textToChange = player.PlayerNameplate.usrNameText;
                    colorChanger.StartChange();
                    colorChanger.StartChangeTextOutline();
                    // Load and setup Talker icon
                    Texture2D talkerTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Talker.png");
                    Sprite talkerSprite = CreateSpriteFromTexture(talkerTexture);
                    SetupTalkerIcon(player, talkerSprite);
                }
            }
            catch
            {
                // Handle exceptions
            }
        }
       

        private Texture2D LoadTextureFromAssembly(Assembly asm, string resourcePath)
        {
            using (Stream stream = asm.GetManifestResourceStream(resourcePath))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                Texture2D texture = new Texture2D(256, 256);
                texture.LoadImage(buffer);
                return texture;
            }
        }

        private Sprite CreateSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        private void ApplyNameplateSettings(CVRPlayerEntity player, Sprite sprite)
        {
            player.PlayerNameplate.nameplateBackground.sprite = sprite;
            player.PlayerNameplate.nameplateBackground.gameObject.GetComponent<Image>().color = Color.white;
            player.PlayerNameplate.nameplateBackground.color = Color.white;
            player.PlayerNameplate.nameplateBackground.canvasRenderer.SetColor(Color.white);
            player.PlayerNameplate.nameplateBackground.canvasRenderer.SetColor(new Color32(255, 255, 255, 70));
            player.PlayerNameplate.nameplateBackground.canvasRenderer.SetAlpha(10 * 4);
            player.PlayerNameplate.usrNameText.color = new Color(1, 1, 1, 1);
            player.PlayerNameplate.gameObject.transform.position = new Vector3(player.PlayerNameplate.gameObject.transform.position.x,
            player.PlayerNameplate.gameObject.transform.position.y + 60,
            player.PlayerNameplate.gameObject.transform.position.z);
            player.PlayerNameplate.friendsImage.canvasRenderer.SetColor(Color.white);
            player.PlayerNameplate.playerImage.canvasRenderer.SetAlpha((float)10000f);
            player.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha((float)10000f);
            player.PlayerNameplate.friendsImage.color = new Color32(1, 1, 1, 255);
        }
        private void SetupTalkerIcon(CVRPlayerEntity player, Sprite sprite)
        {
            GameObject talker = new GameObject("TalkerIcon");
            Image talkerImage = talker.AddComponent<Image>();
            talkerImage.sprite = sprite;
            // Ensure image is visible
            talkerImage.color = Color.white;
            // Set size through RectTransform
            RectTransform rt = talker.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(99, 55);  // Adjust these values as needed
            talker.transform.position = player.PlayerNameplate.gameObject.transform.position;
            talker.transform.localPosition = new Vector3(player.PlayerNameplate.gameObject.transform.localPosition.x + 0.2343f,
                player.PlayerNameplate.gameObject.transform.localPosition.y + 0.12f,
                player.PlayerNameplate.gameObject.transform.localPosition.z);

            // Set the rotation to match the nameplate's rotation
            talker.transform.rotation = player.PlayerNameplate.gameObject.transform.rotation;

            talker.transform.SetParent(player.PlayerNameplate.gameObject.transform.Find("Canvas").gameObject.transform.Find("Content").gameObject.transform);
            talker.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
            object[] obj = new object[2] { player, talker };

            // new CoroutineManager.StartManagedCoroutine("IsTalking", IsTalking(obj));

            GameObject corountinemgr = new GameObject();

            corountinemgr.transform.parent = player.PlayerNameplate.gameObject.transform;
            corountinemgr.name = "BDNPM";
            corountinemgr.AddComponent<Classes.CoroutineManager>();
            corountinemgr.GetComponent<Classes.CoroutineManager>().StartCoroutine(IsTalking(obj));
        }
        private IEnumerator IsTalking(object[] obj)
        {
           
            CVRPlayerEntity pl = (CVRPlayerEntity)obj[0];
            GameObject talker = (GameObject)obj[1];

            while (true)
            {
                if (pl.TalkerAmplitude > 0)
                {
                    talker.SetActive(true);
                }
                else
                {
                    talker.SetActive(false);
                } 

                yield return null;
            }
        }

        /*
         * 
         * QUICK MENU INIT
         * 
         */
        public void quickmenyinitstyler(string downloadedtheme)
        {

            try
            {
                // Load a skin from a folder with 2 files: skin.css and function.js. Look for skin.css.
                Assembly asm = Assembly.GetExecutingAssembly();
                string path = Path.GetFullPath(asm.Location + "\\bluedescriptor\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + "\\skins\\");
                }

                string skinpath = path + "\\skins\\" + downloadedtheme;
                string qms = MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin");
                if (qms != "")
                {
                    string cssPath = Path.Combine(skinpath, "skin.css");
                    string jsPath = Path.Combine(skinpath, "function.js");
                    string cssContent = File.Exists(cssPath) ? File.ReadAllText(cssPath) : "";
                    string jsContent = File.Exists(jsPath) ? File.ReadAllText(jsPath) : "";
                    // Execute the JavaScript code with the loaded CSS and JavaScript contents
                    string jsCode = $"document.querySelector('head').innerHTML += '<style>{cssContent}</style>';\n{jsContent}";
                    //ABI_RC.Core.InteractionSystem.CVR_MenuManager.Instance.quickMenu(jsCode); in 2021 this breaks
                }
            }catch(Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }

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
                            quickmenyinitstyler(s.SkinName);

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
                    OnPlayerJoin(pl);
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
