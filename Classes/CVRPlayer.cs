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

        public float[] localplayerposition()
        {
            GameObject player = Localplayer();
            float[] nullified = new float[3];
            if (player == null) return nullified;
            float[] position = new float[3];
            position[0] = (float)player.transform.position.x;
            position[1] = (float)player.transform.position.y;
            position[2] = (float)player.transform.position.z;

            return position;
        }
    }
}
