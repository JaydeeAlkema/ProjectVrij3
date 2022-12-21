using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
	[SerializeField] private Transform connectionPointsParent = null;
	[SerializeField] private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
	[SerializeField] private List<GameObject> neighbours = new List<GameObject>();
	[SerializeField] private List<GameObject> statueSpawnPoints = new List<GameObject>();
	[SerializeField] private List<GameObject> breakableObjects = new List<GameObject>();
	[SerializeField] private int breakableObjectSpawnChance = 75;   // Percentage

	public List<ConnectionPoint> ConnectionPoints { get => connectionPoints; private set => connectionPoints = value; }
	public List<GameObject> Neighbours { get => neighbours; private set => neighbours = value; }
	public List<GameObject> StatueSpawnPoints { get => statueSpawnPoints; private set => statueSpawnPoints = value; }
	public List<GameObject> BreakableObjects { get => breakableObjects; set => breakableObjects = value; }

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

	public void Decorate()
	{
		foreach (GameObject breakableObject in breakableObjects)
		{
			int spawnChance = Random.Range(0, 100);
			if (spawnChance > breakableObjectSpawnChance)
			{
				breakableObject.SetActive(true);
			}
			else
			{
				breakableObject.SetActive(false);
			}
		}
	}
}
