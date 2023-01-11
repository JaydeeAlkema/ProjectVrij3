using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
	[SerializeField] private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
	[SerializeField] private List<GameObject> neighbours = new List<GameObject>();
	[SerializeField] private List<GameObject> statueSpawnPoints = new List<GameObject>();
	[SerializeField] private List<GameObject> breakableObjects = new List<GameObject>();
	[SerializeField] private int breakableObjectSpawnChance = 75;   // Percentage

	public List<ConnectionPoint> ConnectionPoints { get => connectionPoints; private set => connectionPoints = value; }
	public List<GameObject> Neighbours { get => neighbours; private set => neighbours = value; }
	public List<GameObject> StatueSpawnPoints { get => statueSpawnPoints; private set => statueSpawnPoints = value; }
	public List<GameObject> BreakableObjects { get => breakableObjects; set => breakableObjects = value; }

	public void AddNeighbour(GameObject neighbour)
	{
		if (neighbours.Contains(neighbour) == false && neighbour != null)
		{
			neighbours.Add(neighbour);
		}
	}

	public void Decorate()
	{
		if (breakableObjects.Count == 0) return;

		foreach (GameObject breakableObject in breakableObjects)
		{
			if (breakableObject == null)
			{
				Debug.Log($"<color=red>Array Element is null! Please delete this array element to avoid errors like this!</color>", gameObject);
				continue;
			}

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
