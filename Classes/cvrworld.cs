
using ABI.CCK.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class cvrworld
  {
    public GameObject[] gameobjectsinscene(string scene)
    {
      GameObject.FindObjectsOfType<GameObject>();
      Scene sceneByName = SceneManager.GetSceneByName(scene);
      return (sceneByName).GetRootGameObjects();
    }

    public CVRWorld currentworld() => CVRWorld.Instance;

    public Scene[] getallscenesingame()
    {
      int sceneCount = SceneManager.sceneCount;
      Scene[] sceneArray = new Scene[sceneCount];
      for (int index = 0; index < sceneCount; ++index)
        sceneArray[index] = SceneManager.GetSceneAt(index);
      return sceneArray;
    }
  }
}
