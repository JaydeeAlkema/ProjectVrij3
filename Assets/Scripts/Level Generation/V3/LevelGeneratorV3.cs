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

	[SerializeField, BoxGroup("Map Piece Settings")] private int mapPieceOffset = 21;
	[SerializeField, BoxGroup("Map Piece Settings")] private Transform connectedMapPiecesParent = default;
	[SerializeField, BoxGroup("Map Piece Settings")] private Transform disconnectedMapPiecesParent = default;
	[Space]
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> spawnMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> mapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> topDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> rightDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> bottomDeadEndMapPieces = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Map Piece Settings")] private WeightedRandomList<GameObject> leftDeadEndMapPieces = new WeightedRandomList<GameObject>();

	[SerializeField, BoxGroup("Enemy Settings")] private AstarPath astarData = null;
	[SerializeField, BoxGroup("Enemy Settings")] private Transform enemyParentTransform = null;
	[Space]
	[SerializeField, BoxGroup("Enemy Settings")] private int playerSafeZoneRadii = 2;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> enemyPrefabs = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Enemy Settings")] private int enemyCountPerMapPiece = 5;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> rewardEnemyPrefabs = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Enemy Settings")] private int rewardEnemyCountPerLevel = 3;

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
	private Vector2 overlapSize = new Vector2(20, 20);

	private void Start()
	{
		if (GameManager.Instance) GameManager.Instance.FetchDungeonReferences();
	}

	[Button]
	public void GenerateInScene()
	{
		StartCoroutine(Generate());
	}

	public IEnumerator Generate()
	{
		if (Application.isEditor)
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

				// Store all current connection points in the scene in a simple list for later use.
				levelConnectionPointsInScene.AddRange(NewMapPieceConnectionPoints);
				foreach (ConnectionPoint connectionPoint in levelConnectionPointsInScene)
				{
					if (connectionPoint != null)
					{
						Vector3 connectionPointPosition = connectionPoint.transform.position;
						connectionPointsInScene.TryAdd(connectionPoint, new Vector2(connectionPointPosition.x, connectionPointPosition.y));
					}
				}

				// Loop through all the map pieces in the scene. The first map piece that is found that has an unoccupied connection point will be set as reference.
				// If the set reference to the existing map piece does not connect with the new map piece, we continue the loop until we find two map pieces that fit together.
				Vector2 newMapPiecePos = new Vector2();
				bool connected = false;
				foreach (KeyValuePair<GameObject, Vector2> keyValuePair in mapPiecesInScene)
				{
					GameObject mapPieceInScene = keyValuePair.Key;
					List<ConnectionPoint> mapPieceInSceneConnectionPoints = GetConnectionPoints(mapPieceInScene);

					foreach (ConnectionPoint mapPieceInSceneConnectionPoint in mapPieceInSceneConnectionPoints)
					{
						if (mapPieceInSceneConnectionPoint.Occupied == false && !connected)
						{
							GameObject mapPieceInSceneGO = mapPieceInScene;
							Vector2 mapPiecePos = keyValuePair.Value;

							// Connect North to South
							if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.North && newMapPieceConnectionPointSouth != null && newMapPieceConnectionPointSouth.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPiecePos.x, mapPiecePos.y + mapPieceOffset);
								if (Overlap(newMapPieceGO, newMapPiecePos, overlapSize)) continue;
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);

								newMapPieceConnectionPointSouth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								newMapPieceConnectionPointSouth.ConnectedTo = mapPieceInSceneConnectionPoint;
								mapPieceInSceneConnectionPoint.ConnectedTo = newMapPieceConnectionPointSouth;
								connected = true;
							}
							// Connect East to West
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.East && newMapPieceConnectionPointWest != null && newMapPieceConnectionPointWest.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPiecePos.x + mapPieceOffset, mapPiecePos.y);
								if (Overlap(newMapPieceGO, newMapPiecePos, overlapSize)) continue;
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);

								newMapPieceConnectionPointWest.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								newMapPieceConnectionPointWest.ConnectedTo = mapPieceInSceneConnectionPoint;
								mapPieceInSceneConnectionPoint.ConnectedTo = newMapPieceConnectionPointWest;
								connected = true;
							}
							// Connect South to North
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.South && newMapPieceConnectionPointNorth != null && newMapPieceConnectionPointNorth.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPiecePos.x, mapPiecePos.y - mapPieceOffset);
								if (Overlap(newMapPieceGO, newMapPiecePos, overlapSize)) continue;
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);

								newMapPieceConnectionPointNorth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								newMapPieceConnectionPointNorth.ConnectedTo = mapPieceInSceneConnectionPoint;
								mapPieceInSceneConnectionPoint.ConnectedTo = newMapPieceConnectionPointNorth;
								connected = true;
							}
							// Connect West to East
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.West && newMapPieceConnectionPointEast != null && newMapPieceConnectionPointEast.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPiecePos.x - mapPieceOffset, mapPiecePos.y);
								if (Overlap(newMapPieceGO, newMapPiecePos, overlapSize)) continue;
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);

								newMapPieceConnectionPointEast.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								newMapPieceConnectionPointEast.ConnectedTo = mapPieceInSceneConnectionPoint;
								mapPieceInSceneConnectionPoint.ConnectedTo = newMapPieceConnectionPointEast;
								connected = true;
							}
						}
					}
				}

				if (!connected)
				{
					if (debugConnections) Debug.Log($"<color=red>Failed to connect {newMapPieceGO.name}</color>", newMapPieceGO);

					if (Application.isEditor)
						DestroyImmediate(newMapPieceGO);
					else
						Destroy(newMapPieceGO);

					newMapPieceGO = Instantiate(mapPieces.GetRandom());
					newMapPieceGO.transform.parent = disconnectedMapPiecesParent;
					retryLimit--;
				}
				else
				{
					newMapPieceGO.name += $" [{mapPiecesInScene.Count}]";
					newMapPieceGO.transform.position = newMapPiecePos;
					newMapPieceGO.transform.parent = connectedMapPiecesParent;
					mapPiecesInScene.Add(newMapPieceGO, newMapPiecePos);
					newMapPieceGO = null;
				}
			}
		}

		RemoveDisconnectedMapPieces();
		SetMapPieceNeighbours();
		AddDeadEnds();
		SpawnEnemies();

		Bounds mapBounds = GetMaxBounds(connectedMapPiecesParent.gameObject);
		foreach (GridGraph gridGraph in astarData.graphs)
		{
			gridGraph.center = mapBounds.center;
			gridGraph.SetDimensions((int)mapBounds.size.x, (int)mapBounds.size.y, gridGraph.nodeSize);
			AstarPath.active.Scan(gridGraph);
		}

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
	private void SetMapPieceNeighbours()
	{
		Vector2 topNeighbourPos = new Vector2(0, mapPieceOffset);
		Vector2 rightNeighbourPos = new Vector2(mapPieceOffset, 0);
		Vector2 bottomNeighbourPos = new Vector2(0, -mapPieceOffset);
		Vector2 leftNeighbourPos = new Vector2(-mapPieceOffset, 0);

		// Loop through all the map pieces in the scene and set their neighbours
		foreach (KeyValuePair<GameObject, Vector2> mapPieceInScene in mapPiecesInScene)
		{
			GameObject mapPieceGO = mapPieceInScene.Key;
			Vector2 mapPiecePos = mapPieceInScene.Value;

			if (!mapPieceGO.TryGetComponent<MapPiece>(out var mapPiece))
			{
				Debug.Log($"<color=red>Couldn't find MapPiece Component!", mapPieceGO);
				continue;
			}
			GameObject topNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == mapPiecePos + topNeighbourPos).Key;
			GameObject rightNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == mapPiecePos + rightNeighbourPos).Key;
			GameObject bottomNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == mapPiecePos + bottomNeighbourPos).Key;
			GameObject leftNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == mapPiecePos + leftNeighbourPos).Key;

			mapPiece.AddNeighbour(topNeighbourGO);
			mapPiece.AddNeighbour(rightNeighbourGO);
			mapPiece.AddNeighbour(bottomNeighbourGO);
			mapPiece.AddNeighbour(leftNeighbourGO);
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
				if (!connectionPoint.Occupied)
				{
					inCompleteMapPieces.Add(mapPiece);
					continue;
				}
			}
		}

		foreach (MapPiece mapPiece in inCompleteMapPieces)
		{
			Vector2 mapPiecePos = mapPiece.transform.position;

			ConnectionPoint northConnectionPoint = null;
			ConnectionPoint eastConnectionPoint = null;
			ConnectionPoint southConnectionPoint = null;
			ConnectionPoint westConnectionPoint = null;

			GameObject northNeighbour = null;
			GameObject eastNeighbour = null;
			GameObject southNeighbour = null;
			GameObject westNeighbour = null;

			Vector2 northNeighbourPosition = new Vector2(0, mapPieceOffset);
			Vector2 eastNeighbourPosition = new Vector2(mapPieceOffset, 0);
			Vector2 southNeighbourPosition = new Vector2(0, -mapPieceOffset);
			Vector2 westNeighbourPosition = new Vector2(-mapPieceOffset, 0);

			// Set connection point references.
			foreach (ConnectionPoint connectionPoint in mapPiece.ConnectionPoints)
			{
				switch (connectionPoint.Direction)
				{
					case ConnectionPointDirection.North:
						northConnectionPoint = connectionPoint;
						break;
					case ConnectionPointDirection.East:
						eastConnectionPoint = connectionPoint;
						break;
					case ConnectionPointDirection.South:
						southConnectionPoint = connectionPoint;
						break;
					case ConnectionPointDirection.West:
						westConnectionPoint = connectionPoint;
						break;
					default:
						break;
				}
			}

			// Set neighbour references.
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

			if (northConnectionPoint != null && northConnectionPoint.Occupied == false && northNeighbour == null)
			{
				GameObject northNeigbourGO = Instantiate(topDeadEndMapPieces.GetRandom(), mapPiecePos + northNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				MapPiece northNeighbourMapPiece = northNeigbourGO.GetComponent<MapPiece>();
				northNeighbourMapPiece.ConnectionPoints[0].Occupied = true;
				northNeighbourMapPiece.ConnectionPoints[0].ConnectedTo = northConnectionPoint;
				northConnectionPoint.Occupied = true;
				northConnectionPoint.ConnectedTo = northNeighbourMapPiece.ConnectionPoints[0];
				mapPiecesInScene.Add(northNeigbourGO, mapPiecePos + northNeighbourPosition);
			}
			else if (eastConnectionPoint != null && eastConnectionPoint.Occupied == false && eastNeighbour == null)
			{
				GameObject eastNeigbourGO = Instantiate(rightDeadEndMapPieces.GetRandom(), mapPiecePos + eastNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				MapPiece eastNeighbourMapPiece = eastNeigbourGO.GetComponent<MapPiece>();
				eastNeighbourMapPiece.ConnectionPoints[0].Occupied = true;
				eastNeighbourMapPiece.ConnectionPoints[0].ConnectedTo = eastConnectionPoint;
				eastConnectionPoint.Occupied = true;
				eastConnectionPoint.ConnectedTo = eastNeighbourMapPiece.ConnectionPoints[0];
				mapPiecesInScene.Add(eastNeigbourGO, mapPiecePos + eastNeighbourPosition);
			}
			else if (southConnectionPoint != null && southConnectionPoint.Occupied == false && southNeighbour == null)
			{
				GameObject southNeigbourGO = Instantiate(bottomDeadEndMapPieces.GetRandom(), mapPiecePos + southNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				MapPiece southNeighbourMapPiece = southNeigbourGO.GetComponent<MapPiece>();
				southNeighbourMapPiece.ConnectionPoints[0].Occupied = true;
				southNeighbourMapPiece.ConnectionPoints[0].ConnectedTo = southConnectionPoint;
				southConnectionPoint.Occupied = true;
				southConnectionPoint.ConnectedTo = southNeighbourMapPiece.ConnectionPoints[0];
				mapPiecesInScene.Add(southNeigbourGO, mapPiecePos + southNeighbourPosition);
			}
			else if (westConnectionPoint != null && westConnectionPoint.Occupied == false && westNeighbour == null)
			{
				GameObject westNeigbourGO = Instantiate(leftDeadEndMapPieces.GetRandom(), mapPiecePos + westNeighbourPosition, Quaternion.identity, connectedMapPiecesParent);
				MapPiece westNeighbourMapPiece = westNeigbourGO.GetComponent<MapPiece>();
				westNeighbourMapPiece.ConnectionPoints[0].Occupied = true;
				westNeighbourMapPiece.ConnectionPoints[0].ConnectedTo = westConnectionPoint;
				westConnectionPoint.Occupied = true;
				westConnectionPoint.ConnectedTo = westNeighbourMapPiece.ConnectionPoints[0];
				mapPiecesInScene.Add(westNeigbourGO, mapPiecePos + westNeighbourPosition);
			}
		}
	}
	private void SpawnEnemies()
	{
		int playerSafeZoneSize = playerSafeZoneRadii * mapPieceOffset - 1;
		Bounds safeZoneBounds = new Bounds
		{
			center = Vector2.zero,
			size = new Vector2(playerSafeZoneSize, playerSafeZoneSize)
		};

		// Spawn Regular Enemies
		foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
		{
			float distanceToSpawn = Vector2.Distance(Vector2.zero, mapPiece.Value);

			Bounds mapPieceBounds = new Bounds()
			{
				center = mapPiece.Value,
				size = new Vector2(mapPieceOffset, mapPieceOffset)
			};

			if (safeZoneBounds.Intersects(mapPieceBounds)) continue;
			if (debugMapPiecesOutsidePlayerSafeZone) Debug.Log($"<color=magenta>{mapPiece.Key.name} is outside of the Player Safe Zone and can spawn enemies</color>", mapPiece.Key);

			GameObject mapPieceGO = mapPiece.Key;
			Transform mapPieceFloorParentTransform = mapPieceGO.transform.Find("Floors");
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
				Transform mapPieceFloorParentTransform = mapPieceGO.transform.Find("Floors");
				Transform[] floorTiles = mapPieceFloorParentTransform.GetComponentsInChildren<Transform>();
				Transform spawnTile = floorTiles[Random.Range(0, floorTiles.Length)];
				Instantiate(rewardEnemyPrefabs.GetRandom(), new Vector2(spawnTile.position.x, spawnTile.position.y), Quaternion.identity, enemyParentTransform);
			}
		}
	}

	#region Helper Functions
	[Button]
	private void CleanUp()
	{
		ClearLog();

		// Remove map Pieces
		List<Transform> connectedMapPieces = connectedMapPiecesParent.GetComponentsInChildren<Transform>().ToList();
		for (int i = connectedMapPieces.Count - 1; i >= 0; i--)
		{
			Transform mapPieceTransform = connectedMapPieces[i];
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
	}
	public void ClearLog()
	{
		var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
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
	private List<ConnectionPoint> GetConnectionPoints(List<GameObject> gameObjects)
	{
		List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
		foreach (GameObject go in gameObjects)
		{
			connectionPoints.AddRange(gameObject.GetComponentsInChildren<ConnectionPoint>());
		}
		if (connectionPoints.Count > 0)
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
