using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneManager : MonoBehaviour
{
    public static HubSceneManager sceneManagerInstance { get; private set; }
	private string loadScene;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		if( sceneManagerInstance != null && sceneManagerInstance != this )
		{
			Destroy( this );
		}
		else
		{
			sceneManagerInstance = this;
		}
	}

	public void ChangeScene(string sceneToLoad, string currentScene)
	{
		loadScene = sceneToLoad;
		if(sceneToLoad != "Hub Prototype")
		{
			GameManager.Instance.ScriptablePlayer.Player = GameManager.Instance.PlayerInstance.GetComponent<PlayerControler>();
		}
		SceneManager.UnloadSceneAsync( currentScene );
		SceneManager.LoadSceneAsync( sceneToLoad, LoadSceneMode.Additive ).completed += HubSceneManager_completed;
		//SceneManager.LoadSceneAsync( "Scene Manager" );
		//SceneManager.LoadSceneAsync("UITest");
	}

	private void HubSceneManager_completed( AsyncOperation obj )
	{
		SceneManager.SetActiveScene( SceneManager.GetSceneByName( loadScene ) );
		PlayerControler playerValues = GameManager.Instance.ScriptablePlayer.Player;
		PlayerControler player = FindObjectOfType<PlayerControler>().gameObject.GetComponent<PlayerControler>();
		GameManager.Instance.PlayerInstance = player.gameObject;
		player.CurrentMeleeAttack = playerValues.CurrentMeleeAttack;
		player.MeleeAttackScr = playerValues.MeleeAttackScr;
		player.CurrentRangedAttack = playerValues.CurrentRangedAttack;
		player.RangedAttackScr = playerValues.RangedAttackScr;
		player.CurrentDash = playerValues.CurrentDash;
		player.Dash = playerValues.Dash;
		player.ReloadAttacks();
		Debug.Log( player.CurrentMeleeAttack.AbilityUpgrades.Count );
		Debug.Log( player.CurrentRangedAttack.AbilityUpgrades.Count );
	}

	public void StartFirstScenes()
	{
		SceneManager.LoadSceneAsync( "UITest", LoadSceneMode.Additive );
		SceneManager.LoadSceneAsync( "Hub Prototype", LoadSceneMode.Additive );
	}
}
