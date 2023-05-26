using BTKUILib.UIObjects;
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using UnityEngine;


namespace Bluedescriptor_Rewritten.UISYSTEM
{
  
    internal class UI 
    {
        private Page bluedescriptorpage;
        public Page scenespage;


        LineRenderer[] markerss = new LineRenderer[0];

        public void menuinit()
        {
            new Icons().iconsinit();
            /*
             * 
             * 
             * Main settings page
             * Do things differently.
             * TODO: rewrite
             * 1 HOUR LATER: its made less bad
             */
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

            var general = bluedescriptorpage.AddCategory("General settings and features");

            var panicbutton = general.AddButton("Panic","bd_warn","removes all shaders from all avatars");

            panicbutton.OnPress += () =>
            {
                new UIfunctions().panic();
            };
            var loginmusictoggle = general.AddToggle("login background music", "Enable or disable background music on the login screen of chilloutvr", true);

        }
        public void gameobjectsui()
        {   


            if (GameObject.Find("BD_GW"))
            {
                BTKUILib.QuickMenuAPI.ShowAlertToast("Cant use this feature in this world.");
                MelonLogger.Msg("Game World detected");
                return;
            }


            var scenes = new cvrworld().getallscenesingame();
            var objs = new cvrworld().gameobjectsinscene(scenes[0].name);

            int labelpos = 100;
            GUI.BeginScrollView(new Rect(20, 65, 1000, 200), new Vector2(0, 0), new Rect(0, 0, 1000, 200), true, true);
            GUI.Label(new Rect(20, 70, 1000, 200), $"Scene Game objects");
            
            if(objs == null || objs.Length == 0) {
                GUI.Label(new Rect(20, labelpos, 1000, 200), $"NO GAME OBJECTS FOUND.");
            }

            foreach (var obj in objs)
            {
                GUI.Label(new Rect(20, labelpos, 1000, 200), $"Name: {obj.name} | Layer:  {obj.layer} | is static: {obj.isStatic}");

                labelpos += 20;

            }
         

            GUI.EndScrollView();
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
