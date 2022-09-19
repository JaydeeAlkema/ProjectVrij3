using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class LevelGeneratorV2 : MonoBehaviour
{
	[Header("Level Generation Settings")]
	[SerializeField] private int seed;
	[SerializeField] private int chunkSize = 20;
	[SerializeField] private Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[Space]
	[SerializeField] private List<GameObject> chunks = new List<GameObject>();
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

	private IEnumerator CreateChunks()
	{
		for (int x = 0; x < chunkGridSize.x; x++)
		{
			for (int y = 0; y < chunkGridSize.y; y++)
			{
				GameObject chunkGO = new GameObject($"Chunk [{x}][{y}]");
				chunkGO.transform.parent = levelAssetsParent;
				chunkGO.transform.localPosition = new Vector2(x * chunkSize, y * chunkSize);

				chunks.Add(chunkGO);
			}
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator CreatePath()
	{
		List<GameObject> path = new List<GameObject>();
		int randChunkStartingIndex = Random.Range(0, chunks.Count);
		GameObject startingChunk = chunks[randChunkStartingIndex];
		path.Add(startingChunk);

		while (path.Count == chunks.Count)
		{
			GameObject nextChunk = null;
			int dir = Random.Range(1, 5);

			switch (dir)
			{
				case 1:
					break;

				case 2:
					break;

				case 3:
					break;

				case 4:
					break;

				default:
					break;
			}

		}

		yield return new WaitForEndOfFrame();
	}


	private void OnDrawGizmosSelected()
	{
		if (chunks.Count > 0)
		{
			foreach (GameObject chunk in chunks)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(chunk.transform.position, new Vector3(chunkSize, chunkSize));
			}
		}
	}
}
