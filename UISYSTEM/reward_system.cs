using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bluedescriptor_Rewritten.Classes;
using MelonLoader;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class reward_system
    {

        public void rewardinit()
        {
            //on player join check player username for it to be one of the team.
            BTKUILib.QuickMenuAPI.UserJoin += pl =>
            {
                if (pl.Username == "bluethefox")
                {
                    MelonPreferences.SetEntryValue("Bluedescriptor", "YOUMETBLUE", true);
                   // new Audio().PlayAudio("Bluedescriptor_Rewritten.Resources.bluethefox.ogg", "bluethefox");
                }

            };

            //checks players leaving
            BTKUILib.QuickMenuAPI.UserLeave += pl =>
            {


            };




        }
    }
}
