using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneManager : MonoBehaviour
{
	public static HubSceneManager sceneManagerInstance { get; private set; }
	private string loadScene;
	private string lastScene;
	[SerializeField] private PlayerControler playerValues;
	[SerializeField] private PlayerControler player;

	private void Awake()
	{
		DontDestroyOnLoad( this );
		if( sceneManagerInstance != null && sceneManagerInstance != this )
		{
			Destroy( this );
		}
		else
		{
			sceneManagerInstance = this;
		}
	}

	public void ChangeScene( string sceneToLoad, string currentScene )
	{
		loadScene = sceneToLoad;
		if( GameManager.Instance.ScriptablePlayer != null ) { GameManager.Instance.ScriptablePlayer = null; }
		GameManager.Instance.ScriptablePlayer = ( ScriptablePlayer )ScriptableObject.CreateInstance( "ScriptablePlayer" );
		GameManager.Instance.ScriptablePlayer.Player = GameManager.Instance.PlayerInstance.GetComponent<PlayerControler>();
		GameManager.Instance.ScriptablePlayer.AbilityController = GameManager.Instance.PlayerInstance.GetComponent<AbilityController>();
		playerValues = GameManager.Instance.ScriptablePlayer.Player;

		lastScene = currentScene;
		SceneManager.UnloadSceneAsync( currentScene );
		SceneManager.LoadSceneAsync( sceneToLoad, LoadSceneMode.Additive ).completed += HubSceneManager_completed;
		//SceneManager.LoadSceneAsync( "Scene Manager" );
		//SceneManager.LoadSceneAsync("UITest");
	}

	private void HubSceneManager_completed( AsyncOperation obj )
	{
		SceneManager.SetActiveScene( SceneManager.GetSceneByName( loadScene ) );
		if( loadScene != "Hub Prototype" && lastScene != "Hub Prototype" ) { HoldPlayerOnSceneLoad(); }
		Debug.Log( "hold player stats" );
	}

	public void StartFirstScenes()
	{
		SceneManager.LoadSceneAsync( "UITest", LoadSceneMode.Additive );
		SceneManager.LoadSceneAsync( "Hub Prototype", LoadSceneMode.Additive );
	}

	private void HoldPlayerOnSceneLoad()
	{
		//PlayerControler player = FindObjectOfType<PlayerControler>().gameObject.GetComponent<PlayerControler>();
		player = FindObjectOfType<PlayerControler>();
		GameManager.Instance.PlayerInstance = player.gameObject;
		GameManager.Instance.UiManager.ResetAbilityUIValues();
		player.PlayerAbilityController = playerValues.PlayerAbilityController;
		player.PlayerAbilityController.Player = player;
		#region abilitycontroller sets
		AbilityController.AbilityControllerInstance.CurrentAbility1 = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentAbility1;
		AbilityController.AbilityControllerInstance.CurrentAbility2 = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentAbility2;
		AbilityController.AbilityControllerInstance.CurrentAbility3 = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentAbility3;
		AbilityController.AbilityControllerInstance.CurrentAbility4 = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentAbility4;
		AbilityController.AbilityControllerInstance.CurrentDash = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentDash;
		AbilityController.AbilityControllerInstance.CurrentRangedAttack = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentRangedAttack;
		AbilityController.AbilityControllerInstance.CurrentMeleeAttack = GameManager.Instance.ScriptablePlayer.AbilityController.CurrentMeleeAttack;
		#endregion
		player.CurrentMeleeAttack = playerValues.CurrentMeleeAttack;
		player.CurrentMeleeAttack.Player = player;
		player.CurrentMeleeAttack.Init = playerValues.CurrentMeleeAttack.Init;
		player.MeleeAttackScr.SetHoldStats( playerValues.MeleeAttackScr );
		player.CurrentMeleeAttack.Damage = playerValues.CurrentMeleeAttack.Damage;
		player.CurrentRangedAttack = playerValues.CurrentRangedAttack;
		player.CurrentRangedAttack.Player = player;
		player.CurrentRangedAttack.Init = playerValues.CurrentRangedAttack.Init;
		player.RangedAttackScr.SetHoldStats( playerValues.RangedAttackScr );
		player.CurrentDash = playerValues.CurrentDash;
		player.CurrentDash.Player = player;
		player.CurrentDash.Init = playerValues.CurrentDash.Init;
		player.Dash.SetHoldStats( playerValues.Dash );
		if( playerValues.CurrentAbility1 != null )
		{
			player.CurrentAbility1 = playerValues.CurrentAbility1;
			player.CurrentAbility1.Player = player;
			player.Ability1 = playerValues.Ability1;
			player.Ability1.SetHoldStats( playerValues.Ability1 );
			player.CurrentAbility1.Init = playerValues.CurrentAbility1.Init;
			GameManager.Instance.UiManager.SetAbilityUIValues( 1, player.CurrentAbility1.BaseStats.AbilityIcon );
		}
		if( playerValues.CurrentAbility2 != null )
		{
			player.CurrentAbility2 = playerValues.CurrentAbility2;
			player.CurrentAbility2.Player = player;
			player.Ability2 = playerValues.Ability2;
			player.Ability2.SetHoldStats( playerValues.Ability2 );
			player.CurrentAbility2.Init = playerValues.CurrentAbility2.Init;
			GameManager.Instance.UiManager.SetAbilityUIValues( 2, player.CurrentAbility2.BaseStats.AbilityIcon );
		}
		if( playerValues.CurrentAbility3 != null )
		{
			player.CurrentAbility3 = playerValues.CurrentAbility3;
			player.CurrentAbility3.Player = player;
			player.Ability3 = playerValues.Ability3;
			player.Ability3.SetHoldStats( playerValues.Ability3 );
			player.CurrentAbility3.Init = playerValues.CurrentAbility3.Init;
			GameManager.Instance.UiManager.SetAbilityUIValues( 3, player.CurrentAbility3.BaseStats.AbilityIcon);
		}
		if( playerValues.CurrentAbility4 != null )
		{
			player.CurrentAbility4 = playerValues.CurrentAbility4;
			player.CurrentAbility4.Player = player;
			player.Ability4 = playerValues.Ability4;
			player.Ability4.SetHoldStats( playerValues.Ability4 );
			player.CurrentAbility4.Init = playerValues.CurrentAbility4.Init;
			GameManager.Instance.UiManager.SetAbilityUIValues( 4, player.CurrentAbility4.BaseStats.AbilityIcon);
		}
		//player.ReloadAttacks();
		player.initAbilities();
		Debug.Log( player.CurrentMeleeAttack.AbilityUpgrades.Count );
		Debug.Log( player.CurrentRangedAttack.AbilityUpgrades.Count );
		//GameManager.Instance.PlayerInstance = null;
	}
}
