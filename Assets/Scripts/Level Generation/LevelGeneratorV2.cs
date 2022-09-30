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

public enum PathwayGenerationType
{
	Organic,
	Straight
}

public class LevelGeneratorV2 : MonoBehaviour
{
	[Header("Level Generation Settings")]
	[SerializeField] private int seed;
	[SerializeField] private int chunkSize = 35;
	[SerializeField] private int maxRooms = 10;
	[SerializeField] private PathwayGenerationType pathwayGenerationType = PathwayGenerationType.Organic;
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

						GameObject nodeGO = new GameObject($"Node [{nx}][{ny}]");
						Node node = nodeGO.AddComponent<Node>();

						nodeGO.transform.position = new Vector2(nodeCoordinates.x, nodeCoordinates.y);
						nodeGO.transform.parent = chunk.transform;

						node.Walkable = true;
						node.Coordinates = new Vector2Int(Mathf.RoundToInt(nodeGO.transform.position.x), Mathf.RoundToInt(nodeGO.transform.position.y));

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

			int randX = Random.Range(chunk.Coordinates.x - chunkSizeHalf + roomsizeHalfX + 1, chunk.Coordinates.x + chunkSizeHalf - roomsizeHalfX);
			int randY = Random.Range(chunk.Coordinates.y - chunkSizeHalf + roomsizeHalfY + 1, chunk.Coordinates.y + chunkSizeHalf - roomsizeHalfY);

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
			foreach (GameObject pathwayConnectionPoint in room.PathwayEntries)
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
			foreach (GameObject pathwayConnectionPoint in roomToConnectTo.PathwayEntries)
			{
				float distance = Vector2.Distance(room.transform.position, pathwayConnectionPoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					pathwayEndPoint = pathwayConnectionPoint;
				}
			}

			// Disable starting pathway opening
			foreach (GameObject pathwayOpening in room.PathwayEntries)
			{
				if (pathwayOpening == pathwayStartPoint)
				{
					pathwayStartPoint.SetActive(false);
				}
			}
			// Disable ending pathway opening
			foreach (GameObject pathwayOpening in roomToConnectTo.PathwayEntries)
			{
				if (pathwayOpening == pathwayEndPoint)
				{
					pathwayEndPoint.SetActive(false);
				}
			}

			// Actually build the pathways here...
			Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(pathwayStartPoint.transform.position.x), Mathf.RoundToInt(pathwayStartPoint.transform.position.y));
			Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(pathwayEndPoint.transform.position.x), Mathf.RoundToInt(pathwayEndPoint.transform.position.y));
			Vector2Int currentPos = startPos;
			List<Vector2Int> pathPoints = new List<Vector2Int>();

			#region Old Pathfinding
			////TODO:
			//// 1: The current way of making the pathways wider, is a bit of a shitty way to do it. This should honestly just be a single line of code, with the ability to instantly decide the width of the pathway.

			//// We need to start of by adjusting the starting position by 1.
			//// This is done by checking in which way we need to start to make our path.
			//float angle = GetAngle(room.transform.position, roomToConnectTo.transform.position);
			//if (Between(angle, 45, 135))
			//{
			//	currentPos.y += 1;
			//	currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//}
			//else if (Between(angle, 225, 315))
			//{
			//	currentPos.y -= 1;
			//	currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//}
			//else if (Between(angle, 315, 360) || Between(angle, 0, 45))
			//{
			//	currentPos.x += 1;
			//	currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//}
			//else if (Between(angle, 135, 225))
			//{
			//	currentPos.x -= 1;
			//	currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//}

			//// Build the path from start to end
			//while (currentPos != endPos)
			//{
			//	switch (pathwayGenerationType)
			//	{
			//		case PathwayGenerationType.Organic:
			//			if (currentPos.x < endPos.x)
			//			{
			//				currentPos.x += 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//			}
			//			if (currentPos.x > endPos.x)
			//			{
			//				currentPos.x -= 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//			}
			//			if (currentPos.y < endPos.y)
			//			{
			//				currentPos.y += 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//			}
			//			if (currentPos.y > endPos.y)
			//			{
			//				currentPos.y -= 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//			}
			//			break;
			//		case PathwayGenerationType.Straight:
			//			if (currentPos.x < endPos.x)
			//			{
			//				currentPos.x += 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//			}
			//			else if (currentPos.x > endPos.x)
			//			{
			//				currentPos.x -= 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(0, 1));
			//			}
			//			else if (currentPos.y < endPos.y)
			//			{
			//				currentPos.y += 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//			}
			//			else if (currentPos.y > endPos.y)
			//			{
			//				currentPos.y -= 1;
			//				currentPos = AddPathTiles(currentPos, pathPoints, pathWidth, new Vector2Int(1, 0));
			//			}
			//			break;
			//		default:
			//			break;
			//	}
			//}
			#endregion

			#region New Pathfinding (A*)
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

			List<Node> nodes = new List<Node>();
			nodes.AddRange(startChunk.Nodes);
			nodes.AddRange(endChunk.Nodes);

			List<Node> nodesNoDuplicates = nodes.Distinct(new NodeComparer()).ToList();

			foreach (Vector2Int wall in room.CollideableTiles)
			{
				Node node = nodesNoDuplicates.Find(n => n.Coordinates == wall);
				if (node != null) node.Walkable = false;
			}
			foreach (Vector2Int wall in roomToConnectTo.CollideableTiles)
			{
				Node node = nodesNoDuplicates.Find(n => n.Coordinates == wall);
				if (node != null) node.Walkable = false;
			}
			#endregion

			// Instantiate empty pathway tiles.
			if (pathPoints.Count > 0)
			{
				GameObject pathParent = new GameObject($"Pathway [{r}]");
				pathParent.transform.parent = levelAssetsParent;
				int totalX = 0;
				int totalY = 0;
				foreach (Vector2Int pathPoint in pathPoints)
				{
					totalX += pathPoint.x;
					totalY += pathPoint.y;
				}
				int centerX = totalX / pathPoints.Count;
				int centerY = totalY / pathPoints.Count;
				pathParent.transform.position = new Vector2(centerX, centerY);

				// Remove all duplicates. Maybe use a HashSet in the future.
				pathPoints = pathPoints.Distinct().ToList();
				for (int p = pathPoints.Count - 1; p >= 0; p--)
				{
					Vector2Int pathPoint = pathPoints[p];
					Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2Int(pathPoint.x, pathPoint.y), new Vector2(0.75f, 0.75f), 0f);
					if (colliders.Length > 0)
					{
						pathPoints.Remove(pathPoint);
					}
				}

				for (int pp = 0; pp < pathPoints.Count; pp++)
				{
					Vector2Int point = pathPoints[pp];
					GameObject pathPoint = new GameObject($"Point [{pp}]");
					pathPoint.transform.position = new Vector3(point.x, point.y, 0);
					pathPoint.transform.parent = pathParent.transform;

					SpriteRenderer spriteRenderer = pathPoint.AddComponent<SpriteRenderer>();
					spriteRenderer.sprite = pathGroundTileSprites[Random.Range(0, pathGroundTileSprites.Count)];
				}
			}
		}

		executionTime.Stop();
		diagnosticTimes.Add($"Generating pathways between empty rooms took: {executionTime.ElapsedMilliseconds}ms");
		totalGenerationTimeInMilliseconds += executionTime.ElapsedMilliseconds;

		yield return new WaitForEndOfFrame();
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

	/// <summary>
	/// Adds extra pathPoints to the path tiles list to widen the path.
	/// </summary>
	/// <param name="currentPos"> Current pathPoint position.</param>
	/// <param name="pathPoints"> List to add the new positions to.</param>
	/// <param name="pathWidth"> Width of the path</param>
	/// <returns></returns>
	private static Vector2Int AddPathTiles(Vector2Int currentPos, List<Vector2Int> pathPoints, int pathWidth, Vector2Int dir)
	{
		for (int x = -pathWidth; x <= pathWidth; x++)
		{
			for (int y = -pathWidth; y <= pathWidth; y++)
			{
				pathPoints.Add(new Vector2Int(currentPos.x + x, currentPos.y + y));
			}
		}

		return currentPos;
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
					Gizmos.color = node.Walkable ? new Color(1, 1, 1, 0.1f) : new Color(1, 0, 0, 0.1f);
					Gizmos.DrawCube(new Vector3(node.Coordinates.x, node.Coordinates.y, 0), Vector3.one * 0.95f);
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
	[SerializeField] private bool walkable;
	[SerializeField] private Vector2Int coordinates;

	public Node(bool walkable, Vector2Int coordinates)
	{
		this.walkable = walkable;
		this.coordinates = coordinates;
	}

	public bool Walkable { get => walkable; set => walkable = value; }
	public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }


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
		return x.Coordinates == y.Coordinates && x.Walkable == y.Walkable;
	}

	// If Equals() returns true for a pair of objects
	// then GetHashCode() must return the same value for these objects.

	public int GetHashCode(Node node)
	{
		//Check whether the object is null
		if (Object.ReferenceEquals(node, null)) return 0;

		//Get hash code for the Name field if it is not null.
		int hashProductName = node.Coordinates == null ? 0 : node.Coordinates.GetHashCode();

		//Get hash code for the Code field.
		int hashProductCode = node.Walkable.GetHashCode();

		//Calculate the hash code for the product.
		return hashProductName ^ hashProductCode;
	}
}
