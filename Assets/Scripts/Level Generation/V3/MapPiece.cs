using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
	[SerializeField] private Transform connectionPointsParent = null;
	[SerializeField] private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
	[SerializeField] private List<GameObject> neighbours = new List<GameObject>();

	public void GetConnectionPointsFromChildren()
	{
		foreach (Transform childTransform in connectionPointsParent.GetComponentsInChildren<Transform>())
		{
			ConnectionPoint connectionPoint = childTransform.GetComponent<ConnectionPoint>();
			if (connectionPoint != null)
			{
				connectionPoints.Add(connectionPoint);
			}
		}
	}

	public void AddNeighbour(GameObject neighbour)
	{
		if (neighbours.Contains(neighbour) == false && neighbour != null)
		{
			neighbours.Add(neighbour);
		}
	}
}
