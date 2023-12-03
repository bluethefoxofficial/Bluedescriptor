using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class CoroutineRunner : MonoBehaviour
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600000F RID: 15 RVA: 0x00002280 File Offset: 0x00000480
	public static CoroutineRunner Instance
	{
		get
		{
			bool flag = CoroutineRunner._instance == null;
			if (flag)
			{
				GameObject gameObject = new GameObject("CoroutineRunner");
				CoroutineRunner._instance = gameObject.AddComponent<CoroutineRunner>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return CoroutineRunner._instance;
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000022C5 File Offset: 0x000004C5
	public void StartRoutine(IEnumerator routine)
	{
		base.StartCoroutine(routine);
	}

	// Token: 0x04000006 RID: 6
	private static CoroutineRunner _instance;
}
