using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[Header("Generation Configuration")]
	[SerializeField] private int seed = 0;
	[SerializeField, MinMaxSlider(5, 20)] private Vector2Int roomCount = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 20)] private Vector2Int roomSize = new Vector2Int();
	[SerializeField, MinMaxSlider(5, 15)] private Vector2Int pathwayLength = new Vector2Int();
	[SerializeField, MinMaxSlider(1, 5)] private Vector2Int pathwaySize = new Vector2Int();

	private List<Vector2> walkerPathCoordinates = new List<Vector2>();

	private void Start()
	{
		seed = seed == 0 ? Random.Range(0, int.MaxValue) : seed;

		Random.InitState(seed);

		InitLevelGeneration();
	}

	private void InitLevelGeneration()
	{

	}

	/// <summary>
	/// This functions generates the layout of the dungeon. This is done with the help of a "walker"
	/// This walker will "walk" a path, everytime this walker reaches it target distance, it takes a turn and marks the tile as a room origin.
	/// After the walker is done walking it's path, every room origin tiles will get saved and used later on in the generator to generate rooms upon.
	/// </summary>
	private void GenerateLevelLayout()
	{
		GameObject walkerGO = new GameObject("Walker");

		int _roomCount = Random.Range(roomCount.x, roomCount.y);
		for (int r = 0; r < _roomCount; r++)
		{
			int _pathLength = Random.Range(pathwayLength.x, pathwayLength.y);
		}
	}
}