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
	[SerializeField] private Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[Space]
	[SerializeField] private List<Chunk> chunks = new List<Chunk>();
	[SerializeField] private List<int> path = new List<int>();
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

		executionTime.Stop();
		Debug.Log($"Level Generation took: {executionTime.ElapsedMilliseconds}ms");
	}

	/// <summary>
	/// Lays the chunks grid.
	/// </summary>
	/// <returns></returns>
	private IEnumerator CreateChunks()
	{
		for (int x = 0; x < chunkGridSize.x; x++)
		{
			for (int y = 0; y < chunkGridSize.y; y++)
			{
				Chunk chunk = new Chunk();
				chunk.SetGameObject(new GameObject($"Chunk [{x}][{y}]"));
				chunk.SetCoordinates(new Vector2Int(x * chunkSize, y * chunkSize));
				chunk.SetOccupied(false);

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
		int randChunkStartingIndex = Random.Range(0, chunks.Count);

		int currentChunkIndex = randChunkStartingIndex;
		int nextChunkIndex = -1;

		chunks[currentChunkIndex].SetOccupied(true);

		int triesFailsafe = 10000;
		int tries = 0;

		while (path.Count != chunks.Count && tries < triesFailsafe)
		{
			tries++;

			List<int> neighbours = GetNeighbouringChunks(chunks[currentChunkIndex].coordinates);

			if (neighbours.Count > 0)
			{
				int randNeighbour = Random.Range(0, neighbours.Count);
				nextChunkIndex = neighbours[randNeighbour];

				if (chunks[nextChunkIndex].gameObject != null && chunks[nextChunkIndex].coordinates != Vector2Int.zero && chunks[nextChunkIndex].occupied == false && path.Contains(nextChunkIndex) == false)
				{
					chunks[nextChunkIndex].SetOccupied(true);
					path.Add(nextChunkIndex);
					currentChunkIndex = nextChunkIndex;
				}
				else
				{
					Debug.Log($"{nextChunkIndex} already is present in the list of path chunks.");
					break;
				}
			}
			else
			{
				tries = triesFailsafe;
			}
		}

		yield return new WaitForEndOfFrame();
	}

	/// <summary>
	/// Get a random chunk.
	/// </summary>
	/// <param name="currentChunk"> Reference to the current chunk. </param>
	/// <param name="nextChunk"> Reference to the next chunk. </param>
	/// <returns></returns>
	private List<int> GetNeighbouringChunks(Vector2Int currentChunkCoordinates)
	{
		List<int> neighbourChunkIndeces = new List<int>();

		int chunkX = currentChunkCoordinates.x;
		int chunkY = currentChunkCoordinates.y;

		int topNeighbourIndex = GetChunkIndexByCoordinatesFromList(new Vector2Int(chunkX, chunkY + chunkSize));
		int rightNeighbourIndex = GetChunkIndexByCoordinatesFromList(new Vector2Int(chunkX + chunkSize, chunkY));
		int bottomNeighbourIndex = GetChunkIndexByCoordinatesFromList(new Vector2Int(chunkX, chunkY - chunkSize));
		int leftNeighbourIndex = GetChunkIndexByCoordinatesFromList(new Vector2Int(chunkX - chunkSize, chunkY));

		if (topNeighbourIndex != -1) neighbourChunkIndeces.Add(topNeighbourIndex);
		if (rightNeighbourIndex != -1) neighbourChunkIndeces.Add(rightNeighbourIndex);
		if (bottomNeighbourIndex != -1) neighbourChunkIndeces.Add(bottomNeighbourIndex);
		if (leftNeighbourIndex != -1) neighbourChunkIndeces.Add(leftNeighbourIndex);

		return neighbourChunkIndeces;
	}

	/// <summary>
	/// Get a chunk from coordinates.
	/// </summary>
	/// <param name="coordinates"> Coordinates to fetch the chunk by. </param>
	/// <returns></returns>
	private int GetChunkIndexByCoordinatesFromList(Vector2Int coordinates)
	{
		for (int i = 0; i < chunks.Count; i++)
		{
			if (chunks[i].coordinates == coordinates)
			{
				return i;
			}
		}
		return -1;
	}

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
				Gizmos.DrawLine(chunks[path[c]].gameObject.transform.position, chunks[path[c] + 1].gameObject.transform.position);
			}
		}
	}
}

[System.Serializable]
public struct Chunk
{
	public GameObject gameObject;
	public Vector2Int coordinates;
	public bool occupied;

	public void SetGameObject(GameObject gameObject)
	{
		this.gameObject = gameObject;
	}
	public void SetCoordinates(Vector2Int coordinates)
	{
		this.coordinates = coordinates;
	}
	public void SetOccupied(bool occupied)
	{
		this.occupied = occupied;
	}
}
