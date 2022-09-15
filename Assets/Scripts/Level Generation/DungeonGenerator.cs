using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[Header("Generation Configuration")]
	[SerializeField] private int seed = 0;
	[SerializeField, MinMaxSlider(5, 20)] private Vector2Int roomCount = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 20)] private Vector2Int roomSize = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 15)] private Vector2Int pathwayLength = new Vector2Int();
	[SerializeField, MinMaxSlider(1, 5)] private Vector2Int pathwaySize = new Vector2Int();

	private List<Vector2> walkerPathCoordinates = new List<Vector2>();
	private List<Vector2> walkerRoomCoordinates = new List<Vector2>();

	private void Start()
	{
		seed = seed == 0 ? Random.Range(0, int.MaxValue) : seed;

		Random.InitState(seed);
		StartCoroutine(GenerateLevelLayout());
	}

	/// <summary>
	/// This functions generates the layout of the dungeon. This is done with the help of a "walker"
	/// This walker will "walk" a path, everytime this walker reaches it target distance, it takes a turn and marks the tile as a room origin.
	/// After the walker is done walking it's path, every room origin tiles will get saved and used later on in the generator to generate rooms upon.
	/// </summary>
	private IEnumerator GenerateLevelLayout()
	{
		// Create the walker gameobject.
		GameObject walkerGO = new GameObject("Walker");
		walkerGO.transform.position = new Vector3(0, 0, 0);

		// Store the previous pathway direction so we dont get overlapping pathways.
		int previousPathwayDirection = 1;

		walkerRoomCoordinates.Add(walkerGO.transform.position);

		int finalRoomCount = Random.Range(roomCount.x, roomCount.y);
		for (int r = 0; r < finalRoomCount; r++)
		{
			// Decide which way the dungeon should start going in.
			// 1: Left, 2: Up, 3: Right, 4: Down.
			// Keep generating a direction as long as it's the same as the previous one.
			int newPathwayDirection = Random.Range(1, 5);
			while (newPathwayDirection == 1 && previousPathwayDirection == 3 || newPathwayDirection == 3 && previousPathwayDirection == 1 ||
				  newPathwayDirection == 2 && previousPathwayDirection == 4 || newPathwayDirection == 4 && previousPathwayDirection == 2)
			{
				newPathwayDirection = Random.Range(1, 5);
			}


			// Set coordinates direction.
			Vector3 newDirection = newPathwayDirection switch
			{
				1 => new Vector3(0, 0, 0),
				2 => new Vector3(0, 0, 90),
				3 => new Vector3(0, 0, 180),
				4 => new Vector3(0, 0, 270),
				_ => new Vector3(0, 0, 0),
			};
			Debug.Log(newDirection);

			// Store current direction into the previous direction variable.
			previousPathwayDirection = newPathwayDirection;

			walkerGO.transform.rotation = Quaternion.Euler(newDirection);

			// Decide the length of the pathway and walk in the previously set direction.
			int pathwayLength = Random.Range(this.pathwayLength.x, this.pathwayLength.y);
			for (int p = 0; p < pathwayLength; p++)
			{
				walkerGO.transform.position += walkerGO.transform.right;
				if (p == pathwayLength - 1)
				{
					walkerRoomCoordinates.Add(walkerGO.transform.position);
				}
				else
				{
					walkerPathCoordinates.Add(walkerGO.transform.position);
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		// Draw path
		if (walkerPathCoordinates.Count > 0)
		{
			foreach (Vector2 coordinate in walkerPathCoordinates)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawCube(coordinate, Vector2.one);
			}
		}

		// Draw room origin points.
		if (walkerRoomCoordinates.Count > 0)
		{
			foreach (Vector2 coordinate in walkerRoomCoordinates)
			{
				if (coordinate == walkerRoomCoordinates[0])
				{
					Gizmos.color = Color.green;
				}
				else if (coordinate == walkerRoomCoordinates[walkerRoomCoordinates.Count - 1])
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.blue;
				}

				Gizmos.DrawCube(coordinate, Vector2.one);
			}
		}
	}
}