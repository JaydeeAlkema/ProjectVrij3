using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventFromParent : MonoBehaviour
{
	public UnityEvent[] unityEvents;

	public void CallEvent(int eventInArray)
	{
		unityEvents[eventInArray].Invoke();
	}
}
