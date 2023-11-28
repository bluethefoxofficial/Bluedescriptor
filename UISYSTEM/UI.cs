using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.UI;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System;
using ABI_RC.Systems.GameEventSystem;
using ABI.CCK.Components;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.VideoPlayer.Scripts;
using ABI_RC.Core.Util.AssetFiltering;
using static ABI_RC.Systems.GameEventSystem.CVRGameEventSystem;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Player;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UI  : MelonMod
    {
        private Page bluedescriptorpage;
        GameObject audioManagerObject;
        AudioManager audioManager;
        UIListCreator lis;
        GameObject musicplayerGO;
        MusicPlayer mp;
        Category songs;
        Page videoremote;
        Page playlistPage;
        Category playlistCategory;
        moddingnetwork mn;
        Category videoplayers;
        public ThemeEngine the;
   


        public List<string[]> plist = new List<string[]>();
      //  bool rainbowhudv = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud");
        bool uisounds = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "uisounds");
        bool playerlistval = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "playerlist");
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            plist.Clear();
           
        }
        bool firstload = false;

        public void uiinit()
        {
            if (MelonLoader.MelonMod.RegisteredMelons.FirstOrDefault(m => m.Info.Name == "BTKUILib") != null)
                menuinit();

            CVRGameEventSystem.World.OnUnload.AddListener(wi =>
{ 
                     plist.Clear();
                     lis.applylist(plist);
                    
             });
        


            CVRGameEventSystem.Spawnable.OnInstantiate.AddListener((s, item) =>
            {
                remotevideolist();
                
            });
            CVRGameEventSystem.Spawnable.OnDestroy.AddListener((s, item) =>
            {
                remotevideolist();

            });
            CVRGameEventSystem.Instance.OnDisconnected.AddListener( ins =>
            {
                remotevideolist();
               
            });
           
            CVRGameEventSystem.Instance.OnConnected.AddListener(ins =>
            {
             
                mn.SendMessageToAll(MetaPort.Instance.ownerId);
                remotevideolist();
            });
     
            //open ui
            CVRGameEventSystem.QuickMenu.OnOpen.AddListener(() =>
            {
               
                 if(uisounds)  audioManager.PlayAudio("leave.wav");
                try
                {
                    if (playerlistval)
                    {
                        lis.Show();
                        lis.resetlistui();
                    }
                }catch (Exception ex)
                {

                }

            });

        
            //close ui
            CVRGameEventSystem.QuickMenu.OnClose.AddListener(() =>
            {

                if (uisounds)
                {
                     if(uisounds)  audioManager.PlayAudio("close.wav");
                }
                if (playerlistval)
                {
                    lis.resetlistui();
                    lis.Hide();
                }
                

            });
            /* ither */
            CVRGameEventSystem.Instance.OnConnectionLost.AddListener(ins =>
            {
    if(uisounds)  audioManager.PlayAudio("conerror.wav");
     
            });

            CVRGameEventSystem.Instance.OnConnected.AddListener(ins => {
                remotevideolist();
                audioManagerObject = new GameObject("AudioManagerObject");
                audioManager = audioManagerObject.AddComponent<AudioManager>();
                if (firstload) return;
                lis.GenerateUICanvas();
               
                firstload = true;
            
            });
            CVRGameEventSystem.Home.OnJoin.AddListener(() =>
            {
                remotevideolist();
             audioManagerObject = new GameObject("AudioManagerObject");
            audioManager = audioManagerObject.AddComponent<AudioManager>();

            
                if (firstload) return;
                lis.GenerateUICanvas();

                firstload = true;

            });

        }
      
        GameObject corountinemgr = new GameObject();

       

        public void menuinit()

        { 
            corountinemgr.AddComponent<Classes.CoroutineManager>();
            
            bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage", true, "bd_logo");

            the = new ThemeEngine();

            var profilecat = bluedescriptorpage.AddCategory("Blue Descriptor Configurator");

             lis = new UIListCreator();
         


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
       
            var panicbutton = general.AddButton("Panic","bd_warn","removes all shaders from all avatars");
            panicbutton.OnPress += () =>
            {
                new UIfunctions().panic();
            };
            var themer = general.AddPage("Theme engine", "bd_themes","Theme engine","bluedescriptor");
           var mts= themer.AddCategory("");
            var installedthemes = themer.AddCategory("installed themes");
           mts.AddButton("refresh theme list", "bd_reconnect","refresh theme list").OnPress += () => {
                installedthemes.ClearChildren();
                if (the.GetInstalledThemes() != null)
                {
                    foreach (string theme in the.GetInstalledThemes())
                    {
                        installedthemes.AddButton("apply " + theme, "bd_themes", $"apply {theme}").OnPress += () =>
                        {
                            the.ApplyThemeQM(theme);
                            MelonPreferences.SetEntryValue("Bluedescriptor", "appliedtheme", theme);
                            MelonPreferences.Save();
                        };
                    }

                }
            };
        


            if (the.GetInstalledThemes() != null)
            {
                foreach (string theme in the.GetInstalledThemes())
                {
                    installedthemes.AddButton("apply " + theme, "bd_themes", $"apply {theme}").OnPress += () =>
                    {
                        the.ApplyThemeQM(theme);
                        MelonPreferences.SetEntryValue("Bluedescriptor", "appliedtheme", theme);
                        MelonPreferences.Save();
                    };
                }

            }



            var downloadablethemes = themer.AddCategory("Online theme Downloader.");

            var antitoxic = general.AddPage("Antitoxin","bd_antitoxic","disconnect when you lag","bluedescriptor");
            antitoxic.AddSlider("wip", "this will allow you to adjust fps when it is complete", 10.0f, 2f, 20f);
            var attoggles = antitoxic.AddCategory("Antitoxic toggles");
            attoggles.AddToggle("autopanic", "autopanic", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "at_autopanic")).OnValueUpdated += val =>
            {
                
                MelonPreferences.SetEntryValue("Bluedescriptor", "at_autopanic",val);
                switch (val)
                {
                    case true:
                        MelonLogger.Msg("autopanic enabled");
                        CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Safety", "Autopanic is enabled");
                        break;

                    default:
                        MelonLogger.Msg("autopanic disabled");
                        CohtmlHud.Instance.ViewDropTextImmediate($"Blue Descriptor", $"Blue Descriptor Safety", "Autopanic is disabled");
                        break;
                }
            };

            var rejoin = general.AddButton("Rejoin current instance", "bd_reconnect", "reconnect");
            rejoin.OnPress += () => {

        BTKUILib.QuickMenuAPI.ShowConfirm("You sure","this action will force you to rejoin which can annoy people, " +
            "only do so if you think you need to, continue", () => {}, () => { });
 };


            var uisettings = general.AddPage("global ui settings", "bd_uisetup", "global seyimgs for BD UI","bluedescriptor");
            var soundfx  = uisettings.AddCategory("UI sound fx");
            soundfx.AddToggle("BD UI sounds", "enable/disable ui sounds", uisounds).OnValueUpdated += v =>
            {
                MelonLogger.Msg($"Saving UI sounds preference: {v}");
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "uisounds", v);
                MelonPreferences.Save();
                uisounds = v;
                MelonLogger.Msg("UI sounds preference saved");
            };

            var playerlist = uisettings.AddCategory("Playerlist settings");
            playerlist.AddToggle("playerlist toggle", "Enable/Disable playerlist", MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "playerlist")).OnValueUpdated += v =>
            {
                MelonLogger.Msg($"Saving player list preference: {v}");
                MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "playerlist", v);
                MelonPreferences.Save();
                if (v)
                {
                    lis.Show();
                }
                else
                {
                    lis.Hide();
                }
                playerlistval = v;
                MelonLogger.Msg("Player list preference saved");
            };


            var musicplayer = general.AddPage("music+", "bd_musicplus", "music player controller", "bluedescriptor");

            musicplayer.AddSlider("Volume", "Song volume", 1, 0, 1).OnValueUpdated += v =>
            {

                mp.SetVolume(v);
                playlistlist();

            };

            var seek = musicplayer.AddSlider("", "", 0, 0, 0);
            seek.OnValueUpdated += v =>
            {
                mp.SetPlaybackPosition(v);
            };

            var music_controller = musicplayer.AddCategory("Music + controller");

            music_controller.AddButton("Play", "bd_playi", "continue playing").OnPress += () =>
            {
                mp.Play();
                seek.MaxValue = 0;
                seek.MaxValue = mp.GetTotalDuration();
                mp.OnPlaybackTimeUpdate += v =>
                {
                    seek.MaxValue = mp.GetTotalDuration();
                    seek.SetSliderValue(v);
                };
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate($"Playing", $"{mp.CurrentlyPlaying()}", "");
            };
            music_controller.AddButton("Pause", "bd_pausei", "Pause current track").OnPress += () =>
            {
                mp.Pause();
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate($"Paused", $"", "");
            };
            music_controller.AddButton("Previous", "bd_r", "<<").OnPress += () =>
            {
                seek.MaxValue = 0;
                seek.MaxValue = mp.GetTotalDuration();
                mp.PlayPrevious();
                playlistlist();
                seek.SetSliderValue(0);
                CohtmlHud.Instance.ViewDropTextImmediate($"Now playing", $"{mp.CurrentlyPlaying()}", "");
            };
            music_controller.AddButton("Next", "bd_ff", ">>").OnPress += () =>
            {

                mp.PlayNext();
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate($"Now playing", $"{mp.CurrentlyPlaying()}", "");
            };
            music_controller.AddButton("stop", "bd_stopi", "stop all music").OnPress += () =>
            {
                seek.MaxValue = 0;
                seek.MaxValue = mp.GetTotalDuration();
                mp.audioSource.Stop();
                playlistlist();
                seek.SetSliderValue(0);
                mp.currentTrackIndex = 0;
                CohtmlHud.Instance.ViewDropTextImmediate($"STOPPED", $"", "");
            };
            
            music_controller.AddButton("Clear Playlist", "bd_trash", "clear playlist").OnPress += () =>
            {
                seek.MaxValue = 0;
                seek.MaxValue = mp.GetTotalDuration();
                mp.playlist.Clear();
                mp.audioSource.Stop();
                seek.SetSliderValue(0);
                playlistlist();
                CohtmlHud.Instance.ViewDropTextImmediate($"Cleared", $"", "");
            };
            music_controller.AddToggle("Loop","toggle loop on and off",false).OnValueUpdated += val => {
                mp.audioSource.loop = val ;
                playlistlist();
            };
            music_controller.AddButton("refresh", "bd_reconnect", "refresh songs in folder list.").OnPress += () =>
            {
                musiclist();
                playlistlist();
            };

            playlistPage = music_controller.AddPage("playlist", "bd_playlist", "show playlist structure/edit","bluedescriptor");
            playlistCategory = playlistPage.AddCategory("current playlist");
            
            songs = musicplayer.AddCategory("==songs in folder=============");
            /* portable video player */
            var vp = general.AddPage("portable video player", "bd_videoplayer", "Portable video player configuration", "bluedescriptor");
            var vpg = vp.AddCategory("video player configuration");

          var spawn_videoplayer =   vpg.AddButton("Spawn player", "bd_videoplayer", "spawn a portable player");
          var destroy_videoplayer =   vpg.AddButton("destroy player", "bd_videoplayer", "destroys the video player");

            spawn_videoplayer.OnPress += () =>
            {
                if(GameObject.Find("BD VP"))
                {
                    CohtmlHud.Instance.ViewDropText("cant spawn?", "you can only have one portable video player");
                    return;
                }
                // Create a new GameObject for the video player
                var player = new GameObject("BD VP");
                player.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
                // Create a temporary cube to copy its mesh
                GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // Copy the mesh from the temporary cube to the player
                MeshFilter playerMeshFilter = player.AddComponent<MeshFilter>();
                playerMeshFilter.mesh = tempCube.GetComponent<MeshFilter>().mesh;
                // Create a new RenderTexture with a resolution of 1920x1080
                RenderTexture rt = new RenderTexture(1920, 1080, 24); // 24 is a common depth value
                rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
                rt.enableRandomWrite = true;
                rt.wrapMode = TextureWrapMode.Clamp;
                
                rt.format = RenderTextureFormat.RGB111110Float;
                // Optionally, copy the renderer and material if needed
                player.AddComponent<MeshRenderer>().material = tempCube.GetComponent<MeshRenderer>().material;

                // Clean up: Destroy the temporary cube
                GameObject.Destroy(tempCube);

                var ui = new GameObject("UI");
                ui.transform.SetParent(player.transform, false);
                // Get the current position of the local player
                Vector3 currentPlayerPosition = new CVRPlayer().Localplayer().transform.position;
                currentPlayerPosition.y = 0.8f;
                Quaternion currentplayerrot = new CVRPlayer().Localplayer().transform.rotation;
                currentplayerrot.z = 0;

                // Set the video player's position to the current player position
                player.transform.localPosition = currentPlayerPosition;
         
                // Add the CVRVideoPlayer component and set its UI position
                var videoPlayerComponent = player.AddComponent<CVRVideoPlayer>();
                videoPlayerComponent.videoPlayerUIPosition = ui.transform;
                videoPlayerComponent.interactiveUI = true;
                videoPlayerComponent.ProjectionTexture = rt;
           GameObject aus =  new GameObject("ouyput");
                aus.AddComponent<AudioSource>();
                aus.transform.SetParent(player.transform);

                videoPlayerComponent.customAudioSource = aus.GetComponent<AudioSource>();
                videoPlayerComponent.audioPlaybackMode = ABI_RC.VideoPlayer.Scripts.VideoPlayerUtils.AudioMode.Direct;
                videoPlayerComponent.syncEnabled = false;
                videoPlayerComponent.currentlySelectedPlaylist = new CVRVideoPlayerPlaylist();

                SharedFilter.SanitizeUnityEvents("CVRVideoPlayer", videoPlayerComponent.startedPlayback);
                SharedFilter.SanitizeUnityEvents("CVRVideoPlayer", videoPlayerComponent.finishedPlayback);
                SharedFilter.SanitizeUnityEvents("CVRVideoPlayer", videoPlayerComponent.pausedPlayback);
                SharedFilter.SanitizeUnityEvents("CVRVideoPlayer", videoPlayerComponent.setUrl);
          
                player.AddComponent<CVRPickupObject>();
                player.AddComponent<BoxCollider>().isTrigger = true;

            };

            destroy_videoplayer.OnPress += () =>
            {
                GameObject videoPlayerObject = GameObject.Find("BD VP");
                if (videoPlayerObject != null)
                {
                    foreach (Transform child in videoPlayerObject.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            };
            /* video player remote */

            videoremote = general.AddPage("video player controller","bd_videoplayer","video player controller.","bluedescriptor");
            videoplayers = videoremote.AddCategory("video player list");

            var recordsystem = new ScreenRecorder();
            var recorder = general.AddPage("recorder","bd_ir","screen recorder","bluedescriptor");

            var recordercat = recorder.AddCategory("recorder");

           var recordbtn =recordercat.AddButton("record", "bd_ir","start a recording");
           var stoprecordbtn =recordercat.AddButton("stop", "bd_stoprec","stop a recording");
            var openrecordingsfolder = recordercat.AddButton("open recordings folder", "bd_recfol","open recordings folder in explorer");

            var optim = general.AddPage("optim configure", "bd_optim", "optimisation settings", "bluedescriptor");

            optim.AddSlider("cull distance", "set autocull distance",50,0,256);

            var kannashell = general.AddPage("VRC Transitioner", "bd_vrc", "Ease the Transition.","bluedescriptor");
           var catonal= kannashell.AddCategory("transitional settings");
            catonal.AddButton("Download and apply catonal themes.", "","Download and apply themes to emulate the vrchat old style.");
         


            recordbtn.OnPress += () =>
            {
                CoroutineRunner.Instance.StartRoutine(recordsystem.StartRecording());
            };

            stoprecordbtn.OnPress += () =>
            {
                CoroutineRunner.Instance.StartRoutine(recordsystem.StopRecording());
            };
            openrecordingsfolder.OnPress += () =>
            {                                            
                Process.Start(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\bluedescriptor\\recordings\\");
            };



            lis.applylist(plist);
     

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

                if (UIfunctions.PlayerHasModInstalled(pl.Uuid))
                {
                    UIfunctions.ShowModIconOnNameplate(pl);
                }

                // MelonLogger.Warning(Friends.FriendsWith(pl.PlayerDescriptor.ownerId).ToString());
                plist.Add(new string[] { pl.Username, Friends.FriendsWith(pl.PlayerDescriptor.ownerId).ToString(), pl.Uuid });

                lis.applylist(plist);
                
                lis.resetlistui();
                new UIfunctions().OnPlayerJoin(pl);
             
                MelonLogger.Msg(pl.Username + " Joined your lobby");

                try
                {
                    if (Friends.FriendsWith(pl.PlayerDescriptor.ownerId))
                    {
                        if (uisounds) audioManager.PlayAudio("join.wav");
                    }
                }
                catch
                {

                }
                
              
            };
          
            BTKUILib.QuickMenuAPI.UserLeave += pl =>
            {
          
                        MelonLogger.Msg(pl.Username + " left your lobby");

                if (Friends.FriendsWith(pl.PlayerDescriptor.ownerId))
                {
                    if (uisounds) audioManager.PlayAudio("leave.wav");
                }

                plist.RemoveAll(item => item.Contains(pl.Username));
                lis.applylist(plist);
                lis.resetlistui();
             

            };
            if (MelonPreferences.GetEntryValue<string>("Bluedescriptor", "appliedtheme").Length > 0)
            {
                try
                {
                    the.ApplyThemeQM(MelonPreferences.GetEntryValue<string>("Bluedescriptor", "appliedtheme"));
                }
                catch { }
            }
            MelonLogger.Msg(System.Drawing.Color.DarkBlue, $"\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@      @@@@@@           @@@@@@@@,                    @@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@   @@@@@@                @@@@@@,                       @@@@@@@@@@@@\r\n@@@@@@@@@@@@@(@@@@@   *@@@@@@         (@@@@@,                         @@@@@@@@@@\r\n@@@@@@@@@@@@@@@@      *@@@@@@,        @@@@@@,        (@@@@@@@         @@@@@@@@@@\r\n@@@@@@@@@@@@@/                       @@@@@@@,        (@@@@@@@&         @@@@@@@@@\r\n@@@@@@@@@@@@@                       @@@@@@@@@@@@@@@@@@@@@@@@@@         @@@@@@@@@\r\n@@@@@@@@@@@@@                          @@@@@,        (@@@@@@@&         @@@@@@@@@\r\n@@@@@@@@@@@@@         *@@@@@@@@         @@@@,        (@@@@@@@          @@@@@@@@@\r\n@@@@@@@@@@@@@         *@@@@@@@          @@@@,        (@@@@@           @@@@@@@@@@\r\n@@@@@@@@@@@@@                          @@@@@,                       (@@@@@@@@@@@\r\n@@@@@@@@@@@@@                        @@@@@@@,                     @@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

        }


        public void musicplayerinit()
        {
          musicplayerGO = new GameObject();

      mp  = musicplayerGO.AddComponent<MusicPlayer>();
            musicplayerGO.AddComponent<AudioSource>();
            mp.Awake(); //initilize the player
                        // Prevent the music player GameObject from being destroyed on scene load
            UnityEngine.Object.DontDestroyOnLoad(musicplayerGO);

            musiclist();
            videoplayers = videoremote.AddCategory("video player list");

        }

        public void musiclist()
        {
            songs.ClearChildren();
            foreach (string song in mp.GetMusicFiles())
            {
                string fileName = Path.GetFileName(song);
                songs.AddButton(fileName, "bd_audiofile", $"Add {fileName} to playlist").OnPress += () =>
                {
                    mp.StartCoroutine(mp.LoadAndAddSongToPlaylist(song));
                    CohtmlHud.Instance.ViewDropTextImmediate($"Added to playlist", $"{fileName}", "");
                    playlistlist();
                };
            }
            playlistlist();
        }

        public void playlistlist()
        {
            playlistCategory.ClearChildren();
            foreach(AudioClip ac in mp.playlist)
            {
                playlistCategory.AddButton(ac.name, "bd_trash", $"click {ac.name} to remove from playlist").OnPress += () =>
                {
                    mp.playlist.Remove(ac);
                    CohtmlHud.Instance.ViewDropTextImmediate($"removed from playlist", $"{ac.name}", "");
                    playlistlist();
                };
            }
        }
        HashSet<string> addedVideoPlayers = new HashSet<string>();


        public void remotevideolist()
        {
            // Assuming addedVideoPlayers is a HashSet or some other collection that prevents duplicates
            HashSet<string> newAddedVideoPlayers = new HashSet<string>();

            // Go through all video players
            foreach (CVRVideoPlayer videoplayer in new utils().GetAllVideoPlayers())
            {
                string videoPlayerName = videoplayer.transform.parent.name;

                // Check if the video player has already been added
                if (addedVideoPlayers.Contains(videoPlayerName))
                {
                    continue;
                }

                // Add the new video player to the temporary new set
                newAddedVideoPlayers.Add(videoPlayerName);

                var vp = videoplayers.AddPage($"{videoPlayerName}", "bd_videoplayer", $"controls for {videoPlayerName}", "bluedescriptor");
                vp.ClearChildren();
                var cat = vp.AddCategory($"controls for {videoPlayerName}");
                
                cat.AddButton("Play", "bd_playi", "Play video player").OnPress += () =>
                {
                    videoplayer.Play();
                };
                cat.AddButton("Pause", "bd_pausei", "Pause video player").OnPress += () =>
                {
                    videoplayer.Pause();
                };
                cat.AddButton("Change URL", "bd_url", "change url").OnPress += () =>
                {
                    BTKUILib.QuickMenuAPI.OpenKeyboard("", v =>
                    {
                        videoplayer.SetUrl(v);
                        videoplayer.Play();

                    });
                };
                new BTKUILib.UIObjects.Objects.MultiSelection("audio mode", null, 0);
            }

            // Now update the main addedVideoPlayers collection with the new additions
            foreach (var videoPlayerName in newAddedVideoPlayers)
            {
                addedVideoPlayers.Add(videoPlayerName);
            }

            if (newAddedVideoPlayers.Count > 0)
            {
                // Clear any existing children UI elements
                videoplayers.ClearChildren();

                // Iterate over all video players
                foreach (CVRVideoPlayer videoplayer in new utils().GetAllVideoPlayers())
                {
                    string videoPlayerName = videoplayer.transform.parent.name;

                    // Check if the video player is in the updated addedVideoPlayers set
                    if (addedVideoPlayers.Contains(videoPlayerName))
                    {
                        // Add a new page for this video player
                        var vp = videoplayers.AddPage($"{videoPlayerName}", "bd_videoplayer", $"Controls for {videoPlayerName}", "bluedescriptor");
                        vp.ClearChildren();
                        // Add a category for the controls
                        var cat = vp.AddCategory($"Controls for {videoPlayerName}");

                        // Add Play, Pause, and Change URL buttons with their respective actions
                        cat.AddButton("Play", "bd_playi", "Play video player").OnPress += () =>
                        {
                            videoplayer.Play();
                        };
                        cat.AddButton("Pause", "bd_pausei", "Pause video player").OnPress += () =>
                        {
                            videoplayer.Pause();
                        };
                        cat.AddButton("Change URL", "bd_url", "change url").OnPress += () =>
                        {
                            BTKUILib.QuickMenuAPI.OpenKeyboard("", v =>
                            {
                                videoplayer.SetUrl(v);
                                videoplayer.Play();
                            });
                        };
                     
                     
                    }
                }
            }
        }
  }
}
