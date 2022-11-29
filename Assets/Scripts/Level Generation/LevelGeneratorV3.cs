using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LevelGeneratorV3 : MonoBehaviour
{
	[SerializeField] private int seed = 0;
	[SerializeField] private int mapPieceLimit = 100;
	[SerializeField] private Transform mapPiecesParent = default;
	[SerializeField] private GameObject spawnMapPiece = default;
	[SerializeField] private WeightedRandomList<GameObject> mapPieces = new WeightedRandomList<GameObject>();
	[Space]
	[SerializeField] private List<GameObject> mapPiecesInScene = new List<GameObject>();
	[Space]
	[SerializeField] private float debugTime = 0.5f;

	private Dictionary<ConnectionPoint, Vector2> connectionPointsInScene = new Dictionary<ConnectionPoint, Vector2>();

	private void Awake()
	{
		if (seed == 0) seed = Random.Range(0, int.MaxValue);
		Random.InitState(seed);
	}

	private void Start()
	{
		Generate();
	}

	private void Generate()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		for (int i = 0; i < mapPieceLimit; i++)
		{
			// Store all current connection points in the scene in a simple list for later use.
			List<ConnectionPoint> levelConnectionPoints = GetConnectionPoints(mapPiecesInScene);
			foreach (ConnectionPoint connectionPoint in levelConnectionPoints)
			{
				Vector3 connectionPointPosition = connectionPoint.transform.position;
				connectionPointsInScene.TryAdd(connectionPoint, new Vector2(connectionPointPosition.x, connectionPointPosition.y));
			}

			// Instantiate a new Map Piece. if this is the first Map Piece, then it must be the spawn Map Piece.
			GameObject newMapPieceGO = null;
			if (i == 0)
			{
				newMapPieceGO = Instantiate(spawnMapPiece);
			}
			else
			{
				newMapPieceGO = Instantiate(mapPieces.GetRandom());
			}

			// Get all connection points from the new map piece.
			List<ConnectionPoint> NewMapPieceConnectionPoints = GetConnectionPoints(newMapPieceGO);
			ConnectionPoint newMapPieceConnectionPointNorth = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.North);
			ConnectionPoint newMapPieceConnectionPointEast = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.East);
			ConnectionPoint newMapPieceConnectionPointSouth = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.South);
			ConnectionPoint newMapPieceConnectionPointWest = NewMapPieceConnectionPoints.Find(x => x.Direction == ConnectionPointDirection.West);

			// Loop through all the map pieces in the scene. The first map piece that is found that has an unoccupied connection point will be set as reference.
			// If the set reference to the existing map piece does not connect with the new map piece, we continue the loop until we find two map pieces that fit together.
			Vector2 newMapPiecePos = new Vector2();
			bool connected = false;
			if (!connected)
			{
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
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x, mapPieceInSceneGO.transform.position.y + 21);
								newMapPieceConnectionPointSouth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect East to West
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.East && newMapPieceConnectionPointWest != null && newMapPieceConnectionPointWest.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x + 21, mapPieceInSceneGO.transform.position.y);
								newMapPieceConnectionPointWest.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect South to North
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.South && newMapPieceConnectionPointNorth != null && newMapPieceConnectionPointNorth.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x, mapPieceInSceneGO.transform.position.y - 21);
								newMapPieceConnectionPointNorth.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
							// Connect West to East
							else if (mapPieceInSceneConnectionPoint.Direction == ConnectionPointDirection.West && newMapPieceConnectionPointEast != null && newMapPieceConnectionPointEast.Occupied == false)
							{
								newMapPiecePos = new Vector2(mapPieceInSceneGO.transform.position.x - 21, mapPieceInSceneGO.transform.position.y);
								newMapPieceConnectionPointEast.Occupied = true;
								mapPieceInSceneConnectionPoint.Occupied = true;
								connected = true;
							}
						}
					}
				}
			}

			newMapPieceGO.transform.position = newMapPiecePos;
			newMapPieceGO.transform.parent = mapPiecesParent;
			mapPiecesInScene.Add(newMapPieceGO);
		}

		stopwatch.Stop();
		Debug.Log($"Level Generation took: {stopwatch.ElapsedMilliseconds}");
	}

	#region Helper Functions
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
