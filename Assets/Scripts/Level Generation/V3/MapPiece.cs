using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
	[SerializeField] private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
	[SerializeField] private List<GameObject> neighbours = new List<GameObject>();

	private void Awake()
	{
		GetConnectionPointsFromChildren();
	}

	private void GetConnectionPointsFromChildren()
	{
		foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>())
		{
			ConnectionPoint connectionPoint = childTransform.GetComponentInChildren<ConnectionPoint>();
			if (connectionPoint != null)
			{
				connectionPoints.Add(connectionPoint);
			}
		}
	}

	public void AddNeighbour(GameObject neighbour)
	{
		if (neighbours.Contains(neighbour) == false)
		{
			neighbours.Add(neighbour);
		}
	}
}
