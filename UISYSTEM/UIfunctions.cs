using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;
using BTKUILib.UIObjects;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UIfunctions 
    {
        public void panic()
        {
            
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Safety","Panic was initilized reload all avatars to undo.");
            //get all gameobjects with the CVRAvatar component from all scenes
            GameObject[] players = new CVRPlayer().remotePlayers();
            //loop through all players
            foreach (var player in players)
            {
                MelonLogger.Msg("Logging Renderers: "+player);
                //get all renderers from the player
                Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
                //loop through all renderers
                foreach (var renderer in renderers)
                {
                    //get all materials from the renderer
                    Material[] materials = renderer.materials;
                    //loop through all materials
                    foreach (var material in materials)
                    {
                        //remove the shader from the material and replace it with a standard shader
                        material.shader = Shader.Find("Standard");
                    }
                }
                MeshRenderer[] meshrenderers = player.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshrenderer in meshrenderers)
                {
                    MelonLogger.Msg("Logging mesh renderers: " + player);
                    Material[] materials = meshrenderer.materials;
                    foreach (var material in materials)
                    {
                        material.shader = Shader.Find("Standard");
                    }
                }
            }
        }

        /* nameplate related */

        public IEnumerator IsTalking(object[] obj)
        {
            CVRPlayerEntity pl = (CVRPlayerEntity)obj[0];
            GameObject talker = (GameObject)obj[1];
            pl.PlayerNameplate.playerImage.canvasRenderer.SetAlpha((float)10000f);
            pl.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha((float)10000f);
            pl.PlayerNameplate.friendsImage.canvasRenderer.SetAlpha((float)10000f);
            talker.gameObject.GetComponent<Image>().canvasRenderer.SetAlpha(10000f);
            while (true)
            {
                //Fixed for 2021
                PlayerNameplate playerNameplateInstance = pl.PlayerNameplate;
                Type playerNameplateType = playerNameplateInstance.GetType();
                FieldInfo wasTalkingField = playerNameplateType.GetField("wasTalking", BindingFlags.Instance | BindingFlags.NonPublic);

                // Get the value of the "wasTalking" field for the playerNameplateInstance
                bool wasTalkingValue = (bool)wasTalkingField.GetValue(playerNameplateInstance);
                if (wasTalkingValue)
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


        public void OnPlayerJoin(CVRPlayerEntity player)
        {
          
            try
            {
                // Vrchat nameplate system
                bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
                if (vrcplate)
                {
                    if (player == null) return;
                    Assembly asm = Assembly.GetExecutingAssembly();
                 Texture2D visitorTexture = null;
                    // Load Visitor image
                    switch (MelonPreferences.GetEntryValue<int>("Bluedescriptor","nameplate")){
                case 0:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor.png");
                    break;   
                case 1:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor_2018.png");
                    break;
                case 2:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.VRC.vrcbannameplate.png");
                    break; 
                case 3:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.Grad.png");
                    break;
                case 4:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.smli.png");
                    break;
                case 5:
                    visitorTexture = LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Nameplates.grenade.png");
                    break;
                    }
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
                    player.PlayerNameplate.usrNameText.outlineWidth = 5;
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
        public Texture2D LoadTextureFromAssembly(Assembly asm, string resourcePath)
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

            player.PlayerNameplate.UpdateNamePlate();
            
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
            player.PlayerNameplate.playerImage.canvasRenderer.SetAlpha((float)10000f);
            player.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha((float)10000f);
            player.PlayerNameplate.friendsImage.color = new Color32(1, 1, 1, 255);
            player.PlayerNameplate.usrNameText.enableVertexGradient = true;
            player.PlayerNameplate.UpdateNamePlate();



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
            corountinemgr.GetComponent<Classes.CoroutineManager>().StartCoroutine(new UIfunctions().IsTalking(obj));
        }
        public void nameplatesettings(Category general)
        {
            var nameplatesettings = general.AddPage("Nameplate settings", "bd_npsettings", "Options to customise your nameplate", "Bluedescriptor");

            var nameplateimagesettings = nameplatesettings.AddCategory("Nameplate image");

            var twentynineteennp = nameplateimagesettings.AddButton("2019 nameplate", "", "Sets the nameplate to the VRC 2019 nameplate");
            var _2018nameplate = nameplateimagesettings.AddButton("2018 nameplate", "", "Sets the nameplate to the VRC 2018 nameplate");
            var vrcban = nameplateimagesettings.AddButton("VRC BANNED nameplate", "", "Sets the nameplate to the VRC BAN nameplate.");
            var gradnp = nameplateimagesettings.AddButton("Gradient nameplate", "", "Sets the nameplate to the Blue descriptor gradient nameplate.");
            var smli = nameplateimagesettings.AddButton("Small line nameplate", "", "Sets the nameplate to the Blue descriptor small line nameplate.");
            var granadenp = nameplateimagesettings.AddButton("Granade nameplate", "", "Sets the nameplate to the Blue descriptor granade nameplate.");
            twentynineteennp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 0); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to 2019 rejoin to apply."); };
            _2018nameplate.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 1); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to 2018 rejoin to apply."); };
            vrcban.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 2); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to VRC ban rejoin to apply."); };
            gradnp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 3); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to BD Gradient rejoin to apply."); };
            smli.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 4); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to BD small line rejoin to apply."); };
            granadenp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 5); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Nameplate", "nameplate set to BD granade rejoin to apply."); };
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
                string fullPath = asm.Location;
                string directoryPath = Path.GetDirectoryName(fullPath);
                string path = directoryPath + "\\bluedescriptor\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + "\\skins\\");
                }
                string skinpath = path + "\\skins\\" + downloadedtheme +"\\";

                MelonLogger.Msg("Skin path: " + skinpath);
                string qms = MelonPreferences.GetEntryValue<string>("Bluedescriptor", "quickmenuskin");
                if (qms != "")
                {
                    string cssPath = Path.Combine(skinpath, "skin.css");
                    string jsPath = Path.Combine(skinpath, "function.js");
                    MelonLogger.Msg("CSS Path "+cssPath);
                    MelonLogger.Msg("JS Path "+jsPath);
                    string cssContent = File.Exists(cssPath) ? File.ReadAllText(cssPath) : "";
                    string jsContent = File.Exists(jsPath) ? File.ReadAllText(jsPath) : "";
                   
                    string jsCode = $"document.querySelector('head').innerHTML += '<style id=\"bdstyler\">{cssContent}</style>';\n{jsContent}";
                   
                    var cohtml = GameObject.Find("Cohtml");

                    if (cohtml == null)
                    {
                        MelonLogger.Error("Error: cohtml not found");
                        return;
                    }

                    var qm = cohtml.transform.Find("QuickMenu");

                    if(qm == null)
                    {
                        MelonLogger.Error("Error: quick menu not found.");
                        return;
                    }
              
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
    }
}
