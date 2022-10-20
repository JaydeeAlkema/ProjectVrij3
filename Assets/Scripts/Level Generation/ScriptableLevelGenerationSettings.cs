using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Generation Settings", menuName = "ScriptableObjects/New Level Generation Settings")]
public class ScriptableLevelGenerationSettings : ScriptableObject
{
	[Foldout("Base Level Generation")] public int seed;
	[Foldout("Base Level Generation")] public int chunkSize = 35;
	[Foldout("Base Level Generation")] public int maxRooms = 10;
	[Foldout("Base Level Generation")] public int pathDepth = 2; // How far from the center the path will generate extra path tiles. So a depth of 2 results in a path that is 5 wide in total. Example: (## # ##)
	[Foldout("Base Level Generation")] public Vector2Int chunkGridSize = new Vector2Int(10, 10);
	[Foldout("Base Level Generation")] public List<ScriptableRoom> spawnableRooms = new List<ScriptableRoom>();

	[Foldout("Sprites")] public List<Sprite> floorSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> topWallSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> bottomWallSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> leftWallSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> rightWallSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> topLeftOuterCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> topRightOuterCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> bottomLeftOuterCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> bottomRightOuterCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> topLeftInnerCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> topRightInnerCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> bottomLeftInnerCornerSprites = new List<Sprite>();
	[Foldout("Sprites")] public List<Sprite> bottomRightInnerCornerSprites = new List<Sprite>();

	[Foldout("Enemy References")] public List<GameObject> fodderEnemies = new List<GameObject>();
	[Foldout("Enemy References")] public List<GameObject> rewardEnemies = new List<GameObject>();
}
