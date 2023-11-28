using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;
using BTKUILib.UIObjects;
using MelonLoader;
using System;
using System.Collections.Generic;
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
        private MonoBehaviour monoBehaviour;
        private static HashSet<string> playersWithMod = new HashSet<string>();
        public List<Sprite> talkerIconSprites;
        bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
        public UIfunctions()
        {

            talkerIconSprites = new List<Sprite>();
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourcePaths = {
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_0.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_1.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_2.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_3.png"
    };

            foreach (var resourcePath in resourcePaths)
            {
                var texture = LoadTextureFromAssembly(asm, resourcePath);
                if (texture != null)
                {
                    var sprite = CreateSpriteFromTexture(texture);
                    if (sprite != null)
                    {
                        talkerIconSprites.Add(sprite);
                    }
                    else
                    {
                        MelonLogger.Error($"Failed to create sprite from texture: {resourcePath}");
                    }
                }
                else
                {
                    MelonLogger.Error($"Failed to load texture from assembly: {resourcePath}");
                }
            }
        }

        public void panic()
        {
            CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Safety", "Panic was initilized reload all avatars to undo.");

            //get all gameobjects with the CVRAvatar component from all scenes
            GameObject[] players = new CVRPlayer().remotePlayers();

            //loop through all players
            foreach (var player in players)
            {
                MelonLogger.Msg("Logging Renderers: " + player);

                // Stop all animations on the player
                Animator[] animators = player.GetComponentsInChildren<Animator>();
                foreach (var animator in animators)
                {
                    animator.StopPlayback();
                }

                // Stop all audio sources on the player, unless they are on a VivoxParticipantTracker
                AudioSource[] audioSources = player.GetComponentsInChildren<AudioSource>();
                foreach (var audioSource in audioSources)
                {
                    if (audioSource.gameObject.name != "VivoxParticipantTracker")
                    {
                        audioSource.Stop();
                    }
                }

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
            pl.PlayerNameplate.playerImage.canvasRenderer.SetAlpha((float)10000f);
            pl.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha((float)10000f);
            pl.PlayerNameplate.friendsImage.canvasRenderer.SetAlpha((float)10000f);
            GameObject talker = (GameObject)obj[1];

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

                pl.PlayerNameplate.playerImage.canvasRenderer.SetAlpha((float)10000f);
                pl.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha((float)10000f);
                pl.PlayerNameplate.friendsImage.canvasRenderer.SetAlpha((float)10000f);
                talker.gameObject.GetComponent<Image>().canvasRenderer.SetAlpha(10000f);
                yield return null;
            }
        }

        private Dictionary<int, string> texturePaths = new Dictionary<int, string>
{
    {0, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor.png"},
    {1, "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor_2018.png"},
    {2, "Bluedescriptor_Rewritten.res.Nameplates.VRC.vrcbannameplate.png"},
    {5, "Bluedescriptor_Rewritten.res.Nameplates.Grad.png"},
    {6, "Bluedescriptor_Rewritten.res.Nameplates.smli.png"},
    {7, "Bluedescriptor_Rewritten.res.Nameplates.grenade.png"},

};


        public void OnPlayerJoin(CVRPlayerEntity player)
        {
            try
            {
                bool vrcPlate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
                if (!vrcPlate || player == null) return;

                Assembly asm = Assembly.GetExecutingAssembly();
                int nameplatePreference = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "nameplate");
                string texturePath = texturePaths.ContainsKey(nameplatePreference) ? texturePaths[nameplatePreference] : null;

                if (!string.IsNullOrEmpty(texturePath))
                {
                    Texture2D visitorTexture = LoadTextureFromAssembly(asm, texturePath);
                    Sprite visitorSprite = CreateSpriteFromTexture(visitorTexture);
                    ApplyNameplateSettings(player, visitorSprite);
                    ProcessPlayerNameplate(player);
                }

                // New code to show an icon if the player has the mod installed
                if (PlayerHasModInstalled(player.Username))
                {
                    ShowModIconOnNameplate(player);
                }
            }
            catch
            {
                // Handle any exceptions here
            }
        }
        public static void RegisterPlayerWithMod(string playerId)
        {
            playersWithMod.Add(playerId);
        }

        public static bool PlayerHasModInstalled(string playerId)
        {
            return playersWithMod.Contains(playerId);
        }
        public static void ShowModIconOnNameplate(CVRPlayerEntity player)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
       
            var nameplateUI = player.PlayerNameplate;
            if (nameplateUI != null)
            {
                // Load or create the sprite for the mod icon
                Sprite modIcon = null; // Implement LoadModIconSprite to load the sprite
              
                Material nm = new Material(Shader.Find("Alpha Blend Interactive/BillboardFacing"));


                // Create a new UI element (e.g., an Image component) for the icon
                var iconElement = new GameObject("ModIcon").AddComponent<UnityEngine.UI.Image>();
                iconElement.material = nm;
                iconElement.sprite = modIcon;

                iconElement.transform.localPosition = new Vector3(player.PlayerNameplate.gameObject.transform.localPosition.x + 0.4343f,
           player.PlayerNameplate.gameObject.transform.localPosition.y + 0.12f,
           player.PlayerNameplate.gameObject.transform.localPosition.z);

                // Position the icon element on the nameplate
                iconElement.transform.SetParent(nameplateUI.transform, false);
                iconElement.transform.localPosition = new Vector3(0, 0, 0); // Adjust position as needed

                // Optionally, set the size of the icon
                iconElement.rectTransform.sizeDelta = new Vector2(20, 20); // Example size
            }
        }
        private void ProcessPlayerNameplate(CVRPlayerEntity player)
        {
            if (player?.PlayerNameplate == null) return;

            var nameplate = player.PlayerNameplate;
            var blackAndWhiteConverter = new BlackAndWhiteConverter();

            // Convert friendsImage to black and white
            Texture2D originalTexture = blackAndWhiteConverter.TextureToTexture2D(nameplate.friendsImage.mainTexture);
            Texture2D blackAndWhiteTexture = blackAndWhiteConverter.ConvertToBlackAndWhite(originalTexture);
            Sprite blackAndWhiteSprite = CreateSpriteFromTexture(blackAndWhiteTexture);
            nameplate.friendsImage.sprite = blackAndWhiteSprite;

            // Adjust position
            var friendsImagePosition = nameplate.friendsImage.transform.localPosition;
            nameplate.friendsImage.transform.localPosition = new Vector3(friendsImagePosition.x - 0.04f, friendsImagePosition.y, friendsImagePosition.z);

            // Apply color changes
            var colorChanger = nameplate.gameObject.AddComponent<RainbowColorChange>();
            colorChanger.imageToChange = nameplate.friendsImage;
            colorChanger.textToChange = nameplate.usrNameText;

            // Text settings
            nameplate.usrNameText.outlineWidth = 5;
            if (nameplate.usrNameText.material == null)
            {
                nameplate.usrNameText.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            }
            nameplate.usrNameText.material.SetFloat("_OutlineWidth", 0.1f);
            nameplate.usrNameText.material.SetColor("_OutlineColor", Color.red);

            colorChanger.StartChange();
            colorChanger.StartChangeText();
            colorChanger.StartChangeTextOutline();

            // Load and setup Talker icon
            Texture2D talkerTexture = LoadTextureFromAssembly(Assembly.GetExecutingAssembly(), "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_0.png");
            Sprite talkerSprite = CreateSpriteFromTexture(talkerTexture);
            SetupTalkerIcon(player, talkerSprite);
            
        }

        public Texture2D LoadTextureFromAssembly(Assembly asm, string resourcePath)
        {
            if (asm == null) throw new ArgumentNullException(nameof(asm));
            if (string.IsNullOrEmpty(resourcePath)) throw new ArgumentException("Resource path cannot be null or empty.", nameof(resourcePath));

            using (Stream stream = asm.GetManifestResourceStream(resourcePath))
            {
                if (stream == null) throw new InvalidOperationException($"Resource '{resourcePath}' not found in assembly.");

                if (stream.Length == 0) return null;

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                Texture2D texture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
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
            if (player?.PlayerNameplate == null) return;

            var nameplate = player.PlayerNameplate;
            nameplate.UpdateNamePlate();

            var colorChangeComponent = nameplate.nameplateBackground.gameObject.AddComponent<RainbowColorChange>();
            colorChangeComponent.imageToChange = nameplate.nameplateBackground;

               var fullWhite = Color.white;

            nameplate.nameplateBackground.sprite = sprite;
           
            nameplate.usrNameText.color = fullWhite;
            nameplate.friendsImage.color = fullWhite;

            nameplate.gameObject.transform.position += new Vector3(0, 60, 0);

            // Assuming 10000f is intentional for a specific effect
            nameplate.playerImage.canvasRenderer.SetAlpha(10000f);
            nameplate.usrNameText.canvasRenderer.SetAlpha(10000f);

            nameplate.usrNameText.enableVertexGradient = true;

            Texture2D convertedTexture = new TextureConversion().ConvertTextureToTexture2D(nameplate.nameplateBackground.mainTexture);
            colorChangeComponent.StartCoroutine(colorChangeComponent.ChangeHueOnEdgesCoroutine(convertedTexture));
        }

        private void SetupTalkerIcon(CVRPlayerEntity player, Sprite sprite)
        {
            player.PlayerNameplate.UpdateNamePlate();
            GameObject talker = new GameObject("TalkerIcon");
            Material nm = new Material(Shader.Find("Alpha Blend Interactive/BillboardFacing"));

            talker.transform.rotation = player.PlayerNameplate.transform.rotation;
          
            Image talkerImage = talker.AddComponent<Image>();
            talkerImage.material = nm;
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
            talker.transform.SetParent(player.PlayerNameplate.gameObject.transform.Find("Canvas").transform);
            talker.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
            object[] obj = new object[2] { player, talker };

            GameObject corountinemgr = new GameObject();
            RainbowColorChange colorChanger = corountinemgr.AddComponent<RainbowColorChange>();
            colorChanger.imageToChange = talkerImage;
            colorChanger.StartChange();
            corountinemgr.name = "BDNPM";
            corountinemgr.transform.SetParent(player.PlayerNameplate.gameObject.transform);
            corountinemgr.AddComponent<Classes.CoroutineManager>();
            corountinemgr.GetComponent<Classes.CoroutineManager>().StartCoroutine(new UIfunctions().IsTalking(obj));
            corountinemgr.GetComponent<Classes.CoroutineManager>().StartCoroutine(new UIfunctions().AnimateTalkerIcon(talkerImage));
        }

        private IEnumerator AnimateTalkerIcon(Image talkerImage)
        {
            int spriteIndex = 0;
            while (true) // Infinite loop to keep the animation running
            {
                talkerImage.sprite = talkerIconSprites[spriteIndex];
                spriteIndex = (spriteIndex + 1) % talkerIconSprites.Count; // Loop back to the first sprite when the end is reached
                yield return new WaitForSeconds(0.2f); // Wait for 200ms before changing to the next sprite
            }
        }
        public void nameplatesettings(Category general)
        {
            var nameplatesettings = general.AddPage("Nameplate settings", "bd_npsettings", "Options to customise your nameplate", "Bluedescriptor");
            nameplatesettings.AddSlider("nameplate speed", "change rainbow speed", MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"),0,100).OnValueUpdated += v =>
            {
                MelonPreferences.SetEntryValue("Bluedescriptor", "nameplate-speed",v);
                MelonPreferences.Save();
            };
            var nameplstegeneralsettings = nameplatesettings.AddCategory("General Nameplate settings");
            var customnameplate = nameplstegeneralsettings.AddToggle("Custom nameplate", "Bring back vrc nameplates and add new custom nameplates", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate"));
            customnameplate.OnValueUpdated += val =>
            {

                MelonPreferences.SetEntryValue("Bluedescriptor", "vrcnameplate", val);
                vrcplate = val;

                MelonPreferences.Save();
            };

            var nameplateimagesettings = nameplatesettings.AddCategory("Nameplate image");

            var twentynineteennp = nameplateimagesettings.AddButton("2019 nameplate", "bd_npsettings", "Sets the nameplate to the VRC 2019 nameplate");
            var _2018nameplate = nameplateimagesettings.AddButton("2018 nameplate", "bd_npsettings", "Sets the nameplate to the VRC 2018 nameplate");
            var vrcban = nameplateimagesettings.AddButton("VRC BANNED nameplate", "bd_npsettings", "Sets the nameplate to the VRC BAN nameplate.");
            var gradnp = nameplateimagesettings.AddButton("Gradient nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor gradient nameplate.");
            var smli = nameplateimagesettings.AddButton("Small line nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor small line nameplate.");
            var granadenp = nameplateimagesettings.AddButton("Granade nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor granade nameplate.");
            twentynineteennp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 0); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to 2019 rejoin to apply."); };
            _2018nameplate.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 1); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to 2018 rejoin to apply."); };
            vrcban.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 2); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to VRC ban rejoin to apply."); };
            gradnp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 3); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to BD Gradient rejoin to apply."); };
            smli.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 4); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to BD small line rejoin to apply."); };
            granadenp.OnPress += () => { MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 5); MelonPreferences.Save(); CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Nameplate", "nameplate set to BD granade rejoin to apply."); };
        
        }

    }
}
