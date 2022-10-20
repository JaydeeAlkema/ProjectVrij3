using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Generation Settings", menuName = "ScriptableObjects/New Level Generation Settings")]
public class ScriptableLevelGenerationSettings : ScriptableObject
{
	[Foldout("Base Level Generation")] public List<ScriptableRoom> spawnableRooms = new List<ScriptableRoom>();
	[Foldout("Base Level Generation")] public List<Sprite> floorSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> topWallSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> bottomWallSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> leftWallSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> rightWallSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> topLeftOuterCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> topRightOuterCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> bottomLeftOuterCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> bottomRightOuterCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> topLeftInnerCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> topRightInnerCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> bottomLeftInnerCornerSprites = new List<Sprite>();
	[Foldout("Base Level Generation")] public List<Sprite> bottomRightInnerCornerSprites = new List<Sprite>();

	[Foldout("Enemy References")] public List<GameObject> fodderEnemies = new List<GameObject>();
	[Foldout("Enemy References")] public List<GameObject> rewardEnemies = new List<GameObject>();
}
