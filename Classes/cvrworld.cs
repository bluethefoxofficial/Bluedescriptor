using ABI.CCK.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bluedescriptor_Rewritten.Classes
{
    // public List<Scene> scenes = new List<Scene>();
    internal class cvrworld
    {
        public GameObject[] gameobjectsinscene(string scene)
        {
            var list = GameObject.FindObjectsOfType<GameObject>();
            // gets see all objects in scene from string scene
            var sceneobj = SceneManager.GetSceneByName(scene);
            list = sceneobj.GetRootGameObjects();
            return list;
        }

        /*
         * 
         * get all scenes in game ()
         * 
         */

        public CVRWorld currentworld() => CVRWorld.Instance;

        public Scene[] getallscenesingame()
        {
            // todo: potentially find a less shit way of doing this.
            var scenecount = SceneManager.sceneCount;
            var scenes = new Scene[scenecount];

            for (int i = 0; i < scenecount; i++)

                scenes[i] = SceneManager.GetSceneAt(i);

            return scenes;
        }
    }
}