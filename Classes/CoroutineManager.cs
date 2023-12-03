using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public class CoroutineManager : MonoBehaviour
    {
        Dictionary<string, Coroutine> coroutineDictionary;

        void Awake() => coroutineDictionary = new Dictionary<string, Coroutine>();

        public void StartManagedCoroutine(string coroutineName, IEnumerator coroutine)
        {
            if (coroutineDictionary.ContainsKey(coroutineName))
                StopManagedCoroutine(coroutineName);

            var coroutine1 = StartCoroutine(coroutine);
            coroutineDictionary.Add(coroutineName, coroutine1);
        }

        public void StopManagedCoroutine(string coroutineName)
        {
            if (!coroutineDictionary.ContainsKey(coroutineName))
                return;

            StopCoroutine(coroutineDictionary[coroutineName]);
            coroutineDictionary.Remove(coroutineName);
        }
    }
}