
using ABI.CCK.Components;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.UI;
using ABI_RC.Core.Util.AssetFiltering;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.VideoPlayer.Scripts;
using Bluedescriptor_Rewritten.Classes;
using BTKUILib;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using Color = System.Drawing.Color;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
  internal class UI : MelonMod
  {
    private Page bluedescriptorpage;
    private GameObject audioManagerObject;
    private AudioManager audioManager;
    private UIListCreator lis;
    private GameObject musicplayerGO;
    private MusicPlayer mp;
    private Category songs;
    private Page videoremote;
    private Page playlistPage;
    private Category playlistCategory;
    private moddingnetwork mn;
    private Category videoplayers;
    public ThemeEngine the;
    public List<string[]> plist = new List<string[]>();
    private bool uisounds = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", nameof (uisounds));
    private bool playerlistval = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "playerlist");
    private bool firstload = false;
    private GameObject corountinemgr = new GameObject();
    private HashSet<string> addedVideoPlayers = new HashSet<string>();

    public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) => plist.Clear();

    public void uiinit()
    {
      if (MelonTypeBase<MelonMod>.RegisteredMelons.FirstOrDefault<MelonMod>(m => m.Info.Name == "BTKUILib") != null)
        menuinit();
      CVRGameEventSystem.World.OnLoad.AddListener(wl => { });
      CVRGameEventSystem.World.OnUnload.AddListener(wi =>
      {
          plist.Clear();
          lis.applylist(plist);
      });
      CVRGameEventSystem.Spawnable.OnInstantiate.AddListener((s, item) => remotevideolist());
      CVRGameEventSystem.Spawnable.OnDestroy.AddListener((s, item) => remotevideolist());
      CVRGameEventSystem.Instance.OnDisconnected.AddListener(ins => remotevideolist());
      CVRGameEventSystem.Instance.OnConnected.AddListener(ins =>
      {
          mn.SendMessageToAll(MetaPort.Instance.ownerId);
          remotevideolist();
      });
            CVRGameEventSystem.QuickMenu.OnOpen.AddListener(
                () =>
                {
                    if (uisounds && uisounds)
                        audioManager.PlayAudio("leave.wav");
                    lis.Show();
                });
      CVRGameEventSystem.QuickMenu.OnClose.AddListener(() =>
      {
          lis.resetlistui();
          lis.Hide();
          if (uisounds && uisounds)
              audioManager.PlayAudio("close.wav");
          if (!playerlistval)
              return;
          lis.Hide();
      });
      CVRGameEventSystem.Instance.OnConnectionLost.AddListener(ins =>
      {
          if (!uisounds)
              return;
          audioManager.PlayAudio("conerror.wav");
      });
      CVRGameEventSystem.Instance.OnConnected.AddListener(ins =>
      {
          remotevideolist();
          audioManagerObject = new GameObject("AudioManagerObject");
          audioManager = audioManagerObject.AddComponent<AudioManager>();
          if (firstload)
              return;
          lis.GenerateUICanvas();
          firstload = true;
      });
      CVRGameEventSystem.Home.OnJoin.AddListener(() =>
      {
          remotevideolist();
          audioManagerObject = new GameObject("AudioManagerObject");
          audioManager = audioManagerObject.AddComponent<AudioManager>();
          if (firstload)
              return;
          lis.GenerateUICanvas();
          firstload = true;
      });
    }

        public void menuinit()
        {
            corountinemgr.AddComponent<CoroutineManager>();
            bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage", true, "bd_logo");
            the = new ThemeEngine();
            bluedescriptorpage.AddCategory("Blue Descriptor Configurator");
            lis = new UIListCreator();
            bluedescriptorpage.MenuTitle = "Blue descriptor properties";
            bluedescriptorpage.MenuSubtitle = "Properties to change how blue decriptor behaves.";
            Category general = bluedescriptorpage.AddCategory("General settings and features");
            new UIfunctions().nameplatesettings(general);
            general.AddButton("Panic", "bd_warn", "removes all shaders from all avatars").OnPress += () => new UIfunctions().panic();
            Page page1 = general.AddPage("Theme engine", "bd_themes", "Theme engine", "bluedescriptor");
            Category category1 = page1.AddCategory("");
            Category installedthemes = page1.AddCategory("installed themes");
            category1.AddButton("refresh theme list", "bd_reconnect", "refresh theme list").OnPress += () =>
            {
                installedthemes.ClearChildren();
                if (the.GetInstalledThemes() == null)
                    return;
                foreach (string installedTheme in the.GetInstalledThemes())
                {
                    string theme = installedTheme;
                    installedthemes.AddButton("apply " + theme, "bd_themes", "apply " + theme).OnPress += () =>
                    {
                        the.ApplyThemeQM(theme);
                        MelonPreferences.SetEntryValue<string>("Bluedescriptor", "appliedtheme", theme);
                        MelonPreferences.Save();
                    };
                }
            };
            if (the.GetInstalledThemes() != null)
            {
                foreach (string installedTheme in the.GetInstalledThemes())
                {
                    string theme = installedTheme;
                    installedthemes.AddButton("apply " + theme, "bd_themes", "apply " + theme).OnPress += () =>
                    {
                        the.ApplyThemeQM(theme);
                        MelonPreferences.SetEntryValue<string>("Bluedescriptor", "appliedtheme", theme);
                        MelonPreferences.Save();
                    };
                }
            }
            page1.AddCategory("Online theme Downloader.");
            Page page2 = general.AddPage("Antitoxin", "bd_antitoxic", "disconnect when you lag", "bluedescriptor");
            page2.AddSlider("wip", "this will allow you to adjust fps when it is complete", 10f, 2f, 20f);
            page2.AddCategory("Antitoxic toggles").AddToggle("autopanic", "autopanic", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "at_autopanic")).OnValueUpdated += val =>
            {
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "at_autopanic", val);
                if (val)
                {
                    MelonLogger.Msg("autopanic enabled");
                    CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Safety", "Autopanic is enabled");
                }
                else
                {
                    MelonLogger.Msg("autopanic disabled");
                    CohtmlHud.Instance.ViewDropTextImmediate("Blue Descriptor", "Blue Descriptor Safety", "Autopanic is disabled");
                }
            };
            general.AddButton("Rejoin current instance", "bd_reconnect", "reconnect").OnPress += () => QuickMenuAPI.ShowConfirm("You sure", "this action will force you to rejoin which can annoy people, only do so if you think you need to, continue", () => { }, () => { });
            Page globalsettings = general.AddPage("global ui settings", "bd_uisetup", "global seyimgs for BD UI", "bluedescriptor");
            globalsettings.AddCategory("UI sound fx").AddToggle("BD UI sounds", "enable/disable ui sounds", uisounds).OnValueUpdated += v =>
            {
                MelonLogger.Msg(string.Format("Saving UI sounds preference: {0}", v));
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "uisoundfx", v);
                MelonPreferences.Save();
                uisounds = v;
                MelonLogger.Msg("UI sounds preference saved");
            };
            var hudsettings = globalsettings.AddCategory("Hud settings");

            hudsettings.AddToggle("Rainbow HUD toggle","R A I N B O W", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud")).OnValueUpdated += v =>
            {
                MelonLogger.Msg("rainbow road was cool!");
               
            };
            globalsettings.AddCategory("Playerlist settings").AddToggle("playerlist toggle", "Enable/Disable playerlist", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "playerlist")).OnValueUpdated += v =>
            {
                MelonLogger.Msg(string.Format("Saving player list preference: {0}", v));
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "playerlist", v);
                MelonPreferences.Save();
                if (v)
                    lis.Show();
                else
                    lis.Hide();
                playerlistval = v;
                MelonLogger.Msg("Player list preference saved");
            };
            Page page4 = general.AddPage("music+", "bd_musicplus", "music player controller", "bluedescriptor");
            page4.AddSlider("Volume", "Song volume", 1f, 0.0f, 1f).OnValueUpdated += v =>
            {
                mp.SetVolume(v);
                playlistlist();
            };
            SliderFloat seek = page4.AddSlider("", "", 0.0f, 0.0f, 0.0f);
            seek.OnValueUpdated += v => mp.SetPlaybackPosition(v);
            Category category2 = page4.AddCategory("Music + controller");
            category2.AddButton("Play", "bd_playi", "continue playing").OnPress += () =>
            {
                mp.Play();
                seek.MaxValue = 0.0f;
                seek.MaxValue = mp.GetTotalDuration();
                mp.OnPlaybackTimeUpdate += v =>
                {
                    seek.MaxValue = mp.GetTotalDuration();
                    seek.SetSliderValue(v);
                };
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate("Playing", mp.CurrentlyPlaying() ?? "", "");
            };
            category2.AddButton("Pause", "bd_pausei", "Pause current track").OnPress += () =>
            {
                mp.Pause();
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate("Paused", "", "");
            };
            category2.AddButton("Previous", "bd_r", "<<").OnPress += () =>
            {
                seek.MaxValue = 0.0f;
                seek.MaxValue = mp.GetTotalDuration();
                mp.PlayPrevious();
                playlistlist();
                seek.SetSliderValue(0.0f);
                CohtmlHud.Instance.ViewDropTextImmediate("Now playing", mp.CurrentlyPlaying() ?? "", "");
            };
            category2.AddButton("Next", "bd_ff", ">>").OnPress += () =>
            {
                mp.PlayNext();
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate("Now playing", mp.CurrentlyPlaying() ?? "", "");
            };
            category2.AddButton("stop", "bd_stopi", "stop all music").OnPress += () =>
            {
                seek.MaxValue = 0.0f;
                seek.MaxValue = mp.GetTotalDuration();
                mp.audioSource.Stop();
                playlistlist();
                seek.SetSliderValue(0.0f);
                mp.currentTrackIndex = 0;
                CohtmlHud.Instance.ViewDropTextImmediate("STOPPED", "", "");
            };
            category2.AddButton("Clear Playlist", "bd_trash", "clear playlist").OnPress += () =>
            {
                seek.MaxValue = 0.0f;
                seek.MaxValue = mp.GetTotalDuration();
                mp.playlist.Clear();
                mp.audioSource.Stop();
                seek.SetSliderValue(0.0f);
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate("Cleared", "", "");
            };
            category2.AddToggle("Loop", "toggle loop on and off", false).OnValueUpdated += val =>
            {
                mp.audioSource.loop = val;
                playlistlist();
            };
            category2.AddButton("refresh", "bd_reconnect", "refresh songs in folder list.").OnPress += () =>
            {
                musiclist();
                playlistlist();
            };
            playlistPage = category2.AddPage("playlist", "bd_playlist", "show playlist structure/edit", "bluedescriptor");
            playlistCategory = playlistPage.AddCategory("current playlist");
            songs = page4.AddCategory("==songs in folder=============");

            if (MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "experimental-features")) { 
            Category category3 = general.AddPage("portable video player", "bd_videoplayer", "Portable video player configuration", "bluedescriptor").AddCategory("video player configuration");
            var button1 = category3.AddButton("Spawn player", "bd_videoplayer", "spawn a portable player");
            var button2 = category3.AddButton("Destroy player", "bd_videoplayer", "Destroys the video player");
            button1.OnPress += (Action)(() =>
            {
                const string videoPlayerName = "BD VP";

                // Check if a video player already exists in the scene
                if (GameObject.Find(videoPlayerName))
                {
                    CohtmlHud.Instance.ViewDropText("Cannot Spawn", "You can only have one portable video player.");
                    return;
                }

                // Create a new GameObject for the video player
                GameObject videoPlayerObject = new GameObject(videoPlayerName);
                videoPlayerObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                // Create a primitive for mesh and material
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                MeshFilter meshFilter = primitive.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = primitive.GetComponent<MeshRenderer>();

                if (meshFilter == null || meshRenderer == null)
                {
                    MelonLogger.Error("Primitive mesh components not found.");
                    return;
                }

                // Copy mesh and material to the video player object
                videoPlayerObject.AddComponent<MeshFilter>().mesh = meshFilter.mesh;
                videoPlayerObject.AddComponent<MeshRenderer>().material = meshRenderer.material;
                GameObject.Destroy(primitive);

                // Set up the render texture for the video player
                RenderTexture renderTexture = new RenderTexture(1920, 1080, 24)
                {
                    depthStencilFormat = GraphicsFormat.R32_SFloat,
                    enableRandomWrite = true,
                    wrapMode = TextureWrapMode.Repeat,
                    format = RenderTextureFormat.ARGB32
                };

                // Configure the CVRVideoPlayer component
                CVRVideoPlayer cvrVideoPlayer = videoPlayerObject.AddComponent<CVRVideoPlayer>();

                // Check MetaPort.Instance and ownerId before using them
                if (MetaPort.Instance == null || string.IsNullOrEmpty(MetaPort.Instance.ownerId))
                {
                    MelonLogger.Error("MetaPort.Instance or ownerId is null.");
                    return;
                }
                MelonLogger.Msg("owner id raw" + MetaPort.Instance.ownerId);
                cvrVideoPlayer.playerId = MetaPort.Instance.ownerId;
                cvrVideoPlayer.ProjectionTexture = renderTexture;
                cvrVideoPlayer.interactiveUI = true;
                cvrVideoPlayer.currentlySelectedPlaylist = new CVRVideoPlayerPlaylist();
                cvrVideoPlayer.syncEnabled = false;

                // Create a UI GameObject and set it as a child of the video player object
                GameObject uiGameObject = new GameObject("UI");
                uiGameObject.transform.SetParent(videoPlayerObject.transform, false);
                cvrVideoPlayer.videoPlayerUIPosition = uiGameObject.transform;

                // Position the video player near the player
                CVRPlayer player = new CVRPlayer();
                if (player == null || player.Localplayer() == null)
                {
                    MelonLogger.Error("Player or Localplayer is null.");
                    return;
                }

                Vector3 playerPosition = player.Localplayer().transform.position;
                playerPosition.y = 0.8f;
                videoPlayerObject.transform.localPosition = playerPosition;

                // Create an audio source for the video player
                GameObject audioSourceObject = new GameObject("AudioOutput");
                audioSourceObject.transform.SetParent(videoPlayerObject.transform);
                AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
                if (audioSource == null)
                {
                    MelonLogger.Error("Failed to add AudioSource component.");
                    return;
                }
                cvrVideoPlayer.customAudioSource = audioSource;

                // Add pickup and interaction components
                videoPlayerObject.AddComponent<CVRPickupObject>();
                videoPlayerObject.AddComponent<BoxCollider>().isTrigger = true;
            });


            button2.OnPress += (Action)(() =>
      {
          GameObject.Destroy(GameObject.Find("BD VP"));
      });



        }
      videoremote = general.AddPage("video player controller", "bd_videoplayer", "video player controller.", "bluedescriptor");
      videoplayers = videoremote.AddCategory("video player list");
      ScreenRecorder recordsystem = new ScreenRecorder();
      Category category4 = general.AddPage("recorder", "bd_ir", "screen recorder", "bluedescriptor").AddCategory("recorder");
      var button3 = category4.AddButton("record", "bd_ir", "start a recording");
      var button4 = category4.AddButton("stop", "bd_stoprec", "stop a recording");
      var button5 = category4.AddButton("open recordings folder", "bd_recfol", "open recordings folder in explorer");
      general.AddPage("optim configure", "bd_optim", "optimisation settings", "bluedescriptor").AddSlider("cull distance", "set autocull distance", 50f, 0.0f, 256f);
      general.AddPage("VRC Transitioner", "bd_vrc", "Ease the Transition.", "bluedescriptor").AddCategory("transitional settings").AddButton("Download and apply catonal themes.", "", "Download and apply themes to emulate the vrchat old style.");
      button3.OnPress += () => CoroutineRunner.Instance.StartRoutine(recordsystem.StartRecording());
      button4.OnPress += () => CoroutineRunner.Instance.StartRoutine(recordsystem.StopRecording());
      button5.OnPress += () => Process.Start(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");
      lis.applylist(plist);
      QuickMenuAPI.UserJoin += pl =>
      {
          if (UIfunctions.PlayerHasModInstalled(pl.Uuid))
              UIfunctions.ShowModIconOnNameplate(pl);
          plist.Add(new string[3]
          {
          pl.Username,
          Friends.FriendsWith(pl.PlayerDescriptor.ownerId).ToString(),
          pl.Uuid
          });
          lis.applylist(plist);
          lis.resetlistui();
          new UIfunctions().OnPlayerJoin(pl);
          MelonLogger.Msg(pl.Username + " Joined your lobby");
          try
          {
              if (!Friends.FriendsWith(pl.PlayerDescriptor.ownerId) || !uisounds)
                  return;
              audioManager.PlayAudio("join.wav");
          }
          catch
          {
          }
      };
      QuickMenuAPI.UserLeave += pl =>
      {
          MelonLogger.Msg(pl.Username + " left your lobby");
          if (Friends.FriendsWith(pl.PlayerDescriptor.ownerId) && uisounds)
              audioManager.PlayAudio("leave.wav");
          plist.RemoveAll(item => item.Contains<string>(pl.Username));
          lis.applylist(plist);
          lis.resetlistui();
      };
      if (MelonPreferences.GetEntryValue<string>("Bluedescriptor", "appliedtheme").Length > 0)
      {
        try
        {
          the.ApplyThemeQM(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "appliedtheme"));
        }
        catch
        {
        }
      }
      MelonLogger.Msg(Color.DarkBlue, "\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@      @@@@@@           @@@@@@@@,                    @@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@   @@@@@@                @@@@@@,                       @@@@@@@@@@@@\r\n@@@@@@@@@@@@@(@@@@@   *@@@@@@         (@@@@@,                         @@@@@@@@@@\r\n@@@@@@@@@@@@@@@@      *@@@@@@,        @@@@@@,        (@@@@@@@         @@@@@@@@@@\r\n@@@@@@@@@@@@@/                       @@@@@@@,        (@@@@@@@&         @@@@@@@@@\r\n@@@@@@@@@@@@@                       @@@@@@@@@@@@@@@@@@@@@@@@@@         @@@@@@@@@\r\n@@@@@@@@@@@@@                          @@@@@,        (@@@@@@@&         @@@@@@@@@\r\n@@@@@@@@@@@@@         *@@@@@@@@         @@@@,        (@@@@@@@          @@@@@@@@@\r\n@@@@@@@@@@@@@         *@@@@@@@          @@@@,        (@@@@@           @@@@@@@@@@\r\n@@@@@@@@@@@@@                          @@@@@,                       (@@@@@@@@@@@\r\n@@@@@@@@@@@@@                        @@@@@@@,                     @@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
    }

    public void musicplayerinit()
    {
      musicplayerGO = new GameObject();
      mp = musicplayerGO.AddComponent<MusicPlayer>();
      musicplayerGO.AddComponent<AudioSource>();
      mp.Awake();
      GameObject.DontDestroyOnLoad( musicplayerGO);
      musiclist();
      videoplayers = videoremote.AddCategory("video player list");
    }

    public void musiclist()
    {
      songs.ClearChildren();
      foreach (string musicFile in mp.GetMusicFiles())
      {
        string song = musicFile;
        string fileName = Path.GetFileName(song);
        songs.AddButton(fileName, "bd_audiofile", "Add " + fileName + " to playlist").OnPress += () =>
        {
            mp.StartCoroutine(mp.LoadAndAddSongToPlaylist(song));
            CohtmlHud.Instance.ViewDropTextImmediate("Added to playlist", fileName ?? "", "");
            playlistlist();
        };
      }
      playlistlist();
    }

    public void playlistlist()
    {
      playlistCategory.ClearChildren();
      foreach (AudioClip audioClip in mp.playlist)
      {
        AudioClip ac = audioClip;
        playlistCategory.AddButton(( ac).name, "bd_trash", "click " + ( ac).name + " to remove from playlist").OnPress += () =>
        {
            mp.playlist.Remove(ac);
            CohtmlHud.Instance.ViewDropTextImmediate("removed from playlist", (ac).name ?? "", "");
            playlistlist();
        };
      }
    }

    public void remotevideolist()
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (CVRVideoPlayer allVideoPlayer in new utils().GetAllVideoPlayers())
      {
        CVRVideoPlayer videoplayer = allVideoPlayer;
        string name = (videoplayer.transform.parent).name;
        if (!addedVideoPlayers.Contains(name))
        {
          stringSet.Add(name);
          Page page = videoplayers.AddPage(name ?? "", "bd_videoplayer", "controls for " + name, "bluedescriptor");
          page.ClearChildren();
          Category category = page.AddCategory("controls for " + name);
          category.AddButton("Play", "bd_playi", "Play video player").OnPress += () => videoplayer.Play();
          category.AddButton("Pause", "bd_pausei", "Pause video player").OnPress += () => videoplayer.Pause();
          category.AddButton("Change URL", "bd_url", "change url").OnPress += () => QuickMenuAPI.OpenKeyboard("", v =>
          {
              videoplayer.SetUrl(v);
              videoplayer.Play();
          });
          MultiSelection multiSelection = new MultiSelection("audio mode", null, 0);
        }
      }
      foreach (string str in stringSet)
        addedVideoPlayers.Add(str);
      if (stringSet.Count <= 0)
        return;
      videoplayers.ClearChildren();
      foreach (CVRVideoPlayer allVideoPlayer in new utils().GetAllVideoPlayers())
      {
        CVRVideoPlayer videoplayer = allVideoPlayer;
        string name = (videoplayer.transform.parent).name;
        if (addedVideoPlayers.Contains(name))
        {
          Page page = videoplayers.AddPage(name ?? "", "bd_videoplayer", "Controls for " + name, "bluedescriptor");
          page.ClearChildren();
          Category category = page.AddCategory("Controls for " + name);
          category.AddButton("Play", "bd_playi", "Play video player").OnPress += () => videoplayer.Play();
          category.AddButton("Pause", "bd_pausei", "Pause video player").OnPress += () => videoplayer.Pause();
          category.AddButton("Change URL", "bd_url", "change url").OnPress += () => QuickMenuAPI.OpenKeyboard("", v =>
          {
              videoplayer.SetUrl(v);
              videoplayer.Play();
          });
        }
      }
    }
  }
}
