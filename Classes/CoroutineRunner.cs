using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var coroutineRunnerObject = new GameObject("CoroutineRunner");
                _instance = coroutineRunnerObject.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(coroutineRunnerObject);
            }

            return _instance;
        }
    }

    public void StartRoutine(IEnumerator routine) => StartCoroutine(routine);
}