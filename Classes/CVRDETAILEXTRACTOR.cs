using System.Collections.Generic;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class CVRDETAILEXTRACTOR
    {
        public GameObject[] objector()
        {
            var list = new List<GameObject>();

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (gameObject.hideFlags == HideFlags.None)
                {
                    list.Add(gameObject);
                }
            }

            return list.ToArray();
        }
    }
}