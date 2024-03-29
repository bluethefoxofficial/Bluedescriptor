﻿using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject coroutineRunnerObject = new GameObject("CoroutineRunner");
                _instance = coroutineRunnerObject.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(coroutineRunnerObject);
            }
            return _instance;
        }
    }

    public void StartRoutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
