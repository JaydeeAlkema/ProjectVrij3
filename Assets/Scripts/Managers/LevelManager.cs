using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int startLevel = 0;
    private int currentLevel;
    [SerializeField] private int startUpgradeCount = 1;
    [SerializeField] private int upgradeCount;
    [SerializeField] private int pointToLevelBase = 1;
    [SerializeField] private int pointToLevel;
    [SerializeField] private float levelCostModifier = 1.2f;
    [SerializeField] private float dificultyModifier = 1.1f;
    private EnemyBase[] enemies;
    public static LevelManager LevelManagerInstance;
    public float DificultyModifier { get => dificultyModifier; set => dificultyModifier = value; }
    public int PointToLevel { get => pointToLevel; private set => pointToLevel = value; }
    public int UpgradeCount { get => upgradeCount; set => upgradeCount = value; }

	private void Awake()
	{
        if( LevelManagerInstance != null && LevelManagerInstance != this )
        {
            Destroy( this );
        }
        else
        {
            LevelManagerInstance = this;
        }
        LevelStarting();
    }

	private void Start()
	{
        GameManager.Instance.OnGameStateChanged += NewSceneWithEnemies;
    }

	private void OnDestroy()
	{
        GameManager.Instance.OnGameStateChanged -= NewSceneWithEnemies;	
	}

	public void LevelStarting()
    {
        currentLevel = startLevel;
        pointToLevel = pointToLevelBase;
        upgradeCount = startUpgradeCount;
	}

    public void IncreaseLevel()
    {
        currentLevel++;
        pointToLevel = Mathf.RoundToInt( pointToLevelBase * Mathf.Pow( levelCostModifier, currentLevel ) );
		OnLevelIncrease?.Invoke( currentLevel, dificultyModifier );
	}

    public void NewSceneWithEnemies(GameState gameState, GameState lastGameState)
    {
        if( gameState == GameState.Dungeon && gameState == lastGameState )
        {
            OnLevelIncrease?.Invoke( currentLevel, dificultyModifier );
        }

        if(gameState == GameState.Dungeon && lastGameState != gameState)
        {
            LevelStarting();
		}
	}

    public delegate void LevelHasIncreased(int level, float dificulty);

    public event LevelHasIncreased OnLevelIncrease;
}
