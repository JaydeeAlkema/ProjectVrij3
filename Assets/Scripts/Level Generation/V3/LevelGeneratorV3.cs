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

	[SerializeField, BoxGroup("Enemy Settings")] private AstarPath astarData = null;
	[SerializeField, BoxGroup("Enemy Settings")] private Transform enemyParentTransform = null;
	[Space]
	[SerializeField, BoxGroup("Enemy Settings")] private int playerSafeZoneRadii = 2;
	[SerializeField, BoxGroup("Enemy Settings")] private int enemyCountPerMapPiece = 5;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> enemyPrefabs = new WeightedRandomList<GameObject>();
	[SerializeField, BoxGroup("Enemy Settings")] private int rewardEnemyCountPerLevel = 3;
	[SerializeField, BoxGroup("Enemy Settings")] private WeightedRandomList<GameObject> rewardEnemyPrefabs = new WeightedRandomList<GameObject>();

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

	private void SpawnEnemies()
	{
		foreach (KeyValuePair<GameObject, Vector2> mapPiece in mapPiecesInScene)
		{
			float distanceToSpawn = Vector2.Distance(Vector2.zero, mapPiece.Value);
			int playerSafeZoneSize = playerSafeZoneRadii * mapPieceOffset - 1;

			Bounds safeZoneBounds = new Bounds
			{
				center = Vector2.zero,
				size = new Vector2(playerSafeZoneSize, playerSafeZoneSize)
			};

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

			List<int> spawnIndeces = new List<int>();
			// ec = EnemyCount
			for (int ec = 0; ec < enemyCountPerMapPiece; ec++)
			{
				int spawnIndex = Random.Range(1, floorTiles.Length);
				while (spawnIndeces.Contains(spawnIndex))
				{
					spawnIndex = Random.Range(1, floorTiles.Length);
				}
				spawnIndeces.Add(spawnIndex);
			}

			foreach (int spawnIndex in spawnIndeces)
			{
				Transform spawnTile = floorTiles[spawnIndex];
				Instantiate(enemyPrefabs.GetRandom(), new Vector2(spawnTile.position.x, spawnTile.position.y), Quaternion.identity, enemyParentTransform);
			}
		}

		Bounds mapBounds = GetMaxBounds(connectedMapPiecesParent.gameObject);
		foreach (GridGraph gridGraph in astarData.graphs)
		{
			gridGraph.center = mapBounds.center;
			gridGraph.SetDimensions((int)mapBounds.size.x, (int)mapBounds.size.y, gridGraph.nodeSize);
			AstarPath.active.Scan(gridGraph);
		}
	}

	private void SetMapPieceNeighbours()
	{
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
			mapPiece.GetConnectionPointsFromChildren();

			Vector2 topNeighbourPos = new Vector2(mapPiecePos.x, mapPiecePos.y + mapPieceOffset);
			Vector2 rightNeighbourPos = new Vector2(mapPiecePos.x + mapPieceOffset, mapPiecePos.y);
			Vector2 bottomNeighbourPos = new Vector2(mapPiecePos.x, mapPiecePos.y - mapPieceOffset);
			Vector2 leftNeighbourPos = new Vector2(mapPiecePos.x - mapPieceOffset, mapPiecePos.y);

			GameObject topNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == topNeighbourPos).Key;
			GameObject rightNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == rightNeighbourPos).Key;
			GameObject bottomNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == bottomNeighbourPos).Key;
			GameObject leftNeighbourGO = mapPiecesInScene.FirstOrDefault(x => x.Value == leftNeighbourPos).Key;

			bool hasTopNeighbour = topNeighbourGO != null;
			bool hasRightNeighbour = rightNeighbourGO != null;
			bool hasBottomNeighbour = bottomNeighbourGO != null;
			bool hasLeftNeighbour = leftNeighbourGO != null;

			if (hasTopNeighbour && topNeighbourGO != null) mapPiece.AddNeighbour(topNeighbourGO);
			if (hasRightNeighbour && rightNeighbourGO != null) mapPiece.AddNeighbour(rightNeighbourGO);
			if (hasBottomNeighbour && bottomNeighbourGO != null) mapPiece.AddNeighbour(bottomNeighbourGO);
			if (hasLeftNeighbour && leftNeighbourGO != null) mapPiece.AddNeighbour(leftNeighbourGO);
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
				if (Application.isEditor)
				{
					DestroyImmediate(mapPieceTransform.gameObject);
				}
				else
				{
					Destroy(mapPieceTransform.gameObject);
				}
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
				if (Application.isEditor)
				{
					DestroyImmediate(enemyTransform.gameObject);
				}
				else
				{
					Destroy(enemyTransform.gameObject);
				}
			}

		}
	}
	private void RemoveDisconnectedMapPieces()
	{
		foreach (Transform disconnectedMapPieceTransform in disconnectedMapPiecesParent.GetComponentsInChildren<Transform>())
		{
			if (disconnectedMapPieceTransform != disconnectedMapPiecesParent && disconnectedMapPieceTransform != null)
			{
				GameObject disconnectedMapPieceGO = disconnectedMapPieceTransform.gameObject;
				if (Application.isEditor)
					DestroyImmediate(disconnectedMapPieceGO);
				else
					Destroy(disconnectedMapPieceGO);

				mapPiecesInScene.Remove(disconnectedMapPieceGO);
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
		// https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children
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
