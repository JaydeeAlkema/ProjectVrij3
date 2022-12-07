using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneManager : MonoBehaviour
{
    public static HubSceneManager sceneManagerInstance { get; private set; }
	private string loadScene;
	private string lastScene;
	private PlayerControler playerValues;

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
		if((sceneToLoad != "Hub Prototype" || sceneToLoad != "MainMenu") && GameManager.Instance.PlayerInstance != null)
		{
			if(GameManager.Instance.ScriptablePlayer != null) { GameManager.Instance.ScriptablePlayer = null; }
			GameManager.Instance.ScriptablePlayer = ( ScriptablePlayer )ScriptableObject.CreateInstance( "ScriptablePlayer" );
			GameManager.Instance.ScriptablePlayer.Player = GameManager.Instance.PlayerInstance.GetComponent<PlayerControler>();
			playerValues = GameManager.Instance.ScriptablePlayer.Player;
		}
		lastScene = currentScene;
		SceneManager.UnloadSceneAsync( currentScene );
		SceneManager.LoadSceneAsync( sceneToLoad, LoadSceneMode.Additive ).completed += HubSceneManager_completed;
		//SceneManager.LoadSceneAsync( "Scene Manager" );
		//SceneManager.LoadSceneAsync("UITest");
	}

	private void HubSceneManager_completed( AsyncOperation obj )
	{
		SceneManager.SetActiveScene( SceneManager.GetSceneByName( loadScene ) );
		if( GameManager.Instance.currentGameState == GameState.Dungeon && GameManager.Instance.lastGamestate == GameManager.Instance.currentGameState) { HoldPlayerOnSceneLoad(); }
		Debug.Log( "hold player stats" );
	}

	public void StartFirstScenes()
	{
		SceneManager.LoadSceneAsync( "UITest", LoadSceneMode.Additive );
		SceneManager.LoadSceneAsync( "Hub Prototype", LoadSceneMode.Additive );
	}

	private void HoldPlayerOnSceneLoad()
	{
		PlayerControler player = FindObjectOfType<PlayerControler>().gameObject.GetComponent<PlayerControler>();
		GameManager.Instance.PlayerInstance = player.gameObject;
		player.PlayerAbilityController = playerValues.PlayerAbilityController;
		player.PlayerAbilityController.Player = player;
		player.CurrentMeleeAttack = playerValues.CurrentMeleeAttack;
		player.CurrentMeleeAttack.Player = player;
		player.CurrentMeleeAttack.Init = playerValues.CurrentMeleeAttack.Init;
		player.MeleeAttackScr.SetHoldStats(playerValues.MeleeAttackScr);
		player.CurrentMeleeAttack.Damage = playerValues.CurrentMeleeAttack.Damage;
		player.CurrentRangedAttack = playerValues.CurrentRangedAttack;
		player.CurrentRangedAttack.Player = player;
		player.CurrentRangedAttack.Init = playerValues.CurrentRangedAttack.Init;
		player.RangedAttackScr.SetHoldStats(playerValues.RangedAttackScr);
		player.CurrentDash = playerValues.CurrentDash;
		player.CurrentDash.Player = player;
		player.CurrentDash.Init = playerValues.CurrentDash.Init;
		player.Dash.SetHoldStats(playerValues.Dash);
		if(playerValues.CurrentAbility1 != null)
		{ 
			player.CurrentAbility1 = playerValues.CurrentAbility1;
			player.CurrentAbility1.Player = player;
			player.Ability1.SetHoldStats(playerValues.Ability1);
			player.CurrentAbility1.Init = playerValues.CurrentAbility1.Init;
		}
		if(playerValues.CurrentAbility2 != null)
		{
			player.CurrentAbility2 = playerValues.CurrentAbility2;
			player.CurrentAbility2.Player = player;
			player.Ability2.SetHoldStats(playerValues.Ability2);
			player.CurrentAbility2.Init = playerValues.CurrentAbility2.Init;
		}
		if(playerValues.CurrentAbility3 != null)
		{
			player.CurrentAbility3 = playerValues.CurrentAbility3;
			player.CurrentAbility3.Player = player;
			player.Ability3.SetHoldStats(playerValues.Ability3);
			player.CurrentAbility3.Init = playerValues.CurrentAbility3.Init;
		}
		if(playerValues.CurrentAbility4 != null)
		{
			player.CurrentAbility4 = playerValues.CurrentAbility4;
			player.CurrentAbility4.Player = player;
			player.Ability4.SetHoldStats(playerValues.Ability4);
			player.CurrentAbility4.Init = playerValues.CurrentAbility4.Init;
		}
		//player.ReloadAttacks();
		player.initAbilities();
		Debug.Log( player.CurrentMeleeAttack.AbilityUpgrades.Count );
		Debug.Log( player.CurrentRangedAttack.AbilityUpgrades.Count );
		//GameManager.Instance.PlayerInstance = null;
	}
}
