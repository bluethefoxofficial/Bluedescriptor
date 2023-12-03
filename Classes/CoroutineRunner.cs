using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (CoroutineRunner._instance == null)
            {
                var gameObject = new GameObject(nameof(CoroutineRunner));
                CoroutineRunner._instance = gameObject.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(gameObject);
            }

            return CoroutineRunner._instance;
        }
    }

    public void StartRoutine(IEnumerator routine) => StartCoroutine(routine);
}