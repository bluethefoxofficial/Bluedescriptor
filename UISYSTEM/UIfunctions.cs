using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;

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

        bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
        public void Quickmenupanelinit()
        {
            //implement logic later :skull:
        }
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


        /* alarm clock functions */

        


        public IEnumerable Alarm()
        {
            while (MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "alarm")) {

                int hour = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "alarm_hour");
                int minute = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "alarm_minute");

                //get current time
                int current_hour = DateTime.Now.Hour;
                int current_minute = DateTime.Now.Minute;

                //check if current time is equal to alarm time
                if (current_hour == hour && current_minute == minute)
                {
                    //play alarm sound
                    AudioSource aud = new AudioSource();
                    string assemblyDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                    Audio audio = new Audio();
                    audio.LoadClip(Path.Combine(assemblyDirectory, "bluedescriptor") + "alarm.ogg").ContinueWith(ac =>
                    {
                        aud.clip = ac.Result;
                        aud.Play();
                    });
                    //show alarm message
                    CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Alarm", $"The alarm was triggered at {hour}:{minute}");
                    //wait 1 minute
                    yield return new WaitForSeconds(60);
                }
                yield return null;

            }
        }


        public void OnPlayerJoin(CVRPlayerEntity player)
        {
            try
            {
                if (player.PlayerNameplate.friendsImage.isActiveAndEnabled)
                {
                    AudioSource aud = new AudioSource();
                    string assemblyDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                    Audio audio = new Audio();
                    audio.LoadClip(Path.Combine(assemblyDirectory, "bluedescriptor") + "join.ogg").ContinueWith(ac =>
                    {
                        aud.clip = ac.Result;
                        aud.Play();
                        MelonLogger.Msg("UWUOWOUWU joined");
                    });
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
            try
            {
            
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
            corountinemgr.GetComponent<Classes.CoroutineManager>().StartCoroutine(new UIfunctions().IsTalking(obj));
        }


        /* quick menu related */

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
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
    }
}
