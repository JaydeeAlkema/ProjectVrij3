using UnityEngine;

public class DeadEnd : MonoBehaviour
{
	[SerializeField] private GameObject objectsToEnableParent = null;
	[SerializeField] private GameObject objectsToDisableParent = null;

	public void Enable()
	{
		objectsToEnableParent.SetActive(true);
		objectsToDisableParent.SetActive(false);
	}
}
