using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LevelGeneratorV2 : MonoBehaviour
{
	[Header("Level Generation Settings")]
	[SerializeField] private int seed;
	[SerializeField] private int chunkSize = 20;
	[SerializeField, InfoBox("The grid size may NEVER be divisible by 2")] private Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();
	[Space]
	[SerializeField] private List<Chunk> chunks = new List<Chunk>();
	[SerializeField] private List<Chunk> path = new List<Chunk>();
	[SerializeField] private List<GameObject> rooms = new List<GameObject>();
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
		Stopwatch executionTime = new Stopwatch();
		executionTime.Start();

		yield return StartCoroutine(CreateChunks());
		yield return StartCoroutine(CreatePath());
		yield return StartCoroutine(CreateEmptyRooms());

		executionTime.Stop();
		Debug.Log($"Level Generation took: {executionTime.ElapsedMilliseconds}ms");
	}

	/// <summary>
	/// Lays the chunks grid.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreateChunks()
	{
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

				chunk.gameObject.transform.parent = levelAssetsParent;
				chunk.gameObject.transform.localPosition = new Vector2(x * chunkSize, y * chunkSize);

				chunks.Add(chunk);
			}
		}
		yield return new WaitForEndOfFrame();
	}

	/// <summary>
	/// Create a path through the chunks, these chunks that are in the path will be used later to spawn rooms in.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreatePath()
	{
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
		}

		yield return new WaitForEndOfFrame();
	}

	private IEnumerator CreateEmptyRooms()
	{
		foreach (Chunk chunk in path)
		{
			GameObject randRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
			Vector2Int randRoomCoordinates = new Vector2Int(Random.Range(chunk.Coordinates.x - (chunkSize / 2), chunk.Coordinates.x + (chunkSize / 2)), Random.Range(chunk.Coordinates.y - (chunkSize / 2), chunk.Coordinates.y + (chunkSize / 2)));

			int tempRoomSize = 20;

			if (randRoomCoordinates.x + tempRoomSize >= chunk.Coordinates.x + chunkSize / 2) randRoomCoordinates.x -= 20;
			if (randRoomCoordinates.x - tempRoomSize <= chunk.Coordinates.x - chunkSize / 2) randRoomCoordinates.x += 20;
			if (randRoomCoordinates.y + tempRoomSize >= chunk.Coordinates.y + chunkSize / 2) randRoomCoordinates.y -= 20;
			if (randRoomCoordinates.y - tempRoomSize <= chunk.Coordinates.y - chunkSize / 2) randRoomCoordinates.y += 20;

			GameObject newRoomGO = Instantiate(randRoomPrefab, new Vector2(randRoomCoordinates.x, randRoomCoordinates.y), Quaternion.identity);
			rooms.Add(newRoomGO);
		}

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
	#endregion

	private void OnDrawGizmosSelected()
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

	public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
	public bool Occupied { get => occupied; set => occupied = value; }
}
