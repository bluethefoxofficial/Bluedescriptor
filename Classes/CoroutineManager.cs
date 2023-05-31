using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public class CoroutineManager : MonoBehaviour
    {
        private Dictionary<string, Coroutine> coroutineDictionary;

        void Awake()
        {
            coroutineDictionary = new Dictionary<string, Coroutine>();
        }

        public void StartManagedCoroutine(string coroutineName, IEnumerator coroutine)
        {
            if (coroutineDictionary.ContainsKey(coroutineName))
            {
                StopManagedCoroutine(coroutineName);
            }

            Coroutine newCoroutine = StartCoroutine(coroutine);
            coroutineDictionary.Add(coroutineName, newCoroutine);
        }

        public void StopManagedCoroutine(string coroutineName)
        {
            if (coroutineDictionary.ContainsKey(coroutineName))
            {
                StopCoroutine(coroutineDictionary[coroutineName]);
                coroutineDictionary.Remove(coroutineName);
            }
        }
    }
}
