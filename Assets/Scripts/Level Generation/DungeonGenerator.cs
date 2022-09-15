using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[Header("Generation Configuration")]
	[SerializeField] private int seed = 0;
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomCount = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomWidth = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomHeight = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int pathwayLength = new Vector2Int();
	[SerializeField, MinMaxSlider(1, 5)] private Vector2Int pathwaySize = new Vector2Int();

	[Header("References")]
	[SerializeField] private Transform mainLevelAssetsParent = default;

	[Header("Runtime Assets/References")]
	[SerializeField] private Dictionary<GameObject, List<Vector2Int>> rooms = new Dictionary<GameObject, List<Vector2Int>>();
	[SerializeField] private List<Vector2Int> occupiedRoomTileCoordinates = new List<Vector2Int>();

	[Header("Debug Settings")]
	[SerializeField, Tooltip("Default = 0.05f")] private float pathwayGizmoPlacementSpeed = 0.05f;
	[SerializeField, Tooltip("Default = 0.01f")] private float roomGizmoTilePlacementSpeed = 0.01f;

	private List<Vector2Int> walkerPathCoordinates = new List<Vector2Int>();
	private List<Vector2Int> walkerRoomOriginCoordinates = new List<Vector2Int>();

	private void Start()
	{
		seed = seed == 0 ? Random.Range(0, int.MaxValue) : seed;

		Random.InitState(seed);
		StartCoroutine(GenerateLevel());
	}

	private IEnumerator GenerateLevel()
	{
		yield return StartCoroutine(GenerateLevelLayout());
		yield return StartCoroutine(GenerateLevelRooms());

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

		walkerRoomOriginCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x, (int)walkerGO.transform.position.y));

		// The reason we remove 1 from the count is because the starting point of the walker is always a room. (otherwise you get a pathway leading to nothing).
		int finalRoomCount = Random.Range(roomCount.x, roomCount.y) - 1;
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
					walkerRoomOriginCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x, (int)walkerGO.transform.position.y));
				}
				else
				{
					walkerPathCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x, (int)walkerGO.transform.position.y));
				}
				yield return new WaitForSeconds(pathwayGizmoPlacementSpeed);
			}
		}
	}

	/// <summary>
	/// Generates all the level rooms with each a random size.
	/// </summary>
	/// <returns></returns>
	private IEnumerator GenerateLevelRooms()
	{
		foreach (Vector2 roomOriginCoordinate in walkerRoomOriginCoordinates)
		{
			int finalRoomWidth = Random.Range(roomWidth.x, roomWidth.y);
			int finalRoomHeight = Random.Range(roomHeight.x, roomHeight.y);
			GameObject roomParent = new GameObject("Room " + rooms.Count);
			roomParent.transform.parent = mainLevelAssetsParent;

			List<Vector2Int> roomTileCoordinates = new List<Vector2Int>();

			// Make room uneven. Why? Because an even room does not have a center tile.
			if (finalRoomWidth % 2 == 0)
			{
				finalRoomWidth -= 1;
			}
			if (finalRoomHeight % 2 == 0)
			{
				finalRoomHeight -= 1;
			}

			// Generate the actual room tile coordinates.
			for (int x = 0; x < finalRoomWidth; x++)
			{
				for (int y = 0; y < finalRoomHeight; y++)
				{
					Vector2Int tilePos = new Vector2Int((int)roomOriginCoordinate.x - (finalRoomWidth / 2) + x, (int)roomOriginCoordinate.y - (finalRoomHeight / 2) + y);
					roomTileCoordinates.Add(tilePos);
					if (!occupiedRoomTileCoordinates.Contains(tilePos))
					{
						occupiedRoomTileCoordinates.Add(tilePos);
					}
					yield return new WaitForSeconds(roomGizmoTilePlacementSpeed);
				}
			}

			// Add them all to the list for later use.
			rooms.Add(roomParent, roomTileCoordinates);
		}
	}

	private void OnDrawGizmosSelected()
	{
		// Draw path
		if (walkerPathCoordinates.Count > 0)
		{
			Gizmos.color = Color.white;
			foreach (Vector2 coordinate in walkerPathCoordinates)
			{
				Gizmos.DrawCube(coordinate, Vector2.one);
			}
		}

		// Draw room origin points.
		if (walkerRoomOriginCoordinates.Count > 0)
		{
			foreach (Vector2 coordinate in walkerRoomOriginCoordinates)
			{
				if (coordinate == walkerRoomOriginCoordinates[0])
				{
					Gizmos.color = Color.green;
				}
				else if (coordinate == walkerRoomOriginCoordinates[walkerRoomOriginCoordinates.Count - 1])
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

		// Draw room tiles.
		if (occupiedRoomTileCoordinates.Count > 0)
		{
			Gizmos.color = Color.grey;
			//foreach (KeyValuePair<GameObject, List<Vector2Int>> room in rooms)
			//{
			//	foreach (Vector2Int roomTile in room.Value)
			//	{
			//		Gizmos.DrawWireCube(new Vector2(roomTile.x, roomTile.y), Vector2.one);
			//	}
			//}

			foreach (Vector2Int occupiedRoomTileCoordinate in occupiedRoomTileCoordinates)
			{
				Gizmos.DrawWireCube(new Vector2(occupiedRoomTileCoordinate.x, occupiedRoomTileCoordinate.y), Vector2.one);
			}
		}
	}
}