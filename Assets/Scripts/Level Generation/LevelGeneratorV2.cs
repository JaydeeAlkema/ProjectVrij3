using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum RoomType
{
	Generic,
	Treassure,
	Boss,
	Spawn
}

public class LevelGeneratorV2 : MonoBehaviour
{
	[Header("Level Generation Settings")]
	[SerializeField] private int seed;
	[SerializeField] private int chunkSize = 35;
	[SerializeField] private int maxRooms = 10;
	[SerializeField] private int pathDepth = 2; // How far from the center the path will generate extra path tiles. So a depth of 2 results in a path that is 5 wide in total. Example: (## # ##)
	[SerializeField, Tooltip("The grid size may NEVER be divisible by 2")] private Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[SerializeField] private List<ScriptableRoom> spawnableRooms = new List<ScriptableRoom>();
	[SerializeField] private List<Sprite> pathGroundTileSprites = new List<Sprite>();
	[SerializeField] private List<Sprite> pathWallTileSprites = new List<Sprite>();
	[Space]
	[SerializeField] private List<Chunk> chunks = new List<Chunk>();
	[SerializeField] private List<Chunk> path = new List<Chunk>();
	[SerializeField] private List<Room> rooms = new List<Room>();
	[Space(10)]

	[Header("References")]
	[SerializeField] private Transform chunksParent = default;
	[SerializeField] private Transform roomsParent = default;
	[SerializeField] private Transform pathwaysParent = default;

	[Header("Debugging")]
	[SerializeField] private bool showGizmos = false;
	[SerializeField] private List<string> diagnosticTimes = new List<string>();
	[SerializeField] private long totalGenerationTimeInMilliseconds = 0;

	List<GameObject> pathwayParents = new List<GameObject>();

	public List<Room> Rooms { get => rooms; set => rooms = value; }

	private void Start()
	{
		if (seed == 0)
		{
			seed = Random.Range(0, int.MaxValue);
		}

		Random.InitState(seed);

	}

	public IEnumerator GenerateLevel()
	{
		yield return StartCoroutine(CreateChunks());
		yield return StartCoroutine(CreatePathThroughChunks());
		yield return StartCoroutine(CreateEmptyRooms());
		yield return StartCoroutine(GeneratePathwaysBetweenEmptyRooms());

		foreach (string diagnosticTime in diagnosticTimes)
		{
			Debug.Log(diagnosticTime);
		}
		Debug.Log($"Level Generation took in total: {totalGenerationTimeInMilliseconds}ms");
	}

	/// <summary>
	/// Lays the chunks grid.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreateChunks()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		if (chunkGridSize.x < 3) chunkGridSize.x = 3;
		if (chunkGridSize.y < 3) chunkGridSize.y = 3;

		if (chunkGridSize.x > 19) chunkGridSize.x = 19;
		if (chunkGridSize.y > 19) chunkGridSize.y = 19;

		// The grid size may NEVER be divisible by 2, this will cause an even grid without a center chunk... We must always have a center chunk!
		if (chunkGridSize.x % 2 == 0) chunkGridSize.x -= 1;
		if (chunkGridSize.y % 2 == 0) chunkGridSize.y -= 1;

		for (int x = 0; x < chunkGridSize.x; x++)
		{
			for (int y = 0; y < chunkGridSize.y; y++)
			{
				GameObject chunkGO = new GameObject($"Chunk [{x}][{y}]");
				Chunk chunk = chunkGO.AddComponent<Chunk>();

				chunk.Coordinates = new Vector2Int(x * chunkSize, y * chunkSize);
				chunk.Occupied = false;

				chunk.gameObject.transform.position = new Vector2(chunk.Coordinates.x, chunk.Coordinates.y);
				chunk.gameObject.transform.parent = chunksParent;

				// Create starting chunk node grid (the +1 comes from the fact that the nodes start on the edges of the chunk, not within the chunk)
				for (int nx = 0; nx < chunkSize + 1; nx++)
				{
					for (int ny = 0; ny < chunkSize + 1; ny++)
					{
						Vector2Int nodeCoordinates = new Vector2Int(Mathf.RoundToInt(chunk.transform.position.x - chunkSize / 2) + nx, Mathf.RoundToInt(chunk.transform.position.y - chunkSize / 2) + ny);

						GameObject nodeGO = new GameObject($"Node [{nodeCoordinates.x}][{nodeCoordinates.y}]");

						nodeGO.transform.position = new Vector2(nodeCoordinates.x, nodeCoordinates.y);
						nodeGO.transform.parent = chunk.transform;
					}
				}

				chunks.Add(chunk);
			}
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Chunk Generation took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
	}

	/// <summary>
	/// Create a path through the chunks, these chunks that are in the path will be used later to spawn rooms in.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreatePathThroughChunks()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		Vector2Int middleChunkCoordinates = new Vector2Int(chunks[chunks.Count - 1].Coordinates.x / 2, chunks[chunks.Count - 1].Coordinates.y / 2);
		Chunk currentChunk = GetChunkByCoordinates(middleChunkCoordinates);
		currentChunk.Occupied = true;

		path.Add(currentChunk);

		while (true)
		{
			List<Chunk> neighbours = GetNeighbouringChunks(currentChunk.Coordinates);

			if (neighbours.Count > 0)
			{
				int randNeighbour = Random.Range(0, neighbours.Count);
				Chunk nextChunk = neighbours[randNeighbour];

				nextChunk.Occupied = true;
				path.Add(nextChunk);
				currentChunk = nextChunk;
			}
			else
			{
				break;
			}

			if (path.Count >= maxRooms) break;
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Chunk path generation took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
	}

	/// <summary>
	/// Instantiate empty tooms within the borders of the chunks and setup the rooms for later use.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreateEmptyRooms()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		// Spawn the rooms within the chunk borders.
		foreach (Chunk chunk in path)
		{
			ScriptableRoom randRoom = spawnableRooms[Random.Range(0, spawnableRooms.Count)];

			GameObject newRoomGO = Instantiate(randRoom.Prefab, new Vector2(0, 0), Quaternion.identity);
			newRoomGO.name = $"Room [{rooms.Count + 1}]";
			Room room = newRoomGO.GetComponent<Room>();
			chunk.Room = room;

			int chunkSizeHalf = chunkSize / 2;
			int roomsizeHalfX = room.RoomSize.x / 2;
			int roomsizeHalfY = room.RoomSize.y / 2;

			int randX = Random.Range(chunk.Coordinates.x - chunkSizeHalf + roomsizeHalfX + 3, chunk.Coordinates.x + chunkSizeHalf - roomsizeHalfX);
			int randY = Random.Range(chunk.Coordinates.y - chunkSizeHalf + roomsizeHalfY + 3, chunk.Coordinates.y + chunkSizeHalf - roomsizeHalfY);

			newRoomGO.transform.position = new Vector2(randX, randY);

			int randRot = Random.Range(0, 4);
			newRoomGO.transform.Rotate(new Vector3(0, 0, randRot * 90));

			newRoomGO.transform.parent = roomsParent;
			rooms.Add(room);
		}

		// Assign connected rooms for each room.
		int roomIndex = 0;
		foreach (Room room in rooms)
		{
			if (roomIndex - 1 >= 0 && path[roomIndex - 1].Room != null) room.AddConnectedRoom(path[roomIndex - 1].Room);
			if (roomIndex + 1 < path.Count && path[roomIndex + 1].Room != null) room.AddConnectedRoom(path[roomIndex + 1].Room);

			roomIndex++;
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Empty rooms generation took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
	}

	/// <summary>
	/// Generate pathways between empty rooms
	/// </summary>
	/// <returns></returns>
	private IEnumerator GeneratePathwaysBetweenEmptyRooms()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		// Open the pathway openings for each room.
		for (int r = 0; r < rooms.Count - 1; r++)
		{
			Room room = rooms[r];
			Room roomToConnectTo = room.ConnectedRooms[room.ConnectedRooms.Count - 1];
			GameObject pathwayStartPoint = null;
			GameObject pathwayEndPoint = null;

			// Get nearest pathwayConnectionPoint from own room to the room to connect to.
			float nearestDistance = Mathf.Infinity;
			foreach (GameObject pathwayConnectionPoint in room.PathwayOpenings)
			{
				float distance = Vector2.Distance(roomToConnectTo.transform.position, pathwayConnectionPoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					pathwayStartPoint = pathwayConnectionPoint;
				}
			}
			// Now we do the same, but from the connecting room to our own.
			nearestDistance = Mathf.Infinity;
			foreach (GameObject pathwayConnectionPoint in roomToConnectTo.PathwayOpenings)
			{
				float distance = Vector2.Distance(room.transform.position, pathwayConnectionPoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					pathwayEndPoint = pathwayConnectionPoint;
				}
			}

			// Disable starting pathway opening
			foreach (GameObject pathwayOpening in room.PathwayOpenings)
			{
				if (pathwayOpening == pathwayStartPoint)
				{
					pathwayStartPoint.SetActive(false);
					foreach (Transform child in pathwayStartPoint.GetComponentInChildren<Transform>())
					{
						if (child != null)
							child.gameObject.SetActive(false);
					}
				}
			}
			// Disable ending pathway opening
			foreach (GameObject pathwayOpening in roomToConnectTo.PathwayOpenings)
			{
				if (pathwayOpening == pathwayEndPoint)
				{
					pathwayEndPoint.SetActive(false);
					foreach (Transform child in pathwayEndPoint.GetComponentInChildren<Transform>())
					{
						if (child != null)
							child.gameObject.SetActive(false);
					}
				}
			}
		}

		// Setup Astar Graph
		Vector2Int middleChunkCoordinates = new Vector2Int(chunks[chunks.Count - 1].Coordinates.x / 2, chunks[chunks.Count - 1].Coordinates.y / 2);
		AstarData astarData = AstarPath.active.data;
		GridGraph gg = astarData.gridGraph;

		GameObject SeekerGO = new GameObject("Seeker");
		Seeker seeker = SeekerGO.AddComponent<Seeker>();

		gg.center = new Vector3(middleChunkCoordinates.x, middleChunkCoordinates.y, 0);
		gg.SetDimensions(chunkSize * chunkGridSize.x + 1, chunkSize * chunkGridSize.y + 1, gg.nodeSize);
		AstarPath.active.Scan();

		while (AstarPath.active.IsAnyGraphUpdateInProgress)
		{
			yield return new WaitForEndOfFrame();
		}

		for (int r = 0; r < rooms.Count - 1; r++)
		{
			Room room = rooms[r];
			Room roomToConnectTo = room.ConnectedRooms[room.ConnectedRooms.Count - 1];
			GameObject pathwayStartPoint = null;
			GameObject pathwayEndPoint = null;

			// Get nearest pathwayConnectionPoint from own room to the room to connect to.
			float nearestDistance = Mathf.Infinity;
			foreach (GameObject pathwayConnectionPoint in room.PathwayOpenings)
			{
				float distance = Vector2.Distance(roomToConnectTo.transform.position, pathwayConnectionPoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					pathwayStartPoint = pathwayConnectionPoint;
				}
			}
			// Now we do the same, but from the connecting room to our own.
			nearestDistance = Mathf.Infinity;
			foreach (GameObject pathwayConnectionPoint in roomToConnectTo.PathwayOpenings)
			{
				float distance = Vector2.Distance(room.transform.position, pathwayConnectionPoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					pathwayEndPoint = pathwayConnectionPoint;
				}
			}

			// Actually build the pathways here...
			Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(pathwayStartPoint.transform.position.x), Mathf.RoundToInt(pathwayStartPoint.transform.position.y));
			Vector2Int targetPos = new Vector2Int(Mathf.RoundToInt(pathwayEndPoint.transform.position.x), Mathf.RoundToInt(pathwayEndPoint.transform.position.y));

			while (seeker.IsDone() == false)
			{
				yield return new WaitForEndOfFrame();
			}

			Path path = null;
			path = seeker.StartPath(new Vector3(startPos.x, startPos.y, 0), new Vector3(targetPos.x, targetPos.y, 0), OnPathComplete);
		}

		while (AstarPath.active.IsAnyGraphUpdateInProgress || seeker.IsDone() == false)
		{
			yield return new WaitForEndOfFrame();
		}

		Destroy(SeekerGO);
		executionTime.Stop();
		diagnosticTimes.Add($"Generating pathways between empty rooms took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
	}

	private void OnPathComplete(Path p)
	{
		//Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
		Vector3 startVector = p.vectorPath[0];
		Vector3 endVector = p.vectorPath[p.vectorPath.Count - 1];
		Vector3 pathPointParentCenter = new Vector3(startVector.x, startVector.y, 0) - new Vector3(endVector.x, endVector.y, 0);

		GameObject pathPointParentGO = new GameObject("Pathway");
		pathPointParentGO.transform.position = pathPointParentCenter;
		pathPointParentGO.transform.parent = pathwaysParent;

		List<Vector2Int> pathPoints = new List<Vector2Int>();

		foreach (Vector3 pathCoord in p.vectorPath)
		{
			Vector2Int pathCoordInt = new Vector2Int(Mathf.RoundToInt(pathCoord.x), Mathf.RoundToInt(pathCoord.y));
			pathPoints.Add(pathCoordInt);

			GameObject pathPointGO = new GameObject($"PathPoint[{pathCoordInt.x}][{pathCoordInt.y}]");
			pathPointGO.transform.position = new Vector3(pathCoordInt.x, pathCoordInt.y, 0);
			pathPointGO.transform.parent = pathPointParentGO.transform;

			SpriteRenderer originPathPointSpriteRenderer = pathPointGO.AddComponent<SpriteRenderer>();
			originPathPointSpriteRenderer.sprite = pathGroundTileSprites[0];

			for (int x = -pathDepth; x < pathDepth + 1; x++)
			{
				for (int y = -pathDepth; y < pathDepth + 1; y++)
				{
					// Do not check for center tile. we know this one exists already.
					if (x != 0 && y != 0)
					{
						Vector2Int newCoord = new Vector2Int(Mathf.RoundToInt(pathCoordInt.x + x), Mathf.RoundToInt(pathCoordInt.y + y));
						if (pathPoints.Contains(newCoord) == false)
						{
							GameObject newPathPointGO = new GameObject($"PathPoint[{newCoord.x}][{newCoord.y}]");
							newPathPointGO.transform.position = new Vector3(newCoord.x, newCoord.y, 0);
							newPathPointGO.transform.parent = pathPointParentGO.transform;
							pathPoints.Add(newCoord);

							SpriteRenderer newPathPointSpriteRenderer = newPathPointGO.AddComponent<SpriteRenderer>();
							newPathPointSpriteRenderer.sprite = pathGroundTileSprites[0];
						}
					}
				}
			}
		}

		diagnosticTimes.Add($"Pathfinder took: {p.duration}ms");
		totalGenerationTimeInMilliseconds += (long)p.duration;
	}

	#region Helpers
	/// <summary>
	/// Get a random chunk.
	/// </summary>
	/// <param name="currentChunk"> Reference to the current chunk. </param>
	/// <param name="nextChunk"> Reference to the next chunk. </param>
	/// <returns></returns>
	private List<Chunk> GetNeighbouringChunks(Vector2Int currentChunkCoordinates)
	{
		List<Chunk> neighbourChunkIndeces = new List<Chunk>();

		int chunkX = currentChunkCoordinates.x;
		int chunkY = currentChunkCoordinates.y;

		Chunk topNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX, chunkY + chunkSize));
		Chunk rightNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX + chunkSize, chunkY));
		Chunk bottomNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX, chunkY - chunkSize));
		Chunk leftNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX - chunkSize, chunkY));

		if (topNeighbour != null && path.Contains(topNeighbour) == false) neighbourChunkIndeces.Add(topNeighbour);
		if (rightNeighbour != null && path.Contains(rightNeighbour) == false) neighbourChunkIndeces.Add(rightNeighbour);
		if (bottomNeighbour != null && path.Contains(bottomNeighbour) == false) neighbourChunkIndeces.Add(bottomNeighbour);
		if (leftNeighbour != null && path.Contains(leftNeighbour) == false) neighbourChunkIndeces.Add(leftNeighbour);

		return neighbourChunkIndeces;
	}

	/// <summary>
	/// Get a chunk from coordinates.
	/// </summary>
	/// <param name="coordinates"> Coordinates to fetch the chunk by. </param>
	/// <returns></returns>
	private Chunk GetChunkByCoordinates(Vector2Int coordinates)
	{
		for (int i = 0; i < chunks.Count; i++)
		{
			if (chunks[i].Coordinates == coordinates)
			{
				return chunks[i];
			}
		}
		return null;
	}
	#endregion

	private void OnDrawGizmos()
	{
		if (!showGizmos) return;

		if (chunks.Count > 0)
		{
			for (int i = 0; i < chunks.Count; i++)
			{
				Chunk chunk = chunks[i];
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(chunk.gameObject.transform.position, new Vector3(chunkSize, chunkSize));
			}
		}

		if (path.Count > 1)
		{
			for (int c = 0; c < path.Count - 1; c++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(path[c].gameObject.transform.position, path[c + 1].gameObject.transform.position);
			}
		}
	}
}

[System.Serializable]
public class Chunk : MonoBehaviour
{
	[SerializeField] private Vector2Int coordinates = new Vector2Int();
	[SerializeField] private bool occupied = false;
	[SerializeField] private Room room;

	public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
	public bool Occupied { get => occupied; set => occupied = value; }
	public Room Room { get => room; set => room = value; }
}