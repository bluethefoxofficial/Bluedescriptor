using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bluedescriptor_Rewritten.Classes
{
   // public List<Scene> scenes = new List<Scene>();
    internal class cvrworld
    {
        public GameObject[] gameobjectsinscene(string scene)
        {

            GameObject[] list = GameObject.FindObjectsOfType<GameObject>();
            //gets see all objects in scene from string scene
          Scene sceneobj =   SceneManager.GetSceneByName(scene);
           list = sceneobj.GetRootGameObjects();
            return list;
        }


        /*
         * 
         * get all scenes in game ()
         * 
         */

        public Scene[] getallscenesingame()
        {
            //todo: potentially find a less shit way of doing this.
           int scenecount =  SceneManager.sceneCount;
            Scene[] scenes = new Scene[scenecount];

            for(int i = 0; i < scenecount; i++)
            {

                scenes[i] = SceneManager.GetSceneAt(i);

            }
           
            return scenes;
        }

    }
}
