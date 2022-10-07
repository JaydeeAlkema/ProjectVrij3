using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
	[SerializeField] private int pathWidth = 3;
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
	[SerializeField] private Transform levelAssetsParent = default;

	[Header("Debugging")]
	[SerializeField] private List<string> diagnosticTimes = new List<string>();
	[SerializeField] private long totalGenerationTimeInMilliseconds = 0;

	List<Node> pathPoints = new List<Node>();

	private void Start()
	{
		if (seed == 0)
		{
			seed = Random.Range(0, int.MaxValue);
		}

		Random.InitState(seed);

		StartCoroutine(GenerateLevel());
	}

	private IEnumerator GenerateLevel()
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
				chunk.gameObject.transform.parent = levelAssetsParent;

				// Create starting chunk node grid (the +1 comes from the fact that the nodes start on the edges of the chunk, not within the chunk)
				for (int nx = 0; nx < chunkSize + 1; nx++)
				{
					for (int ny = 0; ny < chunkSize + 1; ny++)
					{
						Vector2Int nodeCoordinates = new Vector2Int(Mathf.RoundToInt(chunk.transform.position.x - chunkSize / 2) + nx, Mathf.RoundToInt(chunk.transform.position.y - chunkSize / 2) + ny);

						GameObject nodeGO = new GameObject($"Node [{nodeCoordinates.x}][{nodeCoordinates.y}]");
						Node node = nodeGO.AddComponent<Node>();

						nodeGO.transform.position = new Vector2(nodeCoordinates.x, nodeCoordinates.y);
						nodeGO.transform.parent = chunk.transform;

						node.walkable = true;
						node.coordinates = new Vector2Int(Mathf.RoundToInt(nodeGO.transform.position.x), Mathf.RoundToInt(nodeGO.transform.position.y));

						chunk.Nodes.Add(node);
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

			newRoomGO.transform.parent = levelAssetsParent;
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

			// Actually build the pathways here...
			Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(pathwayStartPoint.transform.position.x), Mathf.RoundToInt(pathwayStartPoint.transform.position.y));
			Vector2Int targetPos = new Vector2Int(Mathf.RoundToInt(pathwayEndPoint.transform.position.x), Mathf.RoundToInt(pathwayEndPoint.transform.position.y));

			// Get the two chunks that need to connect their rooms together
			Chunk startChunk = null;
			Chunk endChunk = null;

			foreach (Chunk chunk in chunks)
			{
				if (chunk.Room == room)
				{
					startChunk = chunk;
				}
				if (chunk.Room == roomToConnectTo)
				{
					endChunk = chunk;
				}
			}

			// Get all the nodes between the two chunks
			List<Node> nodes = new List<Node>();
			nodes.AddRange(startChunk.Nodes);
			nodes.AddRange(endChunk.Nodes);

			// Remove any duplicate nodes.
			List<Node> nodesNoDuplicates = nodes.Distinct(new NodeComparer()).ToList();
			List<Transform> collideableTiles = new List<Transform>();
			collideableTiles.AddRange(room.CollideableTiles);
			collideableTiles.AddRange(roomToConnectTo.CollideableTiles);

			// Set all nodes walkeable variable that have the same coordinates of the collideable tiles as false.
			foreach (Transform collideableTile in collideableTiles)
			{
				if (collideableTile.gameObject.activeInHierarchy || collideableTile.parent.gameObject.activeInHierarchy)
				{
					Vector2Int wallCoordinates = new Vector2Int(Mathf.RoundToInt(collideableTile.position.x), Mathf.RoundToInt(collideableTile.position.y));
					Node node = nodesNoDuplicates.Find(n => n.coordinates == wallCoordinates);
					if (node != null) node.walkable = false;
				}
			}

			FindShortestPathBetweenRoomsWithAStar(startPos, targetPos, nodesNoDuplicates);

			// Instantiate empty pathway tiles.
			if (pathPoints.Count > 0)
			{
				GameObject pathParent = new GameObject($"Pathway [{r}]");
				pathParent.transform.parent = levelAssetsParent;
				int totalX = 0;
				int totalY = 0;
				foreach (Node pathPoint in pathPoints)
				{
					totalX += pathPoint.coordinates.x;
					totalY += pathPoint.coordinates.y;
				}
				int centerX = totalX / pathPoints.Count;
				int centerY = totalY / pathPoints.Count;
				pathParent.transform.position = new Vector2(centerX, centerY);

				// Remove all duplicates. Maybe use a HashSet in the future.
				List<Node> pathPointsNoDuplicates = pathPoints.Distinct().ToList();
				for (int p = pathPointsNoDuplicates.Count - 1; p >= 0; p--)
				{
					Node pathPoint = pathPointsNoDuplicates[p];
					Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2Int(pathPoint.coordinates.x, pathPoint.coordinates.y), new Vector2(0.75f, 0.75f), 0f);
					if (colliders.Length > 0)
					{
						pathPointsNoDuplicates.Remove(pathPoint);
					}
				}

				// This will a fairly expensive operation. We will have to check for each pathPoint, it's neighbouring points (Not to be confused with nodes)
				// Then depending on the amount of neighbours and orientation, the tile sprite will be chosen and rotated accordingly.
				for (int pp = 0; pp < pathPointsNoDuplicates.Count; pp++)
				{
					// The "Origin" node is always surrounded by other nodes. So the origin node will always be a ground tile.
					Node originNode = pathPointsNoDuplicates[pp];
					GameObject pathPoint = new GameObject($"Point [{pp}]");
					pathPoint.transform.position = new Vector3(originNode.coordinates.x, originNode.coordinates.y, 0);
					pathPoint.transform.parent = pathParent.transform;

					SpriteRenderer spriteRenderer = pathPoint.AddComponent<SpriteRenderer>();
					spriteRenderer.sprite = pathGroundTileSprites[Random.Range(0, pathGroundTileSprites.Count)];
					spriteRenderer.color = Color.blue; // For debugging purposes only!

					// Add extra path to make the pathway wider.
					for (int x = -(pathWidth / 2); x < (pathWidth / 2) + 1; x++)
					{
						for (int y = -(pathWidth / 2); y < (pathWidth / 2) + 1; y++)
						{
							Vector2Int coordinates = new Vector2Int(originNode.coordinates.x + x, originNode.coordinates.y + y);
							Node node = nodesNoDuplicates.Find(n => n.coordinates == coordinates);

							if (node != null && !pathPointsNoDuplicates.Contains(node) && node.walkable)
							{
								GameObject neighbouringPathPoint = new GameObject($"Point [{pp}]");
								SpriteRenderer neighbouringPathPointSpriteRenderer = neighbouringPathPoint.AddComponent<SpriteRenderer>();
								neighbouringPathPoint.transform.position = new Vector3(node.coordinates.x, node.coordinates.y, 0);
								neighbouringPathPoint.transform.parent = pathParent.transform;

								//Vector2Int topNeighbourCoordinates = new Vector2Int(node.coordinates.x, node.coordinates.y + 1);
								//Vector2Int topRightNeighbourCoordinates = new Vector2Int(node.coordinates.x + 1, node.coordinates.y + 1);
								//Vector2Int rightNeighbourCoordinates = new Vector2Int(node.coordinates.x + 1, node.coordinates.y);
								//Vector2Int bottomRightNeighbourCoordinates = new Vector2Int(node.coordinates.x + 1, node.coordinates.y - 1);
								//Vector2Int bottomNeighbourCoordinates = new Vector2Int(node.coordinates.x, node.coordinates.y - 1);
								//Vector2Int bottomLeftNeighbourCoordinates = new Vector2Int(node.coordinates.x - 1, node.coordinates.y - 1);
								//Vector2Int leftNeighbourCoordinates = new Vector2Int(node.coordinates.x - 1, node.coordinates.y);
								//Vector2Int topLeftNeighbourCoordinates = new Vector2Int(node.coordinates.x - 1, node.coordinates.y + 1);

								//List<Node> neighbours = new List<Node>();
								//Node topNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == topNeighbourCoordinates);
								//Node topRightNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == topRightNeighbourCoordinates);
								//Node rightNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == rightNeighbourCoordinates);
								//Node bottomRightNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == bottomRightNeighbourCoordinates);
								//Node bottomNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == bottomNeighbourCoordinates);
								//Node bottomLeftNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == bottomLeftNeighbourCoordinates);
								//Node leftNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == leftNeighbourCoordinates);
								//Node topLeftNeighbourNode = pathPointsNoDuplicates.Find(p => p.coordinates == topLeftNeighbourCoordinates);

								//if (topNeighbourNode) neighbours.Add(topNeighbourNode);
								//if (topRightNeighbourNode) neighbours.Add(topRightNeighbourNode);
								//if (rightNeighbourNode) neighbours.Add(rightNeighbourNode);
								//if (bottomRightNeighbourNode) neighbours.Add(bottomRightNeighbourNode);
								//if (bottomNeighbourNode) neighbours.Add(bottomNeighbourNode);
								//if (bottomLeftNeighbourNode) neighbours.Add(bottomLeftNeighbourNode);
								//if (leftNeighbourNode) neighbours.Add(leftNeighbourNode);
								//if (topLeftNeighbourNode) neighbours.Add(topLeftNeighbourNode);

								// Node has 4 adjecent neighbours. This should be a ground node.
								//if (topNeighbourNode && rightNeighbourNode && bottomNeighbourNode && leftNeighbourNode)
								//{
								neighbouringPathPointSpriteRenderer.sprite = pathGroundTileSprites[Random.Range(0, pathGroundTileSprites.Count)];
								//}
								//else
								//{
								//	neighbouringPathPointSpriteRenderer.sprite = pathWallTileSprites[Random.Range(0, pathGroundTileSprites.Count)];
								//}
							}
						}
					}
				}
			}
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Generating pathways between empty rooms took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
	}

	#region A* Functions
	/// <summary>
	/// This finds the shortest path between startPos and targetPos using A*
	/// </summary>
	/// <param name="startPos"> starting position. </param>
	/// <param name="targetPos"> target position. </param>
	/// <param name="nodesNoDuplicates"> List of nodes to pathfind through. </param>
	private void FindShortestPathBetweenRoomsWithAStar(Vector2Int startPos, Vector2Int targetPos, List<Node> nodesNoDuplicates)
	{
		Node startNode = nodesNoDuplicates.Find(n => n.coordinates == startPos);
		Node targetNode = nodesNoDuplicates.Find(n => n.coordinates == targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(startNode);
		pathPoints.Add(startNode);
		pathPoints.Add(targetNode);

		while (openSet.Count > 0)
		{
			Node currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
				{
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if (currentNode == targetNode)
			{
				RetracePath(startNode, targetNode);
				return;
			}

			List<Node> neighbourNodes = GetNeighbouringNodes(currentNode, nodesNoDuplicates);
			foreach (Node neighbourNode in neighbourNodes)
			{
				if (!neighbourNode.walkable || closedSet.Contains(neighbourNode)) continue;

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);
				if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))
				{
					neighbourNode.gCost = newMovementCostToNeighbour;
					neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
					neighbourNode.parent = currentNode;

					if (!openSet.Contains(neighbourNode))
					{
						openSet.Add(neighbourNode);
					}
				}
			}
		}

	}

	/// <summary>
	/// Get neighbouring nodes
	/// </summary>
	/// <param name="node"> Origin node. </param>
	/// <param name="nodes"> List of nodes that hold the neighbouring nodes. </param>
	/// <returns></returns>
	private List<Node> GetNeighbouringNodes(Node node, List<Node> nodes)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) continue;

				int checkX = node.coordinates.x + x;
				int checkY = node.coordinates.y + y;

				Node neighbourNode = nodes.Find(n => n.coordinates == new Vector2Int(checkX, checkY));
				if (neighbourNode != null)
					neighbours.Add(neighbourNode);
			}
		}

		return neighbours;
	}

	/// <summary>
	/// Get the distance between node A and node B.
	/// </summary>
	/// <param name="nodeA"> Starting node. </param>
	/// <param name="nodeB"> Target node. </param>
	/// <returns></returns>
	private int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.coordinates.x - nodeB.coordinates.x);
		int distY = Mathf.Abs(nodeA.coordinates.y - nodeB.coordinates.y);

		if (distX > distY)
		{
			return 14 * distY + 10 * (distX - distY);
		}
		return 14 * distX + 10 * (distY - distX);
	}

	/// <summary>
	/// Retrace the path from start to end. (and reverse the loop)
	/// </summary>
	/// <param name="startNode"> start point of the path. </param>
	/// <param name="endNode"> end point of the path. </param>
	private void RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		pathPoints.AddRange(path);
		pathPoints = pathPoints.Distinct().ToList();
	}
	#endregion

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

	// https://answers.unity.com/questions/444414/get-angle-between-2-vector2s.html
	public float GetAngle(Vector2 A, Vector2 B)
	{
		//difference
		var Delta = B - A;
		//use atan2 to get the angle; Atan2 returns radians
		var angleRadians = Mathf.Atan2(Delta.y, Delta.x);

		//convert to degrees
		var angleDegrees = angleRadians * Mathf.Rad2Deg;

		//angleDegrees will be in the range (-180,180].
		//I like normalizing to [0,360) myself, but this is optional..
		if (angleDegrees < 0)
			angleDegrees += 360;

		return angleDegrees;
	}
	bool Between(float angle, float A, float B)
	{
		if (angle < B && angle > A)
		{
			return true;
		}
		return false;
	}
	#endregion

	private void OnDrawGizmos()
	{
		if (chunks.Count > 0)
		{
			for (int i = 0; i < chunks.Count; i++)
			{
				Chunk chunk = chunks[i];
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(chunk.gameObject.transform.position, new Vector3(chunkSize, chunkSize));

				if (chunk.Nodes.Count == 0) return;
				foreach (Node node in chunk.Nodes)
				{
					Gizmos.color = node.walkable ? new Color(1, 1, 1, 0.1f) : new Color(1, 0, 0, 0.1f);
					Gizmos.DrawCube(new Vector3(node.coordinates.x, node.coordinates.y, 0), Vector3.one * 0.95f);
				}
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
	[SerializeField] private List<Node> nodes = new List<Node>();

	public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
	public bool Occupied { get => occupied; set => occupied = value; }
	public Room Room { get => room; set => room = value; }
	public List<Node> Nodes { get => nodes; set => nodes = value; }
}

public class Node : MonoBehaviour
{
	public bool walkable;
	public Vector2Int coordinates;

	public int gCost;
	public int hCost;
	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
	public Node parent;
}

public class NodeComparer : IEqualityComparer<Node>
{
	// Nodes are equal if their coordinates and walkeable bool are equal.
	public bool Equals(Node x, Node y)
	{
		//Check whether the compared objects reference the same data.
		if (Object.ReferenceEquals(x, y)) return true;

		//Check whether any of the compared objects is null.
		if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
			return false;

		//Check whether the nodes properties are equal.
		return x.coordinates == y.coordinates && x.walkable == y.walkable;
	}

	// If Equals() returns true for a pair of objects
	// then GetHashCode() must return the same value for these objects.

	public int GetHashCode(Node node)
	{
		//Check whether the object is null
		if (Object.ReferenceEquals(node, null)) return 0;

		//Get hash code for the Name field if it is not null.
		int hashProductName = node.coordinates == null ? 0 : node.coordinates.GetHashCode();

		//Get hash code for the Code field.
		int hashProductCode = node.walkable.GetHashCode();

		//Calculate the hash code for the product.
		return hashProductName ^ hashProductCode;
	}
}
