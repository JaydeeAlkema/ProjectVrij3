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

				chunk.gameObject.transform.position = new Vector2(x * chunkSize, y * chunkSize);
				chunk.gameObject.transform.parent = levelAssetsParent;

				chunks.Add(chunk);
			}
		}

		executionTime.Stop();
		Debug.Log($"Chunk Generation took: {executionTime.ElapsedMilliseconds}ms");

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
		Debug.Log($"Chunk path generation took: {executionTime.ElapsedMilliseconds}ms");

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
		Debug.Log($"Empty rooms generation took: {executionTime.ElapsedMilliseconds}ms");

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

			//TODO:
			// 1: The current way of making the pathways wider, is a bit of a shitty way to do it. This should honestly just be a single line of code, with the ability to instantly decide the width of the pathway.

			// We need to start of by adjusting the starting position by 1.
			// This is done by checking in which way we need to start to make our path.
			float angle = GetAngle(room.transform.position, roomToConnectTo.transform.position);
			if (Between(angle, 45, 135))
			{
				currentPos.y += 1;
				pathPoints.Add(currentPos);
				pathPoints.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
			}
			else if (Between(angle, 225, 315))
			{
				currentPos.y -= 1;
				pathPoints.Add(currentPos);
				pathPoints.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
				pathPoints.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
			}
			else if (Between(angle, 315, 360) || Between(angle, 0, 45))
			{
				currentPos.x += 1;
				pathPoints.Add(currentPos);
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 2));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
			}
			else if (Between(angle, 135, 225))
			{
				currentPos.x -= 1;
				pathPoints.Add(currentPos);
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 2));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
				pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
			}

			// Build the path from start to end
			while (currentPos != endPos)
			{
				if (currentPos.x < endPos.x)
				{
					currentPos.x += 1;
					pathPoints.Add(currentPos);
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 2));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
				}
				if (currentPos.x > endPos.x)
				{
					currentPos.x -= 1;
					pathPoints.Add(currentPos);
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 2));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
					pathPoints.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
				}
				if (currentPos.y < endPos.y)
				{
					currentPos.y += 1;
					pathPoints.Add(currentPos);
					pathPoints.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
				}
				if (currentPos.y > endPos.y)
				{
					currentPos.y -= 1;
					pathPoints.Add(currentPos);
					pathPoints.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
					pathPoints.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
				}
			}

			// Instantiate empty pathway tiles.
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
			for (int pp = 0; pp < pathPoints.Count; pp++)
			{
				Vector2Int point = pathPoints[pp];
				GameObject pathPoint = new GameObject($"Point [{pp}]");
				pathPoint.transform.position = new Vector3(point.x, point.y, 0);
				pathPoint.transform.parent = pathParent.transform;

				SpriteRenderer spriteRenderer = pathPoint.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = pathGroundTileSprites[Random.Range(0, pathGroundTileSprites.Count)];
				if (pp == 0)
				{
					spriteRenderer.color = Color.red;
				}
			}
		}

		executionTime.Stop();
		Debug.Log($"Generating pathways between empty rooms took: {executionTime.ElapsedMilliseconds}ms");

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
