using NaughtyAttributes;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LevelGeneratorV3 : MonoBehaviour
{
	[SerializeField, BoxGroup("Main Settings")] private int seed = 0;
	[SerializeField, BoxGroup("Main Settings")] private bool generateRandomSeed = false;
	[SerializeField, BoxGroup("Main Settings")] private int mapPieceLimit = 100;
	[SerializeField, BoxGroup("Main Settings")] private int mapPiecePlacementTryLimit = 2;

	[Space(10)]

	[SerializeField, BoxGroup("Map Piece Settings")] private int mapPieceOffset = 21;
	[SerializeField, BoxGroup("Map Piece Settings")] private Transform connectedMapPiecesParent = default;
	[SerializeField, BoxGroup("Map Piece Settings")] private Transform disconnectedMapPiecesParent = default;
	[SerializeField, BoxGroup("Map Piece Settings")] private Transform interactablesParent = default;
	[Space]
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> spawnMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> mapPieces = new WeightedRandomList<GameObject>();
	[Space]
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> northDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> eastDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> southDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> westDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[Space]
	[SerializeField, BoxGroup("Map Piece Settings")] private GameObject bossPortalPrefab = default;
	[SerializeField, BoxGroup("Map Piece Settings")] private GameObject levelStatuePrefab = default;

	[Space(10)]

	[SerializeField, BoxGroup("Enemy Settings")] private AstarPath astarData = null;
	[SerializeField, BoxGroup("Enemy Settings")] private Transform enemyParentTransform = null;
	[Space]
	[SerializeField, BoxGroup("Enemy Settings")] private int playerSafeZoneRadii = 2;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> enemyPrefabs = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Enemy Settings")] private int enemyCountPerMapPiece = 5;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> rewardEnemyPrefabs = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Enemy Settings")] private int rewardEnemyCountPerLevel = 3;

	[Space(10)]

	[SerializeField, BoxGroup("Debug Variables")] private float debugTime = 0.5f;
	[Space]
	[SerializeField, BoxGroup("Debug Variables")] private bool debugOverlaps = false;
	[SerializeField, BoxGroup("Debug Variables")] private bool debugConnections = false;
	[SerializeField, BoxGroup("Debug Variables")] private bool debugMapPiecesInSceneDictionary = false;
	[SerializeField, BoxGroup("Debug Variables")] private bool debugMapPiecesOutsidePlayerSafeZone = false;
	[Space]
	[SerializeField, BoxGroup("Debug Variables")] private int goodGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int okGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int passibleGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int badGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int TerribleGenerationTime;

	private Dictionary<GameObject, Vector2> mapPiecesInScene = new Dictionary<GameObject, Vector2>();
	private Dictionary<ConnectionPoint, Vector2> connectionPointsInScene = new Dictionary<ConnectionPoint, Vector2>();
	private Vector2 overlapSize = new Vector2(40, 40);

	public Dictionary<GameObject, Vector2> MapPiecesInScene { get => mapPiecesInScene; private set => mapPiecesInScene = value; }
	public Vector2 OverlapSize { get => overlapSize; private set => overlapSize = value; }

	private void Start()
	{
		if (GameManager.Instance) GameManager.Instance.FetchDungeonReferences();
	}

	[Button]
	public void GenerateInScene()
	{
		StartCoroutine(Generate());

		if (mapPiecesInScene.Count < mapPieceLimit)
		{
			ClearLog();
			CleanUp();
			Debug.Log($"<color=red> Bad seed, {seed}</color>");
		}
	}
	public IEnumerator Generate()
	{
		CleanUp();

		if (generateRandomSeed) seed = Random.Range(0, int.MaxValue);
		Random.InitState(seed);

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		GameObject spawnMapPieceGO = Instantiate(spawnMapPieces.GetRandom());
		spawnMapPieceGO.name += $" [Spawn]";
		spawnMapPieceGO.transform.position = Vector2.zero;
		spawnMapPieceGO.transform.parent = connectedMapPiecesParent;
		mapPiecesInScene.Add(spawnMapPieceGO, Vector2.zero);

		// Generate the level. This is a dirty generation, meaning it has lots of dead-ends, incorrect connections, etc. We clean those up later.
		List<ConnectionPoint> levelConnectionPointsInScene = new List<ConnectionPoint>();
		for (int i = 0; i < mapPieceLimit; i++)
		{
			// Instantiate a new Map Piece. if this is the first Map Piece, then it must be the spawn Map Piece.
			GameObject newMapPieceGO = Instantiate(mapPieces.GetRandom());

			newMapPieceGO.transform.parent = disconnectedMapPiecesParent;

			int retryLimit = mapPiecePlacementTryLimit;
			while (newMapPieceGO != null && retryLimit > 0)
			{
				// Get all connection points from the new map piece.
				List<ConnectionPoint> NewMapPieceConnectionPoints = GetConnectionPoints(newMapPieceGO);
				ConnectionPoint newMapPieceConnectionPointNorth = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.North);
				ConnectionPoint newMapPieceConnectionPointEast = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.East);
				ConnectionPoint newMapPieceConnectionPointSouth = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.South);
				ConnectionPoint newMapPieceConnectionPointWest = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.West);

				// Loop through all the map pieces in the scene. The first map piece that is found that has an unoccupied connection point will be set as reference.
				// If the set reference to the existing map piece does not connect with the new map piece, we continue the loop until we find two map pieces that fit together.
				Vector2 newMapPiecePos = new Vector2();
				MapPiece newMapPiece = newMapPieceGO.GetComponent<MapPiece>();
				bool connected = false;

				foreach (KeyValuePair<GameObject, Vector2> keyValuePair in mapPiecesInScene)
				{
					GameObject mapPieceInScene = keyValuePair.Key;
					Vector2 mapPiecePos = keyValuePair.Value;
					List<ConnectionPoint> mapPieceInSceneConnectionPoints = GetConnectionPoints(mapPieceInScene);

					foreach (ConnectionPoint mapPieceInSceneConnectionPoint in mapPieceInSceneConnectionPoints)
					{
						ConnectionPointStatus mapPieceInSceneConnectionPointStatus = mapPieceInSceneConnectionPoint.Status;

						if (mapPieceInSceneConnectionPointStatus == ConnectionPointStatus.Connected || connected) continue;

						switch (mapPieceInSceneConnectionPoint.Direction)
						{
							case ConnectionPointDirection.North:
								newMapPiecePos = new Vector2(mapPiecePos.x, mapPiecePos.y + mapPieceOffset);
								ConnectMapPieces(newMapPieceGO, mapPieceInScene, newMapPieceConnectionPointSouth, mapPieceInSceneConnectionPoint, newMapPiecePos, ref connected);
								break;
							case ConnectionPointDirection.East:
								newMapPiecePos = new Vector2(mapPiecePos.x + mapPieceOffset, mapPiecePos.y);
								ConnectMapPieces(newMapPieceGO, mapPieceInScene, newMapPieceConnectionPointWest, mapPieceInSceneConnectionPoint, newMapPiecePos, ref connected);
								break;
							case ConnectionPointDirection.South:
								newMapPiecePos = new Vector2(mapPiecePos.x, mapPiecePos.y - mapPieceOffset);
								ConnectMapPieces(newMapPieceGO, mapPieceInScene, newMapPieceConnectionPointNorth, mapPieceInSceneConnectionPoint, newMapPiecePos, ref connected);
								break;
							case ConnectionPointDirection.West:
								newMapPiecePos = new Vector2(mapPiecePos.x - mapPieceOffset, mapPiecePos.y);
								ConnectMapPieces(newMapPieceGO, mapPieceInScene, newMapPieceConnectionPointEast, mapPieceInSceneConnectionPoint, newMapPiecePos, ref connected);
								break;
						}
					}
				}

				newMapPieceGO.transform.position = newMapPiecePos;
				if (connected && !IsMapPieceBlockedFromConnecting(newMapPieceGO))
				{
					newMapPieceGO.name += $" [{mapPiecesInScene.Count}]";
					newMapPieceGO.transform.parent = connectedMapPiecesParent;
					mapPiecesInScene.Add(newMapPieceGO, newMapPiecePos);
					newMapPieceGO = null;
					// Temp
					foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
					{
						MapPiece mapPiece = mapPieceInScene.Key.GetComponent<MapPiece>();
						SetMapPieceNeighbours(mapPiece);
						foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
						{
							if (connectionPoint.ConnectedTo == null && connectionPoint.Status == ConnectionPointStatus.Connected)
							{
								connectionPoint.Status = ConnectionPointStatus.Disconnected;
							}
						}
					}
				}
				else
				{
					if (debugConnections) Debug.Log($"<color=red>Failed to connect {newMapPieceGO.name}</color>", newMapPieceGO);

					// Destroy the new map piece if it was not connected to any other map pieces
					DestroyImmediate(newMapPieceGO);

					// Generate a new map piece and set its parent to the disconnected map pieces parent
					newMapPieceGO = Instantiate(mapPieces.GetRandom());
					newMapPieceGO.transform.parent = disconnectedMapPiecesParent;
					retryLimit--;
				}
			}
		}

		// Temp
		foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
		{
			MapPiece mapPiece = mapPieceInScene.Key.GetComponent<MapPiece>();
			SetMapPieceNeighbours(mapPiece);
			foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
			{
				if (connectionPoint.ConnectedTo == null && connectionPoint.Status == ConnectionPointStatus.Connected)
				{
					connectionPoint.Status = ConnectionPointStatus.Disconnected;
				}
			}
		}

		RemoveDisconnectedMapPieces();
		AddDeadEnds();
		SpawnEnemies();
		SpawnLevelStatue();
		DecorateLevel();

		// Set GridGraph position and size.
		Bounds mapBounds = GetMaxBounds(connectedMapPiecesParent.gameObject);
		foreach (GridGraph gridGraph in astarData.graphs.Cast<GridGraph>())
		{
			gridGraph.center = mapBounds.center;
			gridGraph.SetDimensions((int)mapBounds.size.x, (int)mapBounds.size.y, gridGraph.nodeSize);
			AstarPath.active.Scan(gridGraph);
		}

		// Spawn the boss portal in the furthest map piece from the spawn.
		float dst = 0;
		Vector2 furthestPositionFromSpawn = new Vector2();
		foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
		{
			if (mapPieceInScene.Key.name.ToLower().Contains("dead end") == false) continue;
			float distanceToMapPiece = Vector2.Distance(Vector2.zero, mapPieceInScene.Value);
			if (distanceToMapPiece > dst)
			{
				dst = distanceToMapPiece;
				furthestPositionFromSpawn = mapPieceInScene.Value;
			}
		}

		GameObject bossPortalGO = Instantiate(bossPortalPrefab, furthestPositionFromSpawn, Quaternion.identity, interactablesParent);
		Portal portal = bossPortalGO.GetComponent<Portal>();
		portal.CurrentSceneName = "Jaydee Testing Scene";
		portal.SceneToLoadName = "Boss Testing";

		if (debugMapPiecesInSceneDictionary)
		{
			foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
			{
				Debug.Log($"{mapPiece.Key.name} - {mapPiece.Value}", mapPiece.Key);
			}
		}

		stopwatch.Stop();
		long generationTime = stopwatch.ElapsedMilliseconds;
		string colorString = "";

		if (generationTime < goodGenerationTime) colorString = "lime";
		else if (generationTime > goodGenerationTime && generationTime < passibleGenerationTime) colorString = "yellow";
		else if (generationTime > passibleGenerationTime && generationTime < okGenerationTime) colorString = "orange";
		else if (generationTime > okGenerationTime && generationTime < badGenerationTime) colorString = "brown";
		else if (generationTime > TerribleGenerationTime) colorString = "red";

		Debug.Log($"<color={colorString}>Level Generation took: {generationTime}ms</color>");
		if (debugConnections || debugOverlaps || debugMapPiecesInSceneDictionary || debugMapPiecesOutsidePlayerSafeZone) Debug.Log($"<color=cyan>Enabling Debugging Logs adds extra time to the level generation within the Editor. Disable any extra debug logging within the Level Generator to get the actual level generation times</color>");

		yield return null;
	}
	private void RemoveDisconnectedMapPieces()
	{
		foreach (Transform disconnectedMapPieceTransform in disconnectedMapPiecesParent.GetComponentsInChildren<Transform>())
		{
			if (disconnectedMapPieceTransform != disconnectedMapPiecesParent && disconnectedMapPieceTransform != null)
			{
				GameObject disconnectedMapPieceGO = disconnectedMapPieceTransform.gameObject;
				DestroyImmediate(disconnectedMapPieceGO);
			}
		}
	}
	private void AddDeadEnds()
	{
		// Get all the map pieces with Unoccupied connection points and store them in a list for later use.
		List<MapPiece> inCompleteMapPieces = new List<MapPiece>();
		foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
		{
			MapPiece mapPiece = mapPieceInScene.Key.GetComponent<MapPiece>();
			foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
			{
				if (connectionPoint.Status == ConnectionPointStatus.Disconnected)
				{
					inCompleteMapPieces.Add(mapPiece);
				}
			}
		}

		// Iterate over the list of map pieces with unoccupied connection points
		foreach (MapPiece mapPiece in inCompleteMapPieces)
		{
			// Set the neighbours for the current map piece
			SetMapPieceNeighbours(mapPiece);

			// Get the position of the current map piece
			Vector2 mapPiecePos = mapPiece.transform.position;

			// Declare variables to hold references to the connection points and neighbours in each direction
			ConnectionPoint northConnectionPointReference = null;
			ConnectionPoint eastConnectionPointReference = null;
			ConnectionPoint southConnectionPointReference = null;
			ConnectionPoint westConnectionPointReference = null;

			GameObject northNeighbour = null;
			GameObject eastNeighbour = null;
			GameObject southNeighbour = null;
			GameObject westNeighbour = null;

			// Declare variables to hold the positions of the neighbours in each direction
			Vector2 northNeighbourPosition = new Vector2(0, mapPieceOffset);
			Vector2 eastNeighbourPosition = new Vector2(mapPieceOffset, 0);
			Vector2 southNeighbourPosition = new Vector2(0, -mapPieceOffset);
			Vector2 westNeighbourPosition = new Vector2(-mapPieceOffset, 0);

			// Set connection point references
			foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
			{
				switch (connectionPoint.Direction)
				{
					case ConnectionPointDirection.North:
						northConnectionPointReference = connectionPoint;
						break;
					case ConnectionPointDirection.East:
						eastConnectionPointReference = connectionPoint;
						break;
					case ConnectionPointDirection.South:
						southConnectionPointReference = connectionPoint;
						break;
					case ConnectionPointDirection.West:
						westConnectionPointReference = connectionPoint;
						break;
					default:
						break;
				}
			}

			// Set neighbour references
			foreach (GameObject neighbour in mapPiece.Neighbours)
			{
				Vector2 neighbourPos = neighbour.transform.position;

				if (mapPiecePos + northNeighbourPosition == neighbourPos)
				{
					northNeighbour = neighbour;
				}
				else if (mapPiecePos + eastNeighbourPosition == neighbourPos)
				{
					eastNeighbour = neighbour;
				}
				else if (mapPiecePos + southNeighbourPosition == neighbourPos)
				{
					southNeighbour = neighbour;
				}
				else if (mapPiecePos + westNeighbourPosition == neighbourPos)
				{
					westNeighbour = neighbour;
				}
			}

			// Check if the north connection point is unoccupied and if there is no neighbour in the north direction
			if (northConnectionPointReference != null && northConnectionPointReference.Status == ConnectionPointStatus.Disconnected && northNeighbour == null)
			{
				// Create a new dead end map piece in the north direction and add it to the scene
				GameObject deadEndMapPiece = Instantiate(northDeadEndMapPieces.GetRandom(), mapPiecePos + northNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				if (Overlap(deadEndMapPiece, deadEndMapPiece.transform.position, overlapSize))
				{
					DestroyImmediate(deadEndMapPiece);
				}
				else
				{
					mapPiecesInScene.Add(deadEndMapPiece, mapPiecePos + northNeighbourPosition);

					// Get the MapPiece component of the new dead end map piece
					MapPiece deadEndMapPieceMapPiece = deadEndMapPiece.GetComponent<MapPiece>();

					// Set the connection point status of the south connection point of the new dead end map piece to Occupied
					deadEndMapPieceMapPiece.ConnectionPoints[0].Status = ConnectionPointStatus.Connected;

					// Set the connection point status of the north connection point of the current map piece to Occupied
					northConnectionPointReference.Status = ConnectionPointStatus.Connected;

					// Add the new dead end map piece to the list of neighbours of the current map piece
					mapPiece.Neighbours.Add(deadEndMapPiece);
				}
			}
			else if (northConnectionPointReference != null && northConnectionPointReference.Status == ConnectionPointStatus.Disconnected && northNeighbour != null && northConnectionPointReference.DeadEnd != null)
			{
				northConnectionPointReference.DeadEnd.Enable();
				northConnectionPointReference.Status = ConnectionPointStatus.Blocked;
			}

			// Check if the east connection point is unoccupied and if there is no neighbour in the east direction
			if (eastConnectionPointReference != null && eastConnectionPointReference.Status == ConnectionPointStatus.Disconnected && eastNeighbour == null)
			{
				// Create a new dead end map piece in the east direction and add it to the scene
				GameObject deadEndMapPiece = Instantiate(eastDeadEndMapPieces.GetRandom(), mapPiecePos + eastNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				if (Overlap(deadEndMapPiece, deadEndMapPiece.transform.position, overlapSize))
				{
					DestroyImmediate(deadEndMapPiece);
				}
				else
				{
					mapPiecesInScene.Add(deadEndMapPiece, mapPiecePos + eastNeighbourPosition);

					// Get the MapPiece component of the new dead end map piece
					MapPiece deadEndMapPieceMapPiece = deadEndMapPiece.GetComponent<MapPiece>();

					// Set the connection point status of the west connection point of the new dead end map piece to Occupied
					deadEndMapPieceMapPiece.ConnectionPoints[0].Status = ConnectionPointStatus.Connected;

					// Set the connection point status of the east connection point of the current map piece to Occupied
					eastConnectionPointReference.Status = ConnectionPointStatus.Connected;

					// Add the new dead end map piece to the list of neighbours of the current map piece
					mapPiece.Neighbours.Add(deadEndMapPiece);
				}
			}
			else if (eastConnectionPointReference != null && eastConnectionPointReference.Status == ConnectionPointStatus.Disconnected && eastNeighbour != null && eastConnectionPointReference.DeadEnd != null)
			{
				eastConnectionPointReference.DeadEnd.Enable();
				eastConnectionPointReference.Status = ConnectionPointStatus.Blocked;
			}

			// Check if the south connection point is unoccupied and if there is no neighbour in the south direction
			if (southConnectionPointReference != null && southConnectionPointReference.Status == ConnectionPointStatus.Disconnected && southNeighbour == null)
			{
				// Create a new dead end map piece in the south direction and add it to the scene
				GameObject deadEndMapPiece = Instantiate(southDeadEndMapPieces.GetRandom(), mapPiecePos + southNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				if (Overlap(deadEndMapPiece, deadEndMapPiece.transform.position, overlapSize))
				{
					DestroyImmediate(deadEndMapPiece);
				}
				else
				{
					mapPiecesInScene.Add(deadEndMapPiece, mapPiecePos + southNeighbourPosition);

					// Get the MapPiece component of the new dead end map piece
					MapPiece deadEndMapPieceMapPiece = deadEndMapPiece.GetComponent<MapPiece>();

					// Set the connection point status of the north connection point of the new dead end map piece to Occupied
					deadEndMapPieceMapPiece.ConnectionPoints[0].Status = ConnectionPointStatus.Connected;

					// Set the connection point status of the south connection point of the current map piece to Occupied
					southConnectionPointReference.Status = ConnectionPointStatus.Connected;

					// Add the new dead end map piece to the list of neighbours of the current map piece
					mapPiece.Neighbours.Add(deadEndMapPiece);
				}
			}
			else if (southConnectionPointReference != null && southConnectionPointReference.Status == ConnectionPointStatus.Disconnected && southNeighbour != null && southConnectionPointReference.DeadEnd != null)
			{
				southConnectionPointReference.DeadEnd.Enable();
				southConnectionPointReference.Status = ConnectionPointStatus.Blocked;
			}

			// Check if the west connection point is unoccupied and if there is no neighbour in the west direction
			if (westConnectionPointReference != null && westConnectionPointReference.Status == ConnectionPointStatus.Disconnected && westNeighbour == null)
			{
				// Create a new dead end map piece in the west direction and add it to the scene
				GameObject deadEndMapPiece = Instantiate(westDeadEndMapPieces.GetRandom(), mapPiecePos + westNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				if (Overlap(deadEndMapPiece, deadEndMapPiece.transform.position, overlapSize))
				{
					DestroyImmediate(deadEndMapPiece);
				}
				else
				{
					mapPiecesInScene.Add(deadEndMapPiece, mapPiecePos + westNeighbourPosition);

					// Get the MapPiece component of the new dead end map piece
					MapPiece deadEndMapPieceMapPiece = deadEndMapPiece.GetComponent<MapPiece>();

					// Set the connection point status of the east connection point of the new dead end map piece to Occupied
					deadEndMapPieceMapPiece.ConnectionPoints[0].Status = ConnectionPointStatus.Connected;

					// Set the connection point status of the west connection point of the current map piece to Occupied
					westConnectionPointReference.Status = ConnectionPointStatus.Connected;

					// Add the new dead end map piece to the list of neighbours of the current map piece
					mapPiece.Neighbours.Add(deadEndMapPiece);
				}
			}
			else if (westConnectionPointReference != null && westConnectionPointReference.Status == ConnectionPointStatus.Disconnected && westNeighbour != null && westConnectionPointReference.DeadEnd != null)
			{
				westConnectionPointReference.DeadEnd.Enable();
				westConnectionPointReference.Status = ConnectionPointStatus.Blocked;
			}
		}
	}
	private void SpawnEnemies()
	{
		// Spawn Regular Enemies
		foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
		{
			// Check if the current map piece is the first map piece (player spawn map piece). If so, we do not spawn any enemies.
			GameObject mapPieceGO = mapPiece.Key;
			if (Vector2.Distance(mapPieceGO.transform.position, Vector2.zero) < mapPieceOffset) continue;

			Transform mapPieceFloorParentTransform = mapPieceGO.transform.Find("Tiles").Find("Floor");
			Transform[] floorTiles = mapPieceFloorParentTransform.GetComponentsInChildren<Transform>();

			List<int> regularEnemySpawnIndeces = new List<int>();
			for (int enemyCount = 0; enemyCount < enemyCountPerMapPiece; enemyCount++)
			{
				int spawnIndex = Random.Range(1, floorTiles.Length);
				while (regularEnemySpawnIndeces.Contains(spawnIndex))
				{
					spawnIndex = Random.Range(1, floorTiles.Length);
				}
				regularEnemySpawnIndeces.Add(spawnIndex);
			}

			foreach (int spawnIndex in regularEnemySpawnIndeces)
			{
				Transform spawnTile = floorTiles[spawnIndex];
				Instantiate(enemyPrefabs.GetRandom(), new Vector2(spawnTile.position.x, spawnTile.position.y), Quaternion.identity, enemyParentTransform);
			}
		}

		// Spawn Reward Enemies
		List<int> rewardEnemyMapPieceSpawnIndeces = new List<int>();
		for (int rewardEnemyCount = 0; rewardEnemyCount < rewardEnemyCountPerLevel; rewardEnemyCount++)
		{
			int spawnIndex = Random.Range(1, mapPiecesInScene.Count);
			while (rewardEnemyMapPieceSpawnIndeces.Contains(spawnIndex))
			{
				spawnIndex = Random.Range(1, mapPiecesInScene.Count);
			}
			rewardEnemyMapPieceSpawnIndeces.Add(spawnIndex);
		}

		int i = 0;
		foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
		{
			if (rewardEnemyMapPieceSpawnIndeces.Contains(i++))
			{
				GameObject mapPieceGO = mapPiece.Key;
				Transform mapPieceFloorParentTransform = mapPieceGO.transform.Find("Tiles").Find("Floor");
				Transform[] floorTiles = mapPieceFloorParentTransform.GetComponentsInChildren<Transform>();
				Transform spawnTile = floorTiles[Random.Range(0, floorTiles.Length)];
				Instantiate(rewardEnemyPrefabs.GetRandom(), new Vector2(spawnTile.position.x, spawnTile.position.y), Quaternion.identity, enemyParentTransform);
			}
		}
	}
	private void SpawnLevelStatue()
	{
		MapPiece mapPiece = null;
		List<GameObject> levelStatueSpawnPointsInMapPiece = null;
		GameObject levelStatueSpawnPoint = null;

		// Try to find a valid map piece and spawn point
		// that is not a "dead end"
		while (mapPiece == null || levelStatueSpawnPointsInMapPiece == null || levelStatueSpawnPointsInMapPiece.Count == 0)
		{
			KeyValuePair<GameObject, Vector2> mapPieceKeyValuePair = mapPiecesInScene.ElementAt(Random.Range(0, mapPiecesInScene.Count));
			if (mapPieceKeyValuePair.Key.name.ToLower().Contains("dead end") == false)
			{
				mapPiece = mapPieceKeyValuePair.Key.GetComponent<MapPiece>();
				levelStatueSpawnPointsInMapPiece = mapPiece.StatueSpawnPoints;
				levelStatueSpawnPoint = levelStatueSpawnPointsInMapPiece[Random.Range(0, levelStatueSpawnPointsInMapPiece.Count)];
			}
		}

		// If a valid map piece and spawn point was found,
		// instantiate the level statue at the spawn point
		if (mapPiece != null && levelStatueSpawnPoint != null)
		{
			Instantiate(levelStatuePrefab, levelStatueSpawnPoint.transform.position, Quaternion.identity, interactablesParent);
		}
	}
	private void DecorateLevel()
	{
		foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
		{
			MapPiece mapPiece = mapPieceInScene.Key.GetComponent<MapPiece>();
			if (mapPiece != null) mapPiece.Decorate();
		}
	}

	#region Helper Functions
	[Button]
	private void CleanUp()
	{
		ClearLog();

		// Remove map Pieces
		List<Transform> connectedMapPieces = connectedMapPiecesParent.GetComponentsInChildren<Transform>().ToList();
		for (int m = connectedMapPieces.Count - 1; m >= 0; m--)
		{
			Transform mapPieceTransform = connectedMapPieces[m];
			if (mapPieceTransform != connectedMapPiecesParent)
			{
				DestroyImmediate(mapPieceTransform.gameObject);
			}
		}

		mapPiecesInScene.Clear();
		connectionPointsInScene.Clear();
		RemoveDisconnectedMapPieces();

		// Remove enemies
		List<Transform> enemiesInScene = enemyParentTransform.GetComponentsInChildren<Transform>().ToList();
		for (int e = enemiesInScene.Count - 1; e >= 0; e--)
		{
			Transform enemyTransform = enemiesInScene[e];
			if (enemyTransform != enemyParentTransform)
			{
				DestroyImmediate(enemyTransform.gameObject);
			}
		}

		// Remove Interactables
		List<Transform> interactablesInScene = interactablesParent.GetComponentsInChildren<Transform>().ToList();
		for (int i = interactablesInScene.Count - 1; i >= 0; i--)
		{
			Transform interactableTransform = interactablesInScene[i];
			if (interactableTransform != interactablesParent)
			{
				DestroyImmediate(interactableTransform.gameObject);
			}
		}
	}
	public void ClearLog()
	{
#if UNITY_EDITOR
		var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
#endif
	}
	private bool IsMapPieceBlockedFromConnecting(GameObject newMapPieceGO)
	{
		MapPiece mapPiece = newMapPieceGO.GetComponent<MapPiece>();
		Vector2 newMapPiecePosition = new Vector2(newMapPieceGO.transform.position.x, newMapPieceGO.transform.position.y);

		Bounds neighbourBounds = new Bounds { size = overlapSize };
		foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
		{
			if (connectionPoint.Status == ConnectionPointStatus.Connected) continue;

			switch (connectionPoint.Direction)
			{
				case ConnectionPointDirection.North:
					neighbourBounds.center = new Vector3(newMapPiecePosition.x, newMapPiecePosition.y + mapPieceOffset);
					break;
				case ConnectionPointDirection.East:
					neighbourBounds.center = new Vector3(newMapPiecePosition.x + mapPieceOffset, newMapPiecePosition.y);
					break;
				case ConnectionPointDirection.South:
					neighbourBounds.center = new Vector3(newMapPiecePosition.x, newMapPiecePosition.y - mapPieceOffset);
					break;
				case ConnectionPointDirection.West:
					neighbourBounds.center = new Vector3(newMapPiecePosition.x - mapPieceOffset, newMapPiecePosition.y);
					break;
				default:
					break;
			}

			foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
			{
				Bounds mapPieceInSceneBounds = new Bounds()
				{
					center = mapPieceInScene.Value,
					size = overlapSize
				};

				if (mapPieceInSceneBounds.Intersects(neighbourBounds)) return true;
			}
		}

		return false;
	}
	private void SetMapPieceNeighbours(MapPiece mapPiece)
	{
		Vector2 mapPiecePos = new Vector2(mapPiece.transform.position.x, mapPiece.transform.position.y);

		Vector2 topNeighbourPos = new Vector2(mapPiecePos.x, mapPiecePos.y + mapPieceOffset);
		Vector2 rightNeighbourPos = new Vector2(mapPiecePos.x + mapPieceOffset, mapPiecePos.y);
		Vector2 bottomNeighbourPos = new Vector2(mapPiecePos.x, mapPiecePos.y - mapPieceOffset);
		Vector2 leftNeighbourPos = new Vector2(mapPiecePos.x - mapPieceOffset, mapPiecePos.y);

		GameObject topNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == topNeighbourPos).Key;
		GameObject rightNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == rightNeighbourPos).Key;
		GameObject bottomNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == bottomNeighbourPos).Key;
		GameObject leftNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == leftNeighbourPos).Key;

		mapPiece.AddNeighbour(topNeighbourGO);
		mapPiece.AddNeighbour(rightNeighbourGO);
		mapPiece.AddNeighbour(bottomNeighbourGO);
		mapPiece.AddNeighbour(leftNeighbourGO);
	}
	private void ConnectMapPieces(GameObject newMapPieceGO, GameObject mapPieceInScene, ConnectionPoint newMapPieceConnectionPoint, ConnectionPoint mapPieceInSceneConnectionPoint, Vector2 newMapPiecePos, ref bool connected)
	{
		if (newMapPieceConnectionPoint != null && newMapPieceConnectionPoint.Status == ConnectionPointStatus.Disconnected && mapPieceInScene.name.ToLower().Contains(newMapPieceGO.name.ToLower()) == false)
		{
			if (!Overlap(newMapPieceGO, newMapPiecePos, overlapSize))
			{
				if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);

				newMapPieceConnectionPoint.Status = ConnectionPointStatus.Connected;
				mapPieceInSceneConnectionPoint.Status = ConnectionPointStatus.Connected;
				newMapPieceConnectionPoint.ConnectedTo = mapPieceInSceneConnectionPoint;
				mapPieceInSceneConnectionPoint.ConnectedTo = newMapPieceConnectionPoint;
				connected = true;
			}
		}
	}
	private bool Overlap(GameObject origin, Vector2 center, Vector2 size)
	{
		Bounds bounds = new Bounds();
		bounds.center = center;
		bounds.size = size;

		foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
		{
			Bounds mapPieceBounds = new Bounds();
			mapPieceBounds.center = mapPiece.Value;
			mapPieceBounds.size = size;

			GameObject mapPieceGO = mapPiece.Key;
			if (bounds.Intersects(mapPieceBounds))
			{
				if (debugOverlaps) Debug.Log($"<color=orange>[{origin.name}] Overlap found with[{mapPieceGO.name}]</color>", mapPieceGO);
				return true;
			}
		}
		return false;
	}
	private List<ConnectionPoint> GetConnectionPoints(GameObject gameObject)
	{
		ConnectionPoint[] connectionPoints = gameObject.GetComponentsInChildren<ConnectionPoint>();
		if (connectionPoints.Length > 0)
		{
			return connectionPoints.ToList();
		}
		else
		{
			Debug.LogWarning("No connection points could be found!");
			return null;
		}
	}
	Bounds GetMaxBounds(GameObject gameObject)
	{
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		if (renderers.Length == 0) return new Bounds(gameObject.transform.position, Vector3.zero);
		Bounds Bounds = renderers[0].bounds;
		foreach (Renderer renderer in renderers)
		{
			Bounds.Encapsulate(renderer.bounds);
		}
		return Bounds;
	}
	#endregion
}
