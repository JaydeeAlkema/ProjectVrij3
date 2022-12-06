using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "ScriptableObjects/Room")]
public class ScriptableRoom : ScriptableObject
{
	[SerializeField, ShowAssetPreview] private GameObject prefab = null;

	public GameObject Prefab { get => prefab; set => prefab = value; }
}
