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
		ResetValue();
	}

	private void OnEnable()
	{
		ResetValue();
	}

	private void OnDisable()
	{
		ResetValue();
	}

	public void ResetValue()
	{
		value = startValue;
	}
}
