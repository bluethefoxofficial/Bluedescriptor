using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using cohtml;
using ABI_RC.Core.Networking.IO.Global;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UI  : MelonMod
    {
        private Page bluedescriptorpage;
        private bool vrcnameplate = true;
        private List<CVRPlayerEntity> plist = new List<CVRPlayerEntity>();
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            plist.Clear();
        }
        void OnPlayerJoin(CVRPlayerEntity player)
        {
            /*
             * 
             * 
             * vrchat nameplate system
             * 
             */
            if (vrcnameplate)
            {
                try
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    byte[] buffer = new byte[255];
                    using (Stream stream = asm.GetManifestResourceStream("Bluedescriptor_Rewritten.res.Nameplates.VRC.Visitor.png"))
                    {
                        buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    Texture2D tex = new Texture2D(256, 256);
                    tex.LoadImage(buffer);
                    Sprite spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    player.PlayerNameplate.nameplateBackground.sprite = spr;
                    player.PlayerNameplate.nameplateBackground.color = new UnityEngine.Color32(0,0,0,128);
                    
                }
                catch(System.Exception ex)
                {
                    MelonLogger.Msg("[BD ERROR DETECTED]========= " +ex + "\n" + ex.Message);
                }
        }
}

        public void menuinit()
        {
            bluedescriptorpage = new Page("Bluedescriptor", "Bluedescriptorpage",true, "bd_logo");
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
