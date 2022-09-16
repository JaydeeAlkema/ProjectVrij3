using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO:
// 1. Change algorithm in such a way that the rooms may never overlap. May result in more boring maps.

public class DungeonGenerator : MonoBehaviour
{
	[Header("Generation Configuration")]
	[SerializeField] private int seed = 0;
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomCount = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomWidth = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int roomHeight = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 30)] private Vector2Int pathwayLength = new Vector2Int();
	[Space(10)]

	[Header("Generation Assets")]
	[SerializeField] private List<Sprite> groundSprites = new List<Sprite>();
	[SerializeField] private List<Sprite> wallSprites = new List<Sprite>();
	[SerializeField] private List<Sprite> innerCornerWallSprites = new List<Sprite>();
	[SerializeField] private List<Sprite> outerCornerWallSprites = new List<Sprite>();

	[Header("References")]
	[SerializeField] private Transform mainDungeonAssetsParent = default;
	[Space(10)]

	[Header("Runtime Assets/References")]
	[SerializeField] private Dictionary<GameObject, List<Vector2Int>> rooms = new Dictionary<GameObject, List<Vector2Int>>();
	[SerializeField] private Dictionary<GameObject, Vector2Int> occupiedTiles = new Dictionary<GameObject, Vector2Int>();
	[Space(10)]

	[Header("Debug Settings")]
	[SerializeField] private bool drawGizmos = true;
	[SerializeField, Tooltip("Default = 0.05f")] private float pathwayGizmoPlacementSpeed = 0.05f;
	[SerializeField, Tooltip("Default = 0.01f")] private float roomGizmoTilePlacementSpeed = 0.01f;

	private List<Vector2Int> walkerPathCoordinates = new List<Vector2Int>();
	private List<Vector2Int> walkerRoomOriginCoordinates = new List<Vector2Int>();

	private void Start()
	{
		seed = seed == 0 ? Random.Range(0, int.MaxValue) : seed;

		Random.InitState(seed);
		StartCoroutine(GenerateDungeon());
	}

	private IEnumerator GenerateDungeon()
	{
		yield return StartCoroutine(GenerateDungeonLayout());
		yield return StartCoroutine(GenerateDungeonRooms());
		yield return StartCoroutine(GenerateDungeonAssets());
	}

	#region Dungeon Layout
	/// <summary>
	/// This functions generates the layout of the dungeon. This is done with the help of a "walker"
	/// This walker will "walk" a path, everytime this walker reaches it target distance, it takes a turn and marks the tile as a room origin.
	/// After the walker is done walking it's path, every room origin tiles will get saved and used later on in the generator to generate rooms upon.
	/// </summary>
	private IEnumerator GenerateDungeonLayout()
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
					//TODO: Find a better way of doing this... This is very shoddy, but the end product works :P
					walkerPathCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x, (int)walkerGO.transform.position.y));
					walkerPathCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x - 1, (int)walkerGO.transform.position.y - 1));
					walkerPathCoordinates.Add(new Vector2Int((int)walkerGO.transform.position.x + 1, (int)walkerGO.transform.position.y + 1));
				}

				if (pathwayGizmoPlacementSpeed == 0)
				{
					yield return null;
				}
				else
				{
					yield return new WaitForSeconds(pathwayGizmoPlacementSpeed);
				}
			}
		}
		walkerPathCoordinates = walkerPathCoordinates.Distinct().ToList();
	}

	/// <summary>
	/// Generates all the Dungeon rooms with each a random size.
	/// </summary>
	/// <returns></returns>
	private IEnumerator GenerateDungeonRooms()
	{
		foreach (Vector2 roomOriginCoordinate in walkerRoomOriginCoordinates)
		{
			int finalRoomWidth = Random.Range(roomWidth.x, roomWidth.y);
			int finalRoomHeight = Random.Range(roomHeight.x, roomHeight.y);
			GameObject roomParent = new GameObject("Room " + rooms.Count);
			roomParent.transform.position = roomOriginCoordinate;
			roomParent.transform.parent = mainDungeonAssetsParent;

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

					if (roomGizmoTilePlacementSpeed == 0)
					{
						yield return null;
					}
					else
					{
						yield return new WaitForSeconds(roomGizmoTilePlacementSpeed);
					}
				}
			}

			// Add them all to the list for later use.
			rooms.Add(roomParent, roomTileCoordinates);
		}
	}
	#endregion

	#region Dungeon Asset Placement
	/// <summary>
	/// Generate the actual assets for in the Dungeon. For example: Walls, Floors, Decoration, etc.
	/// </summary>
	/// <returns></returns>
	private IEnumerator GenerateDungeonAssets()
	{
		// Generate all the tile objects. For now these objects are all empty.
		foreach (KeyValuePair<GameObject, List<Vector2Int>> room in rooms)
		{
			int tileIndex = 0;
			foreach (Vector2Int coordinate in room.Value)
			{
				if (!occupiedTiles.ContainsValue(coordinate))
				{
					GameObject tileGO = new GameObject($"Tile {tileIndex}");
					tileGO.transform.position = new Vector3(coordinate.x, coordinate.y, 0);
					tileGO.transform.parent = room.Key.transform;

					occupiedTiles.Add(tileGO, coordinate);
					tileIndex++;
				}
				else
				{
					yield return null;
				}
			}
		}

		// Loop through all the occupied tiles and get their neighbouring tiles.
		// This is an pretty expensive operation, but very much needed.
		foreach (KeyValuePair<GameObject, Vector2Int> occupiedTile in occupiedTiles)
		{
			// Tile Setup
			GameObject tileGO = occupiedTile.Key;
			Vector2Int tileCoordinate = occupiedTile.Value;

			// All occupied tiles require atleast an SpriteRenderer.
			tileGO.AddComponent(typeof(SpriteRenderer));
			SpriteRenderer spriteRenderer = tileGO.GetComponent<SpriteRenderer>();

			// Neighbouring tile coordinates (Clockwise, starting from the top neighbour)
			Vector2Int topNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x, tileCoordinate.y + 1);
			Vector2Int topRightNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x + 1, tileCoordinate.y + 1);
			Vector2Int rightNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x + 1, tileCoordinate.y);
			Vector2Int bottomRightNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x + 1, tileCoordinate.y - 1);
			Vector2Int bottomNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x, tileCoordinate.y - 1);
			Vector2Int bottomLeftNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x - 1, tileCoordinate.y - 1);
			Vector2Int leftNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x - 1, tileCoordinate.y);
			Vector2Int topLeftNeighbourTileCoordinate = new Vector2Int(tileCoordinate.x - 1, tileCoordinate.y + 1);

			// Neighbouring tile GameObject references.
			GameObject topNeighbourGO = null;
			GameObject topRightNeighbourGO = null;
			GameObject rightNeighbourGO = null;
			GameObject bottomRightNeighbourGO = null;
			GameObject bottomNeighbourGO = null;
			GameObject bottomLeftNeighbourGO = null;
			GameObject leftNeighbourGO = null;
			GameObject topLeftNeighbourGO = null;

			int neighbourCount = 0;
			//string neighbours = "";

			// Get all neighbouring tiles.
			foreach (KeyValuePair<GameObject, Vector2Int> occupiedNeighbourTile in occupiedTiles)
			{
				Vector2Int neighbourTileCoordinate = occupiedNeighbourTile.Value;

				// Try to fetch neighbour tiles from list (Clockwise, starting from the top)
				if (neighbourTileCoordinate == topNeighbourTileCoordinate)
				{
					topNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "top - ";
				}
				else if (neighbourTileCoordinate == topRightNeighbourTileCoordinate)
				{
					topRightNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "topRight - ";
				}
				else if (neighbourTileCoordinate == rightNeighbourTileCoordinate)
				{
					rightNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "right - ";
				}
				else if (neighbourTileCoordinate == bottomRightNeighbourTileCoordinate)
				{
					bottomRightNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "bottomRight - ";
				}
				else if (neighbourTileCoordinate == bottomNeighbourTileCoordinate)
				{
					bottomNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "bottom - ";
				}
				else if (neighbourTileCoordinate == bottomLeftNeighbourTileCoordinate)
				{
					bottomLeftNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "bottomLeft - ";
				}
				else if (neighbourTileCoordinate == leftNeighbourTileCoordinate)
				{
					leftNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "left - ";
				}
				else if (neighbourTileCoordinate == topLeftNeighbourTileCoordinate)
				{
					topLeftNeighbourGO = occupiedTile.Key;
					neighbourCount++;
					//neighbours += "topLeft - ";
				}
			}

			//Debug.Log(neighbourCount + " - " + neighbours, tileGO);

			// Adding the actual sprites to the gameobjects.

			// If the tile has 8 neighbours, it means it is surrounded, thus it can only be a ground tile.
			if (neighbourCount == 8)
			{
				spriteRenderer.sprite = groundSprites[Random.Range(0, groundSprites.Count)];
			}
			// Walls
			else
			{
				// We still check for separate directions (top, right, bottom and left) because later we might have different looking sprites for each direction.

				// TOP
				if (leftNeighbourGO && bottomNeighbourGO && rightNeighbourGO && !topNeighbourGO)
				{
					spriteRenderer.sprite = wallSprites[Random.Range(0, wallSprites.Count)];
				}
				// RIGHT
				else if (topNeighbourGO && leftNeighbourGO && bottomNeighbourGO && !rightNeighbourGO)
				{
					spriteRenderer.sprite = wallSprites[Random.Range(0, wallSprites.Count)];
				}
				// BOTTOM
				else if (leftNeighbourGO && topNeighbourGO && rightNeighbourGO && !bottomNeighbourGO)
				{
					spriteRenderer.sprite = wallSprites[Random.Range(0, wallSprites.Count)];
				}
				// LEFT
				else if (topNeighbourGO && rightNeighbourGO && bottomNeighbourGO && !leftNeighbourGO)
				{
					spriteRenderer.sprite = wallSprites[Random.Range(0, wallSprites.Count)];
				}

				// Outer Corners
				// TOP LEFT
				else if (rightNeighbourGO && bottomRightNeighbourGO && bottomNeighbourGO && !leftNeighbourGO && !topLeftNeighbourGO && !topNeighbourGO)
				{
					spriteRenderer.sprite = innerCornerWallSprites[Random.Range(0, innerCornerWallSprites.Count)];
				}
				// TOP RIGHT
				else if (leftNeighbourGO && bottomLeftNeighbourGO && bottomNeighbourGO && !topNeighbourGO && !topRightNeighbourGO && !rightNeighbourGO)
				{
					spriteRenderer.sprite = innerCornerWallSprites[Random.Range(0, innerCornerWallSprites.Count)];
					spriteRenderer.flipX = true;
				}
				// BOTTOM RIGHT
				else if (topNeighbourGO && topLeftNeighbourGO && leftNeighbourGO && !bottomNeighbourGO && !bottomRightNeighbourGO && !rightNeighbourGO)
				{
					spriteRenderer.sprite = innerCornerWallSprites[Random.Range(0, innerCornerWallSprites.Count)];
					spriteRenderer.flipX = true;
					spriteRenderer.flipY = true;
				}
				// BOTTOM LEFT
				else if (topNeighbourGO && topRightNeighbourGO && rightNeighbourGO && !leftNeighbourGO && !bottomLeftNeighbourGO && !bottomNeighbourGO)
				{
					spriteRenderer.sprite = innerCornerWallSprites[Random.Range(0, innerCornerWallSprites.Count)];
					spriteRenderer.flipY = true;
				}

				// Inner Corners
				// TOP LEFT
				else if (topNeighbourGO && topRightNeighbourGO && rightNeighbourGO && bottomRightNeighbourGO && bottomNeighbourGO && bottomLeftNeighbourGO && leftNeighbourGO && !topLeftNeighbourGO)
				{
					spriteRenderer.sprite = outerCornerWallSprites[Random.Range(0, outerCornerWallSprites.Count)];
					spriteRenderer.flipX = true;
					spriteRenderer.flipY = true;
				}
				// TOP RIGHT
				else if (rightNeighbourGO && bottomRightNeighbourGO && bottomNeighbourGO && bottomLeftNeighbourGO && leftNeighbourGO && topLeftNeighbourGO && topNeighbourGO && !topRightNeighbourGO)
				{
					spriteRenderer.sprite = outerCornerWallSprites[Random.Range(0, outerCornerWallSprites.Count)];
					spriteRenderer.flipY = true;
				}
				// BOTTOM RIGHT
				else if (bottomNeighbourGO && bottomLeftNeighbourGO && leftNeighbourGO && topLeftNeighbourGO && topNeighbourGO && topRightNeighbourGO && rightNeighbourGO && !bottomRightNeighbourGO)
				{
					spriteRenderer.sprite = outerCornerWallSprites[Random.Range(0, outerCornerWallSprites.Count)];
				}
				// BOTTOM LEFT
				else if (leftNeighbourGO && topLeftNeighbourGO && topNeighbourGO && topRightNeighbourGO && rightNeighbourGO && bottomRightNeighbourGO && bottomNeighbourGO && !bottomLeftNeighbourGO)
				{
					spriteRenderer.sprite = outerCornerWallSprites[Random.Range(0, outerCornerWallSprites.Count)];
					spriteRenderer.flipX = true;
				}
			}
		}
		yield return null;
	}
	#endregion

	private void OnDrawGizmos()
	{
		if (drawGizmos == false) return;

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
		if (rooms.Count > 0)
		{
			Gizmos.color = Color.grey;
			foreach (KeyValuePair<GameObject, List<Vector2Int>> room in rooms)
			{
				foreach (Vector2Int roomTile in room.Value)
				{
					Gizmos.DrawWireCube(new Vector2(roomTile.x, roomTile.y), Vector2.one);
				}
			}

			//foreach (Vector2Int occupiedRoomTileCoordinate in occupiedRoomTileCoordinates)
			//{
			//	Gizmos.DrawWireCube(new Vector2(occupiedRoomTileCoordinate.x, occupiedRoomTileCoordinate.y), Vector2.one);
			//}
		}
	}
}