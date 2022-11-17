using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineCaller : MonoBehaviour
{
	public static CoroutineCaller CallerInstance;
	// Start is called before the first frame update
	void Awake()
	{
		CallerInstance = this;
	}

	public void CallCoroutine(IEnumerator coroutineName)
	{
		StartCoroutine(coroutineName);
	}

	public void CancelCoroutine(IEnumerator coroutineName)
	{
		StopCoroutine(coroutineName);
	}
}
