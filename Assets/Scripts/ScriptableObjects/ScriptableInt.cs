using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Int", menuName = "ScriptableObjects/New Scriptable Int")]
public class ScriptableInt : ScriptableObject
{
	public int value;
	public bool resetOnDestroy = false;
	[ShowIf("resetOnDestroy")] public int startValue;

	private void OnDestroy()
	{
		value = startValue;
	}

	private void OnEnable()
	{
		value = startValue;
	}

	private void OnDisable()
	{
		value = startValue;
	}
}
