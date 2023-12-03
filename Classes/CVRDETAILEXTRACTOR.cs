
using System.Collections.Generic;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class CVRDETAILEXTRACTOR
    {
        public GameObject[] objector()
        {
            var gameObjectList = new List<GameObject>();

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if ((gameObject).hideFlags == 0)
                    gameObjectList.Add(gameObject);
            }

            return gameObjectList.ToArray();
        }
    }
}