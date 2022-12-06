using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Generation Settings", menuName = "ScriptableObjects/New Level Generation Settings")]
public class ScriptableLevelGenerationSettings : ScriptableObject
{
	[Foldout("Base Level Generation")] public int chunkSize = 35;
	[Foldout("Base Level Generation")] public int maxRooms = 10;
	[Foldout("Base Level Generation")] public int pathDepth = 2; // How far from the center the path will generate extra path tiles. So a depth of 2 results in a path that is 5 wide in total. Example: (## # ##)
	[Foldout("Base Level Generation")] public int decorationSpawnChance = 5;
	[Foldout("Base Level Generation")] public Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[Foldout("Base Level Generation")] public List<ScriptableRoom> spawnRooms = new List<ScriptableRoom>();
	[Foldout("Base Level Generation")] public List<ScriptableRoom> genericRooms = new List<ScriptableRoom>();
	[Foldout("Base Level Generation")] public List<ScriptableRoom> bossRooms = new List<ScriptableRoom>();

	[Foldout("Decorations")] public WeightedRandomList<GameObject> floorDecorationPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Decorations")] public WeightedRandomList<GameObject> topWallDecorationPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Decorations")] public WeightedRandomList<GameObject> leftWallDecorationPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Decorations")] public WeightedRandomList<GameObject> rightWallDecorationPrefabs = new WeightedRandomList<GameObject>();

	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> floorPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> topWallPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> middleWallPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> bottomWallPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> leftWallPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> rightWallPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> topLeftOuterCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> topRightOuterCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> bottomLeftOuterCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> bottomRightOuterCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> topLeftInnerCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> topRightInnerCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> bottomLeftInnerCornerPrefabs = new WeightedRandomList<GameObject>();
	[Foldout("Environmental Sprites")] public WeightedRandomList<GameObject> bottomRightInnerCornerPrefabs = new WeightedRandomList<GameObject>();

	[Foldout("Enemy Generation")] public EnemyGroup EnemyFodderGroup = new EnemyGroup();
	[Foldout("Enemy Generation")] public EnemyGroup EnemyRewardGroup = new EnemyGroup();
}

public enum EnemyType
{
	Fodder = 1,
	Reward = 2,
};

public enum EnemySpawnMode
{
	Singulair,
	Grouped
}

[System.Serializable]
public struct EnemyGroup
{
	public EnemyType enemyType;
	public EnemySpawnMode enemySpawnMode;
	public WeightedRandomList<GameObject> enemyPrefabs;
	[AllowNesting, MinMaxSlider(0, 10), ShowIf("enemySpawnMode", EnemySpawnMode.Grouped)] public Vector2Int groupCountPerRoom;
	[AllowNesting, MinMaxSlider(0, 10), ShowIf("enemySpawnMode", EnemySpawnMode.Grouped)] public Vector2Int enemyCountPerGroup;
}
