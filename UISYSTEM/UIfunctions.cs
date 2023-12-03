
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
  internal class UIfunctions
  {
    private MonoBehaviour monoBehaviour;
    private static HashSet<string> playersWithMod = new HashSet<string>();
    public List<Sprite> talkerIconSprites;
    private bool vrcplate = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate");
    private Dictionary<int, string> texturePaths = new Dictionary<int, string>()
    {
      {
        0,
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor_LR.png"
      },
      {
        1,
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor_2018.png"
      },
      {
        2,
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.vrcbannameplate.png"
      },
      {
        5,
        "Bluedescriptor_Rewritten.res.Nameplates.Grad.png"
      },
      {
        6,
        "Bluedescriptor_Rewritten.res.Nameplates.smli.png"
      },
      {
        7,
        "Bluedescriptor_Rewritten.res.Nameplates.grenade.png"
      }
    };

    public UIfunctions()
    {
      talkerIconSprites = new List<Sprite>();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      string[] strArray = new string[4]
      {
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_0.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_1.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_2.png",
        "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_3.png"
      };
      foreach (string resourcePath in strArray)
      {
        Texture2D texture = LoadTextureFromAssembly(executingAssembly, resourcePath);
        if ( texture != null)
        {
          Sprite spriteFromTexture = CreateSpriteFromTexture(texture);
          if ( spriteFromTexture != null)
            talkerIconSprites.Add(spriteFromTexture);
          else
            MelonLogger.Error("Failed to create sprite from texture: " + resourcePath);
        }
        else
          MelonLogger.Error("Failed to load texture from assembly: " + resourcePath);
      }
    }

    public void panic()
    {
      CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Safety", "Panic was initilized reload all avatars to undo.");
      foreach (GameObject remotePlayer in new CVRPlayer().remotePlayers())
      {
        MelonLogger.Msg("Logging Renderers: " + remotePlayer?.ToString());
        foreach (Animator componentsInChild in remotePlayer.GetComponentsInChildren<Animator>())
          componentsInChild.StopPlayback();
        foreach (AudioSource componentsInChild in remotePlayer.GetComponentsInChildren<AudioSource>())
        {
          if ((componentsInChild.gameObject).name != "VivoxParticipantTracker")
            componentsInChild.Stop();
        }
        foreach (Renderer componentsInChild in remotePlayer.GetComponentsInChildren<Renderer>())
        {
          foreach (Material material in componentsInChild.materials)
            material.shader = Shader.Find("Standard");
        }
        foreach (MeshRenderer componentsInChild in remotePlayer.GetComponentsInChildren<MeshRenderer>())
        {
          MelonLogger.Msg("Logging mesh renderers: " + remotePlayer?.ToString());
          foreach (Material material in componentsInChild.materials)
            material.shader = Shader.Find("Standard");
        }
      }
    }

    public IEnumerator IsTalking(object[] obj)
    {
      CVRPlayerEntity pl = (CVRPlayerEntity) obj[0];
            pl.PlayerNameplate.playerImage.canvasRenderer.SetAlpha(10000f);
            pl.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha(10000f);
            pl.PlayerNameplate.friendsImage.canvasRenderer.SetAlpha(10000f);
      GameObject talker = (GameObject) obj[1];
      while (true)
      {
        PlayerNameplate playerNameplateInstance = pl.PlayerNameplate;
        Type playerNameplateType = playerNameplateInstance.GetType();
        FieldInfo wasTalkingField = playerNameplateType.GetField("wasTalking", BindingFlags.Instance | BindingFlags.NonPublic);
        if ((bool) wasTalkingField.GetValue( playerNameplateInstance))
          talker.SetActive(true);
        else
          talker.SetActive(false);
                pl.PlayerNameplate.playerImage.canvasRenderer.SetAlpha(10000f);
                pl.PlayerNameplate.usrNameText.canvasRenderer.SetAlpha(10000f);
                pl.PlayerNameplate.friendsImage.canvasRenderer.SetAlpha(10000f);
                talker.gameObject.GetComponent<Image>().canvasRenderer.SetAlpha(10000f);
        yield return  null;
        playerNameplateInstance = null;
        playerNameplateType = null;
        wasTalkingField = null;
      }
    }

    public void OnPlayerJoin(CVRPlayerEntity player)
    {
      try
      {
        if (!MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate") || player == null)
          return;
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        int entryValue = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "nameplate");
        string texturePath = texturePaths.ContainsKey(entryValue) ? texturePaths[entryValue] : null;
        if (!string.IsNullOrEmpty(texturePath))
        {
          Sprite spriteFromTexture = CreateSpriteFromTexture(LoadTextureFromAssembly(executingAssembly, texturePath));
          ApplyNameplateSettings(player, spriteFromTexture);
          ProcessPlayerNameplate(player);
        }
        if (!UIfunctions.PlayerHasModInstalled(player.Username))
          return;
        UIfunctions.ShowModIconOnNameplate(player);
      }
      catch
      {
      }
    }

    public static void RegisterPlayerWithMod(string playerId) => UIfunctions.playersWithMod.Add(playerId);

    public static bool PlayerHasModInstalled(string playerId) => UIfunctions.playersWithMod.Contains(playerId);

    public static void ShowModIconOnNameplate(CVRPlayerEntity player)
    {
      Assembly.GetExecutingAssembly();
      PlayerNameplate playerNameplate = player.PlayerNameplate;
      if ( playerNameplate != null)
        return;
      Sprite sprite = null;
      Material material = new Material(Shader.Find("Alpha Blend Interactive/BillboardFacing"));
      Image image = new GameObject("ModIcon").AddComponent<Image>();
            image.material = material;
      image.sprite = sprite;
            image.transform.localPosition = new Vector3(player.PlayerNameplate.gameObject.transform.localPosition.x + 0.4343f, player.PlayerNameplate.gameObject.transform.localPosition.y + 0.12f, player.PlayerNameplate.gameObject.transform.localPosition.z);
            image.transform.SetParent(playerNameplate.transform, false);
            image.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
    }

    private void ProcessPlayerNameplate(CVRPlayerEntity player)
    {
      if (player?.PlayerNameplate == null)
        return;
      PlayerNameplate playerNameplate = player.PlayerNameplate;
      BlackAndWhiteConverter andWhiteConverter = new BlackAndWhiteConverter();
      Texture2D texture2D = andWhiteConverter.TextureToTexture2D(playerNameplate.friendsImage.mainTexture);
      Sprite spriteFromTexture1 = CreateSpriteFromTexture(andWhiteConverter.ConvertToBlackAndWhite(texture2D));
      playerNameplate.friendsImage.sprite = spriteFromTexture1;
      Vector3 localPosition = playerNameplate.friendsImage.transform.localPosition;
            playerNameplate.friendsImage.transform.localPosition = new Vector3(localPosition.x - 0.04f, localPosition.y, localPosition.z);
      RainbowColorChange rainbowColorChange = playerNameplate.gameObject.AddComponent<RainbowColorChange>();
      rainbowColorChange.imageToChange = playerNameplate.friendsImage;
      rainbowColorChange.textToChange = playerNameplate.usrNameText;
      playerNameplate.usrNameText.outlineWidth = 5f;
            if (playerNameplate.usrNameText.material == null)
                playerNameplate.usrNameText.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            playerNameplate.usrNameText.material.SetFloat("_OutlineWidth", 0.1f);
            playerNameplate.usrNameText.material.SetColor("_OutlineColor", Color.red);
      rainbowColorChange.StartChange();
      rainbowColorChange.StartChangeText();
      rainbowColorChange.StartChangeTextOutline();
      Sprite spriteFromTexture2 = CreateSpriteFromTexture(LoadTextureFromAssembly(Assembly.GetExecutingAssembly(), "Bluedescriptor_Rewritten.res.Nameplates.VRC.talker.Talker_0.png"));
      SetupTalkerIcon(player, spriteFromTexture2);
    }

    public Texture2D LoadTextureFromAssembly(Assembly asm, string resourcePath)
    {
      if (asm == null)
        throw new ArgumentNullException(nameof (asm));
      if (string.IsNullOrEmpty(resourcePath))
        throw new ArgumentException("Resource path cannot be null or empty.", nameof (resourcePath));
      using (Stream manifestResourceStream = asm.GetManifestResourceStream(resourcePath))
      {
        if (manifestResourceStream == null)
          throw new InvalidOperationException("Resource '" + resourcePath + "' not found in assembly.");
        if (manifestResourceStream.Length == 0L)
          return null;
        byte[] buffer = new byte[manifestResourceStream.Length];
        manifestResourceStream.Read(buffer, 0, buffer.Length);
        Texture2D texture2D = new Texture2D(256, 256, (TextureFormat) 4, false);
        ImageConversion.LoadImage(texture2D, buffer);
        return texture2D;
      }
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture) => Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);

    private void ApplyNameplateSettings(CVRPlayerEntity player, Sprite sprite)
    {

            PlayerNameplate playerNameplate = player.PlayerNameplate;
            playerNameplate.UpdateNamePlate();
            RainbowColorChange rainbowColorChange = playerNameplate.nameplateBackground.gameObject.AddComponent<RainbowColorChange>();
            rainbowColorChange.imageToChange = playerNameplate.nameplateBackground;
            Color white = Color.white;
            playerNameplate.nameplateBackground.sprite = sprite;
            playerNameplate.usrNameText.color = white;
            playerNameplate.friendsImage.color = white;
            playerNameplate.gameObject.transform.position += new Vector3(0f, 60f, 0f);
            playerNameplate.playerImage.canvasRenderer.SetAlpha(10000f);
            playerNameplate.usrNameText.canvasRenderer.SetAlpha(10000f);
            playerNameplate.usrNameText.enableVertexGradient = true;
            Texture2D tex = new TextureConversion().ConvertTextureToTexture2D(playerNameplate.nameplateBackground.mainTexture);
          
        }

        private void SetupTalkerIcon(CVRPlayerEntity player, Sprite sprite)
        {
            player.PlayerNameplate.UpdateNamePlate();

            GameObject talkerIconObj = new GameObject("TalkerIcon");
            Material billboardMaterial = new Material(Shader.Find("Alpha Blend Interactive/BillboardFacing"));
            talkerIconObj.transform.rotation = player.PlayerNameplate.transform.rotation;

            Image talkerIconImage = talkerIconObj.AddComponent<Image>();
            talkerIconImage.material = billboardMaterial;
            talkerIconImage.sprite = sprite;
            talkerIconImage.color = Color.white;

            RectTransform talkerIconRect = talkerIconObj.GetComponent<RectTransform>();
            talkerIconRect.sizeDelta = new Vector2(99f, 55f);
            talkerIconObj.transform.position = player.PlayerNameplate.gameObject.transform.position;
            talkerIconObj.transform.localPosition = new Vector3(player.PlayerNameplate.gameObject.transform.localPosition.x + 0.2343f, player.PlayerNameplate.gameObject.transform.localPosition.y + 0.12f, player.PlayerNameplate.gameObject.transform.localPosition.z);
            talkerIconObj.transform.SetParent(player.PlayerNameplate.gameObject.transform.Find("Canvas").transform);
            talkerIconObj.transform.localScale = new Vector3(1f / 500f, 1f / 500f, 1f / 500f);

            object[] coroutineParameters = new object[2] { player, talkerIconObj };

            GameObject coroutineManagerObj = new GameObject("BDNPM");
            RainbowColorChange rainbowColorChanger = coroutineManagerObj.AddComponent<RainbowColorChange>();
            rainbowColorChanger.imageToChange = talkerIconImage;
            rainbowColorChanger.StartChange();

            coroutineManagerObj.transform.SetParent(player.PlayerNameplate.gameObject.transform);
            CoroutineManager coroutineManager = coroutineManagerObj.AddComponent<CoroutineManager>();
            coroutineManager.StartCoroutine(new UIfunctions().IsTalking(coroutineParameters));
            coroutineManager.StartCoroutine(new UIfunctions().AnimateTalkerIcon(talkerIconImage));
        }


        private IEnumerator AnimateTalkerIcon(Image talkerImage)
    {
      int spriteIndex = 0;
      while (true)
      {
        talkerImage.sprite = talkerIconSprites[spriteIndex];
        spriteIndex = (spriteIndex + 1) % talkerIconSprites.Count;
        yield return  new WaitForSeconds(0.2f);
      }
    }
        //nameplate settings
        public void nameplatesettings(Category general)
        {
            Page page = general.AddPage("Nameplate settings", "bd_npsettings", "Options to customize your nameplate", "Bluedescriptor");
            page.AddSlider("nameplate speed", "change rainbow speed", MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"), 0.0f, 100f).OnValueUpdated += v =>
            {
                MelonPreferences.SetEntryValue<float>("Bluedescriptor", "nameplate-speed", v);
                MelonPreferences.Save();
            };
            page.AddCategory("General Nameplate settings").AddToggle("Custom nameplate", "Bring back vrc nameplates and add new custom nameplates", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "vrcnameplate")).OnValueUpdated += val =>
            {
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "vrcnameplate", val);
                vrcplate = val;
                MelonPreferences.Save();
            };

            Category advancedsettings = page.AddCategory("advanced nameplate settings");
            advancedsettings.AddToggle("Low nameplate res", "low resolution for nameplate images (faster load times and reduced lag)", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "nameplatelowres")).OnValueUpdated += v =>
            {
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "nameplatelowres", v);
           
                MelonPreferences.Save();
            };

            Category category = page.AddCategory("Nameplate image");

            var btn2019Nameplate = category.AddButton("2019 nameplate", "bd_npsettings", "Sets the nameplate to the VRC 2019 nameplate");
            var btn2018Nameplate = category.AddButton("2018 nameplate", "bd_npsettings", "Sets the nameplate to the VRC 2018 nameplate");
            var btnVrcBannedNameplate = category.AddButton("VRC BANNED nameplate", "bd_npsettings", "Sets the nameplate to the VRC BAN nameplate.");
            var btnGradientNameplate = category.AddButton("Gradient nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor gradient nameplate.");
            var btnSmallLineNameplate = category.AddButton("Small line nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor small line nameplate.");
            var btnGranadeNameplate = category.AddButton("Granade nameplate", "bd_npsettings", "Sets the nameplate to the Blue descriptor granade nameplate.");

            btn2019Nameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 0);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to 2019 rejoin to apply.");
            };

            btn2018Nameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 1);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to 2018 rejoin to apply.");
            };

            btnVrcBannedNameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 2);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to VRC ban rejoin to apply.");
            };

            btnGradientNameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 3);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to BD Gradient rejoin to apply.");
            };

            btnSmallLineNameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 4);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to BD small line rejoin to apply.");
            };

            btnGranadeNameplate.OnPress += () =>
            {
                MelonPreferences.SetEntryValue<int>("Bluedescriptor", "nameplate", 5);
                MelonPreferences.Save();
                CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Nameplate", "nameplate set to BD granade rejoin to apply.");
            };
        }
    }
}
