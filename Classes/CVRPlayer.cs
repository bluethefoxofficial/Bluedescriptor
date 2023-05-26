using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 

namespace Bluedescriptor_Rewritten.Classes
{
    internal class CVRPlayer
    {
        public UnityEngine.GameObject Localplayer()
        {
            return GameObject.Find("_PLAYERLOCAL");
        }

        public GameObject[] remotePlayers()
        {
            GameObject[] sceneobj = new cvrworld().gameobjectsinscene(new cvrworld().getallscenesingame()[0].name);

            // Initialize players array with the appropriate size
            GameObject[] players = new GameObject[sceneobj.Length];

            int num = 0;
            foreach (var obj in sceneobj)
            {
                // Check if obj contains CVRAvatar component, if so, add to the list
                if (obj.GetComponentInChildren<ABI.CCK.Components.CVRAvatar>() != null)
                {
                    players[num] = obj.gameObject;
                    num++;
                }
            }

            // Resize the players array to the actual number of players found
            System.Array.Resize(ref players, num);

            return players;
        }

        public int[] localplayerposition()
        {
            GameObject player = Localplayer();
            int[] nullified = new int[3];
            if (player == null) return nullified;
            int[] position = new int[3];
            position[0] = (int)player.transform.position.x;
            position[1] = (int)player.transform.position.y;
            position[2] = (int)player.transform.position.z;

            return position;
        }
    }
}
