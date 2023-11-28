using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class CVRDETAILEXTRACTOR
    {
        public GameObject[] objector()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (gameObject.hideFlags == HideFlags.None)
                {
                    list.Add(gameObject);
                }
            }
            return  list.ToArray();
        }
    }
}
