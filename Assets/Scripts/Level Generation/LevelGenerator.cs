using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum levelPieceType
{
	empty,
	room,
	pathway,
	deadend,
	bossRoom,
	treassureRoom
}

public class LevelGenerator : MonoBehaviour
{
	private static LevelGenerator instance = null;

	[Header("Generation Configuration")]
	[SerializeField] private int seed = 0;
	[SerializeField, MinMaxSlider(5, 20)] private Vector2Int roomCount = new Vector2Int();
	[Space]

	[Header("Level Pieces")]
	[SerializeField] private List<LevelPiece> StartingLevelPieceCandidates = new List<LevelPiece>();
	[SerializeField] private List<GameObject> rooms = new List<GameObject>();
	[SerializeField] private List<GameObject> pathways = new List<GameObject>();
	[SerializeField] private List<GameObject> deadends = new List<GameObject>();
	[SerializeField] private List<GameObject> bossRooms = new List<GameObject>();
	[SerializeField] private List<GameObject> treassureRooms = new List<GameObject>();

	public static LevelGenerator Instance { get => instance; set => instance = value; }

	private void Awake()
	{
		if (instance == null || instance != this)
		{
			instance = this;
		}
	}

	private void Start()
	{
		if (seed == 0)
		{
			seed = Random.Range(0, int.MaxValue);
		}

		Random.InitState(seed);

		InitLevelGeneration();
	}

	[Button]
	private void InitLevelGeneration()
	{
		LevelPiece startingLevelPiece = StartingLevelPieceCandidates[Random.Range(0, StartingLevelPieceCandidates.Count)];
		GameObject startingRoomLevelPieceGO = Instantiate(startingLevelPiece.Prefab, Vector2.zero, Quaternion.identity);

		AddRoom(startingRoomLevelPieceGO);
	}

	#region Public Methods
	public void AddRoom(GameObject room)
	{
		rooms.Add(room);
	}

	public void AddPathway(GameObject pathway)
	{
		pathways.Add(pathway);
	}

	public void AddDeadend(GameObject deadend)
	{
		deadends.Add(deadend);
	}

	public void AddBossRoom(GameObject bossRoom)
	{
		bossRooms.Add(bossRoom);
	}

	public void AddTreassureRoom(GameObject treassureRoom)
	{
		treassureRooms.Add(treassureRoom);
	}
	#endregion
}