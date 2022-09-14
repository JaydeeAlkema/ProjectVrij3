using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour
{
	[SerializeField] private GameObject prefab = default;
	[SerializeField] private List<LevelPiece> neighbourLevelPieces = new List<LevelPiece>();
	[SerializeField] private int spawnChancePercentage = 0;
	[Space]
	[SerializeField, ReadOnly] private List<Transform> connectionPoints = new List<Transform>();

	public GameObject Prefab { get => prefab; set => prefab = value; }

	private void Awake()
	{
		connectionPoints.AddRange(transform.Find("Connection Points").GetComponentsInChildren<Transform>());
		connectionPoints.RemoveAt(0);   // Remove the first element, as this is always the parent of the children in the list.
	}
}
