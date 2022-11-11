using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int startLevel = 0;
    private int currentLevel;
    [SerializeField] private int pointToLevelBase = 1;
    [SerializeField] private int pointToLevel;
    [SerializeField] private float levelCostModifier = 1.2f;
    [SerializeField] private float dificultyModifier = 1.1f;
    private EnemyBase[] enemies;
    public static LevelManager LevelManagerInstance;
    public int PointToLevel { get => pointToLevel; private set => pointToLevel = value; }

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
	public void LevelStarting()
    {
        currentLevel = startLevel;
        pointToLevel = pointToLevelBase;
	}

    public void IncreaseLevel()
    {
        currentLevel++;
        pointToLevel = Mathf.RoundToInt( pointToLevelBase * Mathf.Pow( levelCostModifier, currentLevel ) );
        enemies = FindObjectsOfType<EnemyBase>();
		foreach( EnemyBase enemy in enemies )
		{
            enemy.ExpAmount = Mathf.RoundToInt( enemy.ExpAmountBase * Mathf.Pow( dificultyModifier, currentLevel ) );
            Debug.Log( "level increased, exp reward: " + enemy.ExpAmount );
		}
    }
}
