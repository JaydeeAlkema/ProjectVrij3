// Writen by: Jaydee Alkema
// the Level Generator is responsible for, well, generating the map.
// This generator is a bit of a mix of behaviours however. Rooms for example aren't generate, but placed within the level chunks.
// A pathway is then generated between the rooms to connect them together. This might be a bit unconventional, but this is done for experimenting purposes, and it works relatively well.

using NaughtyAttributes;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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

	[SerializeField, Expandable] ScriptableLevelGenerationSettings SLGS = null;
	[Space]

	[Header("Runtime References")]
	[SerializeField] private int seed = 0;
	[SerializeField] private List<Chunk> chunks = new List<Chunk>();
	[SerializeField] private List<Chunk> path = new List<Chunk>();
	[SerializeField] private List<Room> rooms = new List<Room>();
	[SerializeField] private List<GameObject> decorations = new List<GameObject>();
	[SerializeField] private AstarData astarData = null;
	[Space(10)]

	[Header("References")]
	[SerializeField] private Transform chunksParent = default;
	[SerializeField] private Transform roomsParent = default;
	[SerializeField] private Transform pathwaysParent = default;
	[SerializeField] private Material defaultSpriteMaterial = default;
	[Space(10)]

	[Header("Debugging")]
	[SerializeField] private bool showGizmos = false;
	[SerializeField] private List<string> diagnosticTimes = new List<string>();
	[SerializeField] private long totalGenerationTimeInMilliseconds = 0;
	[Space(10)]

	[Header("Spawnable Prefabs")]
	[SerializeField] private GameObject bossFightPortal = default;
	[SerializeField] private GameObject levelUpStatue = default;

	List<GameObject> pathwayParents = new List<GameObject>();

	public List<Room> Rooms { get => rooms; set => rooms = value; }

	private void Start()
	{
		if (seed == 0) seed = Random.Range(0, int.MaxValue);

		Random.InitState(seed);

		GameManager.Instance.FetchDungeonReferences();
	}

	public IEnumerator GenerateLevel()
	{
		yield return StartCoroutine(CreateChunks());
		yield return StartCoroutine(CreatePathThroughChunks());
		yield return StartCoroutine(CreateEmptyRooms());
		yield return StartCoroutine(GeneratePathwaysBetweenEmptyRooms());
		yield return StartCoroutine(ConfigurePathways()); // << This needs to be optimized. By far the worst offender when it comes to generation times.
		yield return StartCoroutine(SpawnEnemies());

		// This should always be called last!
		yield return StartCoroutine(DecorateLevel());

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

		if (SLGS.chunkGridSize.x < 3) SLGS.chunkGridSize.x = 3;
		if (SLGS.chunkGridSize.y < 3) SLGS.chunkGridSize.y = 3;

		if (SLGS.chunkGridSize.x > 19) SLGS.chunkGridSize.x = 19;
		if (SLGS.chunkGridSize.y > 19) SLGS.chunkGridSize.y = 19;

		// The grid size may NEVER be divisible by 2, this will cause an even grid without a center chunk... We must always have a center chunk!
		if (SLGS.chunkGridSize.x % 2 == 0) SLGS.chunkGridSize.x -= 1;
		if (SLGS.chunkGridSize.y % 2 == 0) SLGS.chunkGridSize.y -= 1;

		for (int x = 0; x < SLGS.chunkGridSize.x; x++)
		{
			for (int y = 0; y < SLGS.chunkGridSize.y; y++)
			{
				GameObject chunkGO = new GameObject($"Chunk [{x}][{y}]");
				Chunk chunk = chunkGO.AddComponent<Chunk>();

				chunk.Coordinates = new Vector2Int(x * SLGS.chunkSize, y * SLGS.chunkSize);
				chunk.Occupied = false;

				chunk.gameObject.transform.position = new Vector2(chunk.Coordinates.x, chunk.Coordinates.y);
				chunk.gameObject.transform.parent = chunksParent;

				// Create starting chunk node grid (the +1 comes from the fact that the nodes start on the edges of the chunk, not within the chunk)
				for (int nx = 0; nx < SLGS.chunkSize + 1; nx++)
				{
					for (int ny = 0; ny < SLGS.chunkSize + 1; ny++)
					{
						Vector2Int nodeCoordinates = new Vector2Int(Mathf.RoundToInt(chunk.transform.position.x - SLGS.chunkSize / 2) + nx, Mathf.RoundToInt(chunk.transform.position.y - SLGS.chunkSize / 2) + ny);

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

		yield return null;
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

			if (path.Count >= SLGS.maxRooms) break;
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Chunk path generation took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
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
		for (int r = 0; r < path.Count; r++)
		{
			Chunk chunk = path[r];
			ScriptableRoom randRoom = null;
			// Spawn roomType depending on how far along the path we are (the end of the path should always be a boss room)
			if (r == 0)
			{
				randRoom = SLGS.spawnRooms[Random.Range(0, SLGS.spawnRooms.Count)];
			}
			else if (r == path.Count - 1)
			{
				randRoom = SLGS.bossRooms[Random.Range(0, SLGS.bossRooms.Count)];
			}
			else
			{
				randRoom = SLGS.genericRooms[Random.Range(0, SLGS.genericRooms.Count)];
			}

			GameObject newRoomGO = Instantiate(randRoom.Prefab, new Vector2(0, 0), Quaternion.identity);
			newRoomGO.name = $"Room [{rooms.Count + 1}]";
			Room room = newRoomGO.GetComponent<Room>();
			chunk.Room = room;

			int chunkSizeHalf = SLGS.chunkSize / 2;
			int roomsizeHalfX = room.RoomSize.x / 2;
			int roomsizeHalfY = room.RoomSize.y / 2;

			int randX = Random.Range(chunk.Coordinates.x - chunkSizeHalf + roomsizeHalfX, chunk.Coordinates.x + chunkSizeHalf - roomsizeHalfX);
			int randY = Random.Range(chunk.Coordinates.y - chunkSizeHalf + roomsizeHalfY, chunk.Coordinates.y + chunkSizeHalf - roomsizeHalfY);

			newRoomGO.transform.position = new Vector2(randX, randY);
			newRoomGO.transform.parent = roomsParent;
			rooms.Add(room);

			yield return StartCoroutine(room.InitRoom());
		}

		int roomIndex = 0;
		// Assign connected rooms for each room.
		foreach (Room room in rooms)
		{
			if (roomIndex == 0)
			{
				room.RoomType = RoomType.Spawn;
			}
			else if (roomIndex == rooms.Count - 1)
			{
				room.RoomType = RoomType.Boss;
			}
			else
			{
				room.RoomType = RoomType.Generic;
			}

			if (roomIndex - 1 >= 0 && path[roomIndex - 1].Room != null) room.AddConnectedRoom(path[roomIndex - 1].Room);
			if (roomIndex + 1 < path.Count && path[roomIndex + 1].Room != null) room.AddConnectedRoom(path[roomIndex + 1].Room);

			room.RandomizeFloorTileSprites(SLGS.floorSprites);

			roomIndex++;
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Empty rooms generation took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
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
		List<Transform> occupiedChunks = new List<Transform>();
		foreach (Chunk chunk in chunks)
		{
			if (chunk.Occupied)
			{
				occupiedChunks.Add(chunk.transform);
			}
		}
		Bounds occupiedChunksBounds = GetBoundsFromTransforms(occupiedChunks);
		Vector3 centerOfOccupiedChunks = occupiedChunksBounds.center;
		Vector2Int sizeOfOccupiedChunks = new Vector2Int(Mathf.RoundToInt(occupiedChunksBounds.size.x), Mathf.RoundToInt(occupiedChunksBounds.size.y));
		Vector2Int middleChunkCoordinates = new Vector2Int(Mathf.RoundToInt(centerOfOccupiedChunks.x), Mathf.RoundToInt(centerOfOccupiedChunks.y));
		astarData = AstarPath.active.data;

		GameObject SeekerGO = new GameObject("Seeker");
		Seeker seeker = SeekerGO.AddComponent<Seeker>();

		foreach (GridGraph gridGraph in astarData.graphs)
		{
			gridGraph.center = new Vector3(middleChunkCoordinates.x, middleChunkCoordinates.y, 0);
			gridGraph.SetDimensions(sizeOfOccupiedChunks.x + SLGS.chunkSize, sizeOfOccupiedChunks.y + SLGS.chunkSize, gridGraph.nodeSize);
			AstarPath.active.Scan(gridGraph);
		}

		while (AstarPath.active.IsAnyGraphUpdateInProgress)
		{
			yield return null;
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
				yield return null;
			}

			Path path = null;
			path = seeker.StartPath(new Vector3(startPos.x, startPos.y, 0), new Vector3(targetPos.x, targetPos.y, 0), OnPathComplete);
		}

		while (AstarPath.active.IsAnyGraphUpdateInProgress || seeker.IsDone() == false)
		{
			yield return null;
		}

		foreach (Room room in rooms)
		{
			yield return StartCoroutine(room.InitRoom());
		}

		Destroy(SeekerGO);
		executionTime.Stop();
		diagnosticTimes.Add($"Generating pathways between empty rooms took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
	}

	/// <summary>
	/// Handles the placement of all pathway tiles as soon as a path has been found between two rooms.
	/// </summary>
	/// <param name="p"> Reference to the path. </param>
	private void OnPathComplete(Path p)
	{
		//Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
		Vector3 startVector = p.vectorPath[0];
		Vector3 endVector = p.vectorPath[p.vectorPath.Count - 1];
		Vector3 pathPointParentCenter = new Vector3(startVector.x, startVector.y, 0) - new Vector3(endVector.x, endVector.y, 0);

		GameObject pathPointParentGO = new GameObject("Pathway");
		pathPointParentGO.transform.position = pathPointParentCenter;
		pathPointParentGO.transform.parent = pathwaysParent;

		pathwayParents.Add(pathPointParentGO);

		Room roomA = rooms[pathwayParents.Count - 1];
		Room roomB = rooms[pathwayParents.Count];

		Dictionary<Vector2Int, Transform> occupiedTiles = new Dictionary<Vector2Int, Transform>();
		foreach (KeyValuePair<Vector2Int, Transform> collideableTileA in roomA.CollideableTiles)
		{
			occupiedTiles.TryAdd(collideableTileA.Key, collideableTileA.Value);
		}
		foreach (KeyValuePair<Vector2Int, Transform> nonCollideableTileA in roomA.NoncollideableTiles)
		{
			occupiedTiles.TryAdd(nonCollideableTileA.Key, nonCollideableTileA.Value);
		}
		foreach (KeyValuePair<Vector2Int, Transform> collideableTileB in roomB.CollideableTiles)
		{
			occupiedTiles.TryAdd(collideableTileB.Key, collideableTileB.Value);
		}
		foreach (KeyValuePair<Vector2Int, Transform> nonCollideableTileB in roomB.NoncollideableTiles)
		{
			occupiedTiles.TryAdd(nonCollideableTileB.Key, nonCollideableTileB.Value);
		}

		List<Vector2Int> pathPoints = new List<Vector2Int>();

		// Instantiate all the initial tiles. These tiles will have not been configurated yet, this will need to be done is a separate pass.
		foreach (Vector3 pathCoord in p.vectorPath)
		{
			Vector2Int pathCoordInt = new Vector2Int(Mathf.RoundToInt(pathCoord.x), Mathf.RoundToInt(pathCoord.y));
			//pathPoints.Add(pathCoordInt);

			for (int x = -SLGS.pathDepth; x < SLGS.pathDepth + 1; x++)
			{
				for (int y = -SLGS.pathDepth; y < SLGS.pathDepth + 1; y++)
				{
					Vector2Int newCoord = new Vector2Int(Mathf.RoundToInt(pathCoordInt.x + x), Mathf.RoundToInt(pathCoordInt.y + y));

					Transform overlappingTile;
					occupiedTiles.TryGetValue(newCoord, out overlappingTile);
					if (overlappingTile) Destroy(overlappingTile.gameObject);

					if (pathPoints.Contains(newCoord) == false)
					{
						GameObject newPathPointGO = new GameObject($"PathPoint[{newCoord.x}][{newCoord.y}]");
						newPathPointGO.transform.position = new Vector3(newCoord.x, newCoord.y, 0);
						newPathPointGO.transform.parent = pathPointParentGO.transform;
						pathPoints.Add(newCoord);
					}
				}
			}
		}

		diagnosticTimes.Add($"Pathfinder took: {p.duration}ms");
		totalGenerationTimeInMilliseconds += (long)p.duration;
	}

	/// <summary>
	/// Configures all the pathways. This includes, but is not limited to: Setting the correct sprites depeninding on their neighbouring tiles.
	/// </summary>
	/// <returns></returns>
	private IEnumerator ConfigurePathways()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		WeightedRandomList<Sprite> floorSprites = SLGS.floorSprites;
		List<Sprite> topWallSprites = SLGS.topWallSprites;
		List<Sprite> bottomWallSprites = SLGS.bottomWallSprites;
		List<Sprite> leftWallSprites = SLGS.leftWallSprites;
		List<Sprite> rightWallSprites = SLGS.rightWallSprites;
		List<Sprite> topLeftOuterCornerSprites = SLGS.topLeftOuterCornerSprites;
		List<Sprite> topRightOuterCornerSprites = SLGS.topRightOuterCornerSprites;
		List<Sprite> bottomRightOuterCornerSprites = SLGS.bottomRightOuterCornerSprites;
		List<Sprite> bottomLeftOuterCornerSprites = SLGS.bottomLeftOuterCornerSprites;
		List<Sprite> topLeftInnerCornerSprites = SLGS.topLeftInnerCornerSprites;
		List<Sprite> topRightInnerCornerSprites = SLGS.topRightInnerCornerSprites;
		List<Sprite> bottomRightInnerCornerSprites = SLGS.bottomRightInnerCornerSprites;
		List<Sprite> bottomLeftInnerCornerSprites = SLGS.bottomLeftInnerCornerSprites;

		for (int i = 0; i < pathwayParents.Count; i++)
		{
			Transform pathParent = pathwayParents[i].transform;
			Dictionary<Vector2Int, Transform> childPathTiles = new Dictionary<Vector2Int, Transform>();

			Transform[] childPathTilesTransforms = pathParent.GetComponentsInChildren<Transform>();
			foreach (Transform childPathTileTransform in childPathTilesTransforms)
			{
				if (childPathTileTransform != pathParent)
				{
					Vector2Int childPathTileCoordinate = new Vector2Int(Mathf.RoundToInt(childPathTileTransform.position.x), Mathf.RoundToInt(childPathTileTransform.position.y));
					childPathTiles.TryAdd(childPathTileCoordinate, childPathTileTransform);
				}
			}

			Room roomA = rooms[i];
			Room roomB = rooms[i + 1];

			// Only add the collideable tiles from the origin room per default.
			// For the last room we do need to add both room collideable tiles since this is the last pass.
			foreach (KeyValuePair<Vector2Int, Transform> collideableTile in roomA.CollideableTiles)
			{
				childPathTiles.TryAdd(collideableTile.Key, collideableTile.Value);
			}
			if (i == pathwayParents.Count - 1)
			{
				foreach (KeyValuePair<Vector2Int, Transform> collideableTile in roomB.CollideableTiles)
				{
					childPathTiles.TryAdd(collideableTile.Key, collideableTile.Value);
				}
			}

			foreach (KeyValuePair<Vector2Int, Transform> pathTile in childPathTiles)
			{
				Vector2Int pathTileCoordinate = pathTile.Key;
				Transform pathTileTransform = pathTile.Value;

				SpriteRenderer spriteRenderer = pathTileTransform.GetComponent<SpriteRenderer>();
				BoxCollider2D boxCollider2D = pathTileTransform.GetComponent<BoxCollider2D>();
				pathTileTransform.gameObject.layer = LayerMask.NameToLayer("Unwalkable");

				if (spriteRenderer == null)
				{
					spriteRenderer = pathTileTransform.AddComponent<SpriteRenderer>();
				}
				if (boxCollider2D == null)
				{
					boxCollider2D = pathTileTransform.AddComponent<BoxCollider2D>();
					boxCollider2D.size = Vector2.one;
				}

				//spriteRenderer.sortingLayerName = "Wall";
				spriteRenderer.material = defaultSpriteMaterial;

				Vector2Int pathTileCoord = new Vector2Int(Mathf.RoundToInt(pathTileTransform.position.x), Mathf.RoundToInt(pathTileTransform.position.y));

				Vector2Int topTileCoord = new Vector2Int(pathTileCoord.x, pathTileCoord.y + 1);
				Vector2Int topRightTileCoord = new Vector2Int(pathTileCoord.x + 1, pathTileCoord.y + 1);
				Vector2Int rightTileCoord = new Vector2Int(pathTileCoord.x + 1, pathTileCoord.y);
				Vector2Int bottomRightTileCoord = new Vector2Int(pathTileCoord.x + 1, pathTileCoord.y - 1);
				Vector2Int bottomTileCoord = new Vector2Int(pathTileCoord.x, pathTileCoord.y - 1);
				Vector2Int bottomLeftTileCoord = new Vector2Int(pathTileCoord.x - 1, pathTileCoord.y - 1);
				Vector2Int leftTileCoord = new Vector2Int(pathTileCoord.x - 1, pathTileCoord.y);
				Vector2Int topLeftTileCoord = new Vector2Int(pathTileCoord.x - 1, pathTileCoord.y + 1);

				Transform topTile = null;
				Transform topRightTile = null;
				Transform rightTile = null;
				Transform bottomRightTile = null;
				Transform bottomTile = null;
				Transform bottomLeftTile = null;
				Transform leftTile = null;
				Transform topLeftTile = null;

				Dictionary<Vector2Int, Transform> occupiedTiles = new Dictionary<Vector2Int, Transform>();
				foreach (KeyValuePair<Vector2Int, Transform> childPathTile in childPathTiles)
				{
					occupiedTiles.TryAdd(childPathTile.Key, childPathTile.Value);
				}
				foreach (KeyValuePair<Vector2Int, Transform> collideableTileA in roomA.CollideableTiles)
				{
					occupiedTiles.TryAdd(collideableTileA.Key, collideableTileA.Value);
				}
				foreach (KeyValuePair<Vector2Int, Transform> nonCollideableTileA in roomA.NoncollideableTiles)
				{
					occupiedTiles.TryAdd(nonCollideableTileA.Key, nonCollideableTileA.Value);
				}
				foreach (KeyValuePair<Vector2Int, Transform> collideableTileB in roomB.CollideableTiles)
				{
					occupiedTiles.TryAdd(collideableTileB.Key, collideableTileB.Value);
				}
				foreach (KeyValuePair<Vector2Int, Transform> nonCollideableTileB in roomB.NoncollideableTiles)
				{
					occupiedTiles.TryAdd(nonCollideableTileB.Key, nonCollideableTileB.Value);
				}

				if (i < pathwayParents.Count - 1)
				{
					Transform pathParentA = pathwayParents[i + 1].transform;

					Transform[] childPathTilesTransformsA = pathParentA.GetComponentsInChildren<Transform>();
					foreach (Transform childPathTileTransformA in childPathTilesTransformsA)
					{
						if (childPathTileTransformA != pathParentA)
						{
							Vector2Int childPathTileCoordinateA = new Vector2Int(Mathf.RoundToInt(childPathTileTransformA.position.x), Mathf.RoundToInt(childPathTileTransformA.position.y));
							occupiedTiles.TryAdd(childPathTileCoordinateA, childPathTileTransformA);
						}
					}
				}
				if (i > 0)
				{
					Transform pathParentB = pathwayParents[i - 1].transform;

					Transform[] childPathTilesTransformsB = pathParentB.GetComponentsInChildren<Transform>();
					foreach (Transform childPathTileTransformB in childPathTilesTransformsB)
					{
						if (childPathTileTransformB != pathParentB)
						{
							Vector2Int childPathTileCoordinateB = new Vector2Int(Mathf.RoundToInt(childPathTileTransformB.position.x), Mathf.RoundToInt(childPathTileTransformB.position.y));
							occupiedTiles.TryAdd(childPathTileCoordinateB, childPathTileTransformB);
						}
					}
				}

				occupiedTiles.TryGetValue(topTileCoord, out topTile);
				occupiedTiles.TryGetValue(topRightTileCoord, out topRightTile);
				occupiedTiles.TryGetValue(rightTileCoord, out rightTile);
				occupiedTiles.TryGetValue(bottomRightTileCoord, out bottomRightTile);
				occupiedTiles.TryGetValue(bottomTileCoord, out bottomTile);
				occupiedTiles.TryGetValue(bottomLeftTileCoord, out bottomLeftTile);
				occupiedTiles.TryGetValue(leftTileCoord, out leftTile);
				occupiedTiles.TryGetValue(topLeftTileCoord, out topLeftTile);

				// Floor Tile
				if (topTile && topRightTile && rightTile && bottomRightTile && bottomTile && bottomLeftTile && leftTile && topLeftTile)
				{
					pathTileTransform.gameObject.layer = LayerMask.NameToLayer("Walkable");
					pathTileTransform.name = "Floor Tile";
					spriteRenderer.sprite = floorSprites.GetRandom();
					//spriteRenderer.sortingLayerName = "Floor";
					Destroy(boxCollider2D);
				}

				#region Cardinal Walls
				// Top Wall
				else if (rightTile && bottomRightTile && bottomTile && bottomLeftTile && leftTile)
				{
					pathTileTransform.name = "Top Wall";
					spriteRenderer.sprite = topWallSprites[Random.Range(0, topWallSprites.Count)];
				}
				// Bottom Wall
				else if (!bottomTile && leftTile && topTile && rightTile)
				{
					pathTileTransform.name = "Bottom Wall";
					spriteRenderer.sprite = bottomWallSprites[Random.Range(0, bottomWallSprites.Count)];
					boxCollider2D.offset = new Vector2(0, -0.25f);
					boxCollider2D.size = new Vector2(1, 0.5f);
				}
				// Left Wall
				else if (!leftTile && topTile && topRightTile && rightTile && bottomRightTile && bottomTile)
				{
					pathTileTransform.name = "Left Wall";
					spriteRenderer.sprite = leftWallSprites[Random.Range(0, leftWallSprites.Count)];
				}
				// Right Wall
				else if (!rightTile && bottomTile && bottomLeftTile && leftTile & topLeftTile && topTile)
				{
					pathTileTransform.name = "Right Wall";
					spriteRenderer.sprite = rightWallSprites[Random.Range(0, rightWallSprites.Count)];
				}
				#endregion
				#region Outer Corners
				// Top Left Outer Corner
				else if (!leftTile && !topLeftTile && !topTile && rightTile && bottomRightTile && bottomTile)
				{
					pathTileTransform.name = "Top Left Outer Corner";
					spriteRenderer.sprite = topLeftOuterCornerSprites[Random.Range(0, topLeftOuterCornerSprites.Count)];
					spriteRenderer.flipY = true;
					boxCollider2D.offset = new Vector2(0.25f, 0);
					boxCollider2D.size = new Vector2(0.5f, 1);
				}
				// Top Right Outer Corner
				else if (!topTile && !topRightTile && !rightTile && bottomTile && bottomLeftTile && leftTile)
				{
					pathTileTransform.name = "Top Right Outer Corner";
					spriteRenderer.sprite = topRightOuterCornerSprites[Random.Range(0, topRightOuterCornerSprites.Count)];
					spriteRenderer.flipY = true;
					boxCollider2D.offset = new Vector2(-0.25f, 0);
					boxCollider2D.size = new Vector2(0.5f, 1);
				}
				// Bottom Right Outer Corner
				else if (!rightTile && !bottomRightTile && !bottomTile && leftTile && topLeftTile && topTile)
				{
					pathTileTransform.name = "Bottom Right Outer Corner";
					spriteRenderer.sprite = bottomRightOuterCornerSprites[Random.Range(0, bottomRightOuterCornerSprites.Count)];
				}
				// Bottom Left Outer Corner
				else if (!bottomTile && !bottomLeftTile && !leftTile && topTile && topRightTile && rightTile)
				{
					pathTileTransform.name = "Bottom Left Outer Corner";
					spriteRenderer.sprite = bottomLeftOuterCornerSprites[Random.Range(0, bottomLeftOuterCornerSprites.Count)];
				}
				#endregion
				#region Inner Corners
				// Top Left Inner Corner
				else if (!topLeftTile && topTile && topRightTile && rightTile && bottomRightTile && bottomTile && bottomLeftTile && leftTile)
				{
					pathTileTransform.name = "Top Left Inner Corner";
					spriteRenderer.sprite = topLeftInnerCornerSprites[Random.Range(0, topLeftInnerCornerSprites.Count)];
					spriteRenderer.flipY = true;
				}
				// Top Right Inner Corner
				else if (!topRightTile && rightTile && bottomRightTile && bottomTile && bottomLeftTile && leftTile && topLeftTile && topLeftTile)
				{
					pathTileTransform.name = "Top Right Inner Corner";
					spriteRenderer.sprite = topRightInnerCornerSprites[Random.Range(0, topRightInnerCornerSprites.Count)];
					spriteRenderer.flipX = true;
					spriteRenderer.flipY = true;
				}
				// Bottom Right Inner Corner
				else if (!bottomRightTile && bottomTile && bottomLeftTile && leftTile && topLeftTile && topTile && topRightTile && rightTile)
				{
					pathTileTransform.name = "Bottom Right Inner Corner";
					spriteRenderer.sprite = bottomRightInnerCornerSprites[Random.Range(0, bottomRightInnerCornerSprites.Count)];
					spriteRenderer.flipX = true;
					boxCollider2D.offset = new Vector2(0, -0.25f);
					boxCollider2D.size = new Vector2(1, 0.5f);
				}
				// Bottom Left Inner Corner
				else if (!bottomLeftTile && leftTile && topLeftTile && topTile && topRightTile && rightTile && bottomRightTile && bottomTile)
				{
					pathTileTransform.name = "Bottom Left Inner Corner";
					spriteRenderer.sprite = bottomLeftInnerCornerSprites[Random.Range(0, bottomLeftInnerCornerSprites.Count)];
					spriteRenderer.flipX = true;
					boxCollider2D.offset = new Vector2(0, -0.25f);
					boxCollider2D.size = new Vector2(1, 0.5f);
				}
				#endregion
			}
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Configuring pathways took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
	}

	/// <summary>
	/// Spawn enemies by group and type in the dungeon.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SpawnEnemies()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		#region Fodder Enemy Spawning
		foreach (Room room in rooms)
		{
			if (room.RoomType != RoomType.Spawn && room.RoomType != RoomType.Boss)
			{
				// Get enemyGroup Data
				EnemyGroup fodderEnemyGroup = SLGS.EnemyFodderGroup;
				if (fodderEnemyGroup.enemyType == EnemyType.Fodder)
				{
					// Get random value for the amount of groups in the room
					int groupCount = Random.Range(fodderEnemyGroup.groupCountPerRoom.x, fodderEnemyGroup.groupCountPerRoom.y + 1);
					for (int gc = 0; gc < groupCount; gc++)
					{
						// Get random value for the amount of enemies in the group
						int enemyCount = Random.Range(fodderEnemyGroup.enemyCountPerGroup.x, fodderEnemyGroup.enemyCountPerGroup.y + 1);
						int enemyCountSqrt = Mathf.FloorToInt(Mathf.Sqrt(enemyCount)) / 2;

						// Get spawn position of the group origin.
						int randTileIndex = Random.Range(0, room.NoncollideableTiles.Count);
						Transform randTileTransform = room.NoncollideableTiles.ElementAt(randTileIndex).Value;
						Vector2Int groupSpawnOriginCoordinates = new Vector2Int(Mathf.RoundToInt(randTileTransform.position.x), Mathf.RoundToInt(randTileTransform.position.y));

						for (int x = -enemyCountSqrt; x < enemyCountSqrt + 1; x++)
						{
							for (int y = -enemyCountSqrt; y < enemyCountSqrt + 1; y++)
							{
								// Check in the list of nonCollideableTiles in the room if the enemy spawn coordinate is equal to a tile in the list.
								Vector2Int enemySpawnCoordinate = new Vector2Int(groupSpawnOriginCoordinates.x + x, groupSpawnOriginCoordinates.y + y);

								if (room.NoncollideableTiles.ContainsKey(enemySpawnCoordinate))
								{
									GameObject enemyPrefab = fodderEnemyGroup.enemyPrefabs.GetRandom();
									Instantiate(enemyPrefab, new Vector2(enemySpawnCoordinate.x, enemySpawnCoordinate.y), Quaternion.identity, room.transform);
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Reward Enemy Spawning
		EnemyGroup rewardEnemyGroup = SLGS.EnemyRewardGroup;
		List<int> roomIndecesForRewardEnemySpawning = new List<int>();
		List<int> roomIndeces = new List<int>();

		// not the best way of ensuring reward enemies spawn in unique rooms... But it works :)
		for (int i = 0; i < rooms.Count; i++)
		{
			if (rooms[i].RoomType == RoomType.Generic)
			{
				roomIndeces.Add(i);
			}
		}
		for (int re = 0; re < rewardEnemyGroup.enemyPrefabs.Count; re++)
		{
			if (roomIndeces.Count > 0)
			{
				int randIndex = Random.Range(0, roomIndeces.Count);
				roomIndecesForRewardEnemySpawning.Add(roomIndeces[randIndex]);
				roomIndeces.RemoveAt(randIndex);
			}
		}
		foreach (int index in roomIndecesForRewardEnemySpawning)
		{
			Room room = rooms[index];
			Transform tileToSpawnEnemyUpon = room.NoncollideableTiles.ElementAt(Random.Range(0, room.NoncollideableTiles.Values.Count)).Value;
			Vector2Int enemySpawnPoint = new Vector2Int(Mathf.RoundToInt(tileToSpawnEnemyUpon.position.x), Mathf.RoundToInt(tileToSpawnEnemyUpon.position.y));

			GameObject enemyPrefab = rewardEnemyGroup.enemyPrefabs.GetRandom();
			GameObject enemyGO = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, enemySpawnPoint.y, 0), Quaternion.identity, room.transform);
		}
		#endregion

		executionTime.Stop();
		diagnosticTimes.Add($"Spawning enemies in rooms took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
	}

	/// <summary>
	/// A final pass for the level generator. This handles decorating everything that needs decorating.
	/// </summary>
	/// <returns></returns>
	private IEnumerator DecorateLevel()
	{
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		// Spawn a portal in the boss room.
		Room finalRoom = rooms[rooms.Count - 1];
		Vector2Int portalSpawnPoint = new Vector2Int(Mathf.RoundToInt(finalRoom.transform.position.x), Mathf.RoundToInt(finalRoom.transform.position.y));
		GameObject portalGO = Instantiate(bossFightPortal, new Vector2(portalSpawnPoint.x, portalSpawnPoint.y), Quaternion.identity, finalRoom.PropsParent);
		Portal portal = portalGO.GetComponent<Portal>();
		portal.SceneToLoadName = "Boss Testing";
		portal.CurrentSceneName = "Jaydee Testing Scene";

		// Spawn props throughout the level
		for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
		{
			Room room = rooms[roomIndex];
			if (room.RoomType == RoomType.Boss) break;

			// Spawn Wall props
			foreach (KeyValuePair<Vector2Int, Transform> collideableTile in room.CollideableTiles)
			{
				int randDecorationSpawnChance = Random.Range(0, 100);
				if (randDecorationSpawnChance < SLGS.decorationSpawnChance)
				{
					Transform collideableTileTransform = collideableTile.Value;
					Vector3 randPos = new Vector3(Mathf.RoundToInt(collideableTileTransform.position.x), Mathf.RoundToInt(collideableTileTransform.position.y), 0);

					// Top wall props
					if (collideableTileTransform.name.ToLower().Contains("top") && !collideableTileTransform.name.ToLower().Contains("corner"))
					{
						int randDecorationIndex = Random.Range(0, SLGS.topWallDecorations.Count);
						GameObject decorationGO = Instantiate(SLGS.topWallDecorations[randDecorationIndex].prefab, randPos, Quaternion.identity, room.PropsParent);
						decorations.Add(decorationGO);
					}
					// Left wall props
					else if (collideableTileTransform.name.ToLower().Contains("left") && !collideableTileTransform.name.ToLower().Contains("corner"))
					{
						int randDecorationIndex = Random.Range(0, SLGS.leftWallDecorations.Count);
						GameObject decorationGO = Instantiate(SLGS.leftWallDecorations[randDecorationIndex].prefab, new Vector3(randPos.x + 1, randPos.y, randPos.z), Quaternion.identity, room.PropsParent);
						decorations.Add(decorationGO);
					}
					// Right wall props
					else if (collideableTileTransform.name.ToLower().Contains("right") && !collideableTileTransform.name.ToLower().Contains("corner"))
					{
						int randDecorationIndex = Random.Range(0, SLGS.rightWallDecorations.Count);
						GameObject decorationGO = Instantiate(SLGS.rightWallDecorations[randDecorationIndex].prefab, new Vector3(randPos.x - 1, randPos.y, randPos.z), Quaternion.identity, room.PropsParent);
						decorations.Add(decorationGO);
					}
				}
			}

			// Spawn Floor props
			foreach (KeyValuePair<Vector2Int, Transform> noncollideableTile in room.NoncollideableTiles)
			{
				int randDecorationSpawnChance = Random.Range(0, 100);
				Transform nonCollideableTileTransform = noncollideableTile.Value;

				if (randDecorationSpawnChance < SLGS.decorationSpawnChance)
				{
					int randDecorationIndex = Random.Range(0, SLGS.floorDecorations.Count);
					Vector3 randPos = new Vector3(Mathf.RoundToInt(nonCollideableTileTransform.position.x), Mathf.RoundToInt(nonCollideableTileTransform.position.y), 0);
					GameObject decorationGO = Instantiate(SLGS.floorDecorations[randDecorationIndex].prefab, randPos, Quaternion.identity, room.PropsParent);

					decorations.Add(decorationGO);
				}
			}
		}

		// Spawn Levelup Statue
		// Spawn the Levelup statue only in the last half of the rooms.
		int levelUpStatueRoomIndex = Random.Range(rooms.Count / 2, rooms.Count);
		Room roomWithLevelupStatue = rooms[levelUpStatueRoomIndex];
		int levelUpStatueSpawnPointIndex = Random.Range(0, roomWithLevelupStatue.StatuePoints.Count);
		Transform levelUpStatueSpawnPointTransform = roomWithLevelupStatue.StatuePoints[levelUpStatueSpawnPointIndex].transform;
		Vector2Int levelUpStatueCoordinates = new Vector2Int(Mathf.RoundToInt(levelUpStatueSpawnPointTransform.position.x), Mathf.RoundToInt(levelUpStatueSpawnPointTransform.position.y));
		GameObject levelupStatueGO = Instantiate(levelUpStatue, new Vector3(levelUpStatueCoordinates.x, levelUpStatueCoordinates.y, 0), Quaternion.identity, roomWithLevelupStatue.PropsParent);
		decorations.Add(levelupStatueGO);

		// A final scan.
		AstarPath.active.Scan();
		while (AstarPath.active.IsAnyGraphUpdateInProgress)
		{
			yield return null;
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Decorating the level took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return null;
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

		Chunk topNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX, chunkY + SLGS.chunkSize));
		Chunk rightNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX + SLGS.chunkSize, chunkY));
		Chunk bottomNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX, chunkY - SLGS.chunkSize));
		Chunk leftNeighbour = GetChunkByCoordinates(new Vector2Int(chunkX - SLGS.chunkSize, chunkY));

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

	/// <summary>
	/// Gets the Bounds of all transforms in the list.
	/// </summary>
	/// <param name="transforms"> List of transforms to get Bounds from. </param>
	/// <returns> Bounds. </returns>
	public Bounds GetBoundsFromTransforms(List<Transform> transforms)
	{
		Bounds bound = new Bounds(transforms[0].position, Vector3.zero);
		for (int i = 1; i < transforms.Count; i++)
		{
			bound.Encapsulate(transforms[i].position);
		}
		return bound;
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
				Gizmos.DrawWireCube(chunk.gameObject.transform.position, new Vector3(SLGS.chunkSize, SLGS.chunkSize));
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