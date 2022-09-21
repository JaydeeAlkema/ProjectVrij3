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
	[SerializeField] private List<Chunk> path = new List<Chunk>();
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
		Chunk currentChunk = chunks[(chunks.Count / 2) - 1];
		currentChunk.Occupied = true;

		int triesFailsafe = 1000;
		int tries = 0;

		while (path.Count != chunks.Count && tries < triesFailsafe)
		{
			tries++;

			List<Chunk> neighbours = GetNeighbouringChunks(currentChunk.Coordinates);

			if (neighbours.Count > 0)
			{
				int randNeighbour = Random.Range(0, neighbours.Count);
				Chunk nextChunk = neighbours[randNeighbour];

				if (nextChunk.gameObject != null && nextChunk.Coordinates != Vector2Int.zero && nextChunk.Occupied == false)
				{
					nextChunk.Occupied = true;
					path.Add(nextChunk);
					currentChunk = nextChunk;
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
