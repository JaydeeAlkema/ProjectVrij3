using NaughtyAttributes;
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

	[SerializeField, BoxGroup("Map Piece References")] private Transform connectedMapPiecesParent = default;
	[SerializeField, BoxGroup("Map Piece References")] private Transform disconnectedMapPiecesParent = default;
	[SerializeField, BoxGroup("Map Piece References")] private GameObject spawnMapPiece = default;
	[SerializeField, BoxGroup("Map Piece References")] private WeightedRandomList<GameObject> mapPieces = new WeightedRandomList<GameObject>();

	[SerializeField, BoxGroup("Runtime Variables"), ReadOnly] private List<GameObject> mapPiecesInScene = new List<GameObject>();

	[SerializeField, BoxGroup("Debug Variables")] private float debugTime = 0.5f;
	[SerializeField, BoxGroup("Debug Variables")] private bool debugOverlaps = false;
	[SerializeField, BoxGroup("Debug Variables")] private bool debugConnections = false;
	[SerializeField, BoxGroup("Debug Variables")] private int goodGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int okGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int passibleGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int badGenerationTime;
	[SerializeField, BoxGroup("Debug Variables")] private int TerribleGenerationTime;

	private Dictionary<ConnectionPoint, Vector2> connectionPointsInScene = new Dictionary<ConnectionPoint, Vector2>();
	private Vector2 overlapSize = new Vector2(20, 20);

	private void Start()
	{
		Generate();
	}

	[Button]
	private void Generate()
	{
#if UNITY_EDITOR
		CleanUp();
#endif
		if (generateRandomSeed) seed = Random.Range(0, int.MaxValue);
		Random.InitState(seed);

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

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
				foreach (GameObject mapPieceInScene in mapPiecesInScene)
				{
					List<ConnectionPoint> mapPieceInSceneConnectionPoints = GetConnectionPoints(mapPieceInScene);

					foreach (ConnectionPoint mapPieceInSceneConnectionPoint in mapPieceInSceneConnectionPoints)
					{
						if (mapPieceInSceneConnectionPoint.Occupied == false && !connected)
						{
							GameObject mapPieceInSceneGO = mapPieceInScene;

							// Connect North to South
							if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.North && newMapPieceConnectionPointSouth != null && newMapPieceConnectionPointSouth.Occupied == false)
							{
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x, mapPieceInSceneGO.transform.position.y + 21);
								if (Overlap(newMapPiecePos, overlapSize)) continue;

								newMapPieceConnectionPointSouth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect East to West
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.East && newMapPieceConnectionPointWest != null && newMapPieceConnectionPointWest.Occupied == false)
							{
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x + 21, mapPieceInSceneGO.transform.position.y);
								if (Overlap(newMapPiecePos, overlapSize)) continue;

								newMapPieceConnectionPointWest.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect South to North
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.South && newMapPieceConnectionPointNorth != null && newMapPieceConnectionPointNorth.Occupied == false)
							{
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x, mapPieceInSceneGO.transform.position.y - 21);
								if (Overlap(newMapPiecePos, overlapSize)) continue;

								newMapPieceConnectionPointNorth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect West to East
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.West && newMapPieceConnectionPointEast != null && newMapPieceConnectionPointEast.Occupied == false)
							{
								if (debugConnections) Debug.Log($"Connecting {newMapPieceGO.name} to {mapPieceInScene.name}", newMapPieceGO);
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x - 21, mapPieceInSceneGO.transform.position.y);
								if (Overlap(newMapPiecePos, overlapSize)) continue;

								newMapPieceConnectionPointEast.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
						}
					}
				}

				if (!connected && i > 1 && i < mapPieceLimit)
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
					newMapPieceGO.transform.position = newMapPiecePos;
					newMapPieceGO.transform.parent = connectedMapPiecesParent;
					mapPiecesInScene.Add(newMapPieceGO);
					newMapPieceGO = null;
				}
			}
		}

		RemoveDisconnectedMapPieces();

		stopwatch.Stop();
		long generationTime = stopwatch.ElapsedMilliseconds;
		string colorString = "";

		if (generationTime < goodGenerationTime) colorString = "lime";
		else if (generationTime > goodGenerationTime && generationTime < passibleGenerationTime) colorString = "yellow";
		else if (generationTime > passibleGenerationTime && generationTime < okGenerationTime) colorString = "orange";
		else if (generationTime > okGenerationTime && generationTime < badGenerationTime) colorString = "brown";
		else if (generationTime > TerribleGenerationTime) colorString = "red";

		Debug.Log($"<color={colorString}>Level Generation took: {generationTime}ms</color>");
	}

	#region Helper Functions
	[Button]
	private void CleanUp()
	{
		foreach (GameObject mapPieceGO in mapPiecesInScene.ToList())
		{
			DestroyImmediate(mapPieceGO);
		}
		mapPiecesInScene.Clear();
		connectionPointsInScene.Clear();
		ClearLog();
		RemoveDisconnectedMapPieces();
	}
	private void RemoveDisconnectedMapPieces()
	{
		foreach (Transform disconnectedMapPieceTransform in disconnectedMapPiecesParent.GetComponentsInChildren<Transform>())
		{
			if (disconnectedMapPieceTransform != disconnectedMapPiecesParent && disconnectedMapPieceTransform != null)
			{
				if (Application.isEditor)
					DestroyImmediate(disconnectedMapPieceTransform.gameObject);
				else
					Destroy(disconnectedMapPieceTransform.gameObject);
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
	private bool Overlap(Vector2 centerPoint, Vector2 size)
	{
		Bounds bounds = new Bounds();
		bounds.center = centerPoint;
		bounds.size = size;

		foreach (GameObject mapPiece in mapPiecesInScene)
		{
			Bounds mapPieceBounds = new Bounds();
			mapPieceBounds.center = mapPiece.transform.position;
			mapPieceBounds.size = size;

			if (bounds.Intersects(mapPieceBounds))
			{
				if (debugOverlaps) Debug.Log($"Overlap found with {mapPiece.name} at position {mapPiece.transform.position}", mapPiece);
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
	#endregion
}
