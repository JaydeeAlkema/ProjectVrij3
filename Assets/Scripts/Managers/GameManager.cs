using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;

	[SerializeField, Expandable] private ScriptableInt playerHP;
	[SerializeField, Expandable] private ScriptableFloat playerSpeed;
	[SerializeField] private bool isPaused = false;
	[SerializeField] private AK.Wwise.State SoundStateCalm;
	[SerializeField] private AK.Wwise.State SoundStateCrowded;
	[SerializeField] private AK.Wwise.State CurrentSoundState;
	[SerializeField] private AK.Wwise.Event startMusic;
	[SerializeField] private AK.Wwise.Event stopMusic;

	[SerializeField] private int numberOfEnemiesAggrod = 0;


	[Header("Managers")]
	[SerializeField] private LevelGeneratorV3 levelGenerator = null;
	[SerializeField] private HubSceneManager HubSceneManager = null;
	[SerializeField] private ExpManager expManager = null;
	[SerializeField] private UIManager uiManager = null;
	[SerializeField] private CheatsManager cheatsManager = null;

	[Header("Player")]
	[SerializeField] private GameObject playerInstance = null;
	[SerializeField] private ScriptablePlayer scriptablePlayer = null;

	public GameState currentGameState;
	public GameState lastGamestate;

	public static GameManager Instance { get => instance; private set => instance = value; }
	public ExpManager ExpManager { get => expManager; private set => expManager = value; }
	public UIManager UiManager { get => uiManager; private set => uiManager = value; }
	public GameObject PlayerInstance { get => playerInstance; set => playerInstance = value; }
	public ScriptablePlayer ScriptablePlayer { get => scriptablePlayer; set => scriptablePlayer = value; }
	public ScriptableInt PlayerHP { get => playerHP; set => playerHP = value; }
	public ScriptableFloat PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }
	public bool IsPaused { get => isPaused; private set => isPaused = value; }
	public int NumberOfEnemiesAggrod { get => numberOfEnemiesAggrod; set => numberOfEnemiesAggrod = value; }

	#region Unity Callbacks
	private void Awake()
	{
		if (!instance || instance != this)
		{
			instance = this;
		}

		QualitySettings.vSyncCount = 1;
		//scriptablePlayer = (ScriptablePlayer)ScriptableObject.CreateInstance("ScriptablePlayer");

		SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
	}
	#endregion

	public void Update()
	{
		if (PlayerHP.value <= 0 && currentGameState == GameState.Dungeon)
		{
			ChangeGameState(GameState.GameOver);
		}

		if(PlayerHP.value > playerHP.startValue)
		{
			playerHP.value = playerHP.startValue;
		}

		if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			TogglePauseGame();
			uiManager.SetUIActive(3, isPaused);
		}

		StopMusicForTesting();
	}

	public void EnemyAggroCount(bool isAggro)
	{
		numberOfEnemiesAggrod += isAggro ? 1 : -1;
		if (numberOfEnemiesAggrod == 0)
		{
			CurrentSoundState = SoundStateCalm;
			CurrentSoundState.SetValue();
		}
		else
		{
			CurrentSoundState = SoundStateCrowded;
			CurrentSoundState.SetValue();
		}
	}

	public void TogglePauseGame()
	{
		isPaused = !isPaused;
		Time.timeScale = isPaused ? 0f : 1f;
	}

	public void StopMusicForTesting()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			AudioManager.Instance.PostEventGlobal(stopMusic);
		}
	}

	public void SetPauseState(bool pause)
	{
		isPaused = pause;
		Time.timeScale = isPaused ? 0f : 1f;
	}

	public void FetchDungeonReferences()
	{
		if (FindObjectOfType<LevelGeneratorV3>() != null)
		{
			// Use the awake method for fetching references.
			levelGenerator = FindObjectOfType<LevelGeneratorV3>();
			HubSceneManager = FindObjectOfType<HubSceneManager>();
			expManager = FindObjectOfType<ExpManager>();
			uiManager = FindObjectOfType<UIManager>();
			cheatsManager = FindObjectOfType<CheatsManager>();

			playerInstance = FindObjectOfType<PlayerControler>().gameObject;
			playerInstance.SetActive(false);

			uiManager.SetupDungeonUI();
			uiManager.DisableAllUI();

			TMP_InputField cheatsInputfield = uiManager.UiStates[5].GetComponentInChildren<TMP_InputField>();
			cheatsInputfield.onEndEdit.AddListener(cheatsManager.ExecuteCommand);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName("Jaydee Testing Scene"));
		}

		StartCoroutine(SetupLevel());
	}

	public void SetupNonDungeon(string scene)
	{
		HubSceneManager = FindObjectOfType<HubSceneManager>();
		expManager = FindObjectOfType<ExpManager>();
		uiManager = FindObjectOfType<UIManager>();
		cheatsManager = FindObjectOfType<CheatsManager>();
		ChangeGameState(GameState.Dungeon);
		playerInstance = FindObjectOfType<PlayerControler>().gameObject;
		//playerInstance.SetActive(false);

		uiManager.SetupDungeonUI();
		uiManager.DisableAllUI();

		TMP_InputField cheatsInputfield = uiManager.UiStates[5].GetComponentInChildren<TMP_InputField>();
		cheatsInputfield.onEndEdit.AddListener(cheatsManager.ExecuteCommand);

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
		StartCoroutine(SetupSetRoom());
	}

	public void ChangeGameState(GameState newGameState)
	{
		lastGamestate = currentGameState;
		//if (newGameState != currentGameState)
		//{
			currentGameState = newGameState;
			switch (currentGameState)
			{
				case GameState.Dungeon:
					CurrentSoundState = SoundStateCrowded;
					CurrentSoundState.SetValue();
					AudioManager.Instance.PostEventGlobal(stopMusic);
					Debug.Log("Stopping music.");
					AudioManager.Instance.PostEventGlobal(startMusic);
					Debug.Log("Starting music.");
					OnGameStateChanged?.Invoke(currentGameState, lastGamestate);
					break;
				case GameState.GameOver:
					StartCoroutine(GameOver());
					OnGameStateChanged?.Invoke(currentGameState, lastGamestate);
					break;
				case GameState.Hub:
					AudioManager.Instance.PostEventGlobal(stopMusic);
					Debug.Log("Stopping music.");
					PlayerHP.ResetValue();
					ExpManager.ResetExp();
					OnGameStateChanged?.Invoke(currentGameState, lastGamestate);
					break;
				case GameState.Menu:
					//startMusic.Stop( AudioManager.Instance.gameObject );
					AudioManager.Instance.PostEventGlobal(stopMusic);
					Debug.Log("Stopping music.");
					PlayerHP.ResetValue();
					ExpManager.ResetExp();
					OnGameStateChanged?.Invoke(currentGameState, lastGamestate);
					break;
			}
		//}
	}

	/// <summary>
	/// Basic method that handles the setup of the level and player.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SetupLevel()
	{
		//Show loading screen
		uiManager.SetUIActive(2, true);

		yield return StartCoroutine(levelGenerator.Generate());
		playerInstance.SetActive(true);
		ChangeGameState(GameState.Dungeon);

		//Show dungeon HUD
		uiManager.DisableAllUI();
		uiManager.ResetAbilityUIValues();
		uiManager.SetUIActive(1, true);
	}

	private IEnumerator SetupSetRoom()
	{
		//playerInstance.SetActive(true);

		//Show dungeon HUD
		uiManager.DisableAllUI();
		uiManager.ResetAbilityUIValues();
		uiManager.SetUIActive(1, true);
		yield return new WaitForEndOfFrame();
	}

	IEnumerator GameOver()
	{
		playerInstance.GetComponent<PlayerControler>().Invulnerable = true;
		uiManager.DisableAllUI();
		playerInstance.GetComponentInChildren<CameraToMouseFollow>().gameObject.transform.localPosition = Vector3.zero;
		playerInstance.GetComponent<PlayerControler>().GameOverVFX(1);
		Time.timeScale = 0f;    //Hitstop
		yield return new WaitForSecondsRealtime(0.5f);

		playerInstance.GetComponent<PlayerControler>().GameOverVFX(2);
		playerInstance.GetComponent<PlayerControler>().enabled = false;
		SpriteRenderer[] playerSprites = playerInstance.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer playerSprite in playerSprites)
		{
			playerSprite.gameObject.SetActive(false);
		}
		Time.timeScale = 0.2f;    //Slowdown
		yield return new WaitForSecondsRealtime(2f);

		//Return to normal time
		Time.timeScale = 1f;
		yield return new WaitForSecondsRealtime(0.5f);

		//Deathscreen
		uiManager.SetUIActive(4, true);
		yield return new WaitForSecondsRealtime(3f);

		HubSceneManager.sceneManagerInstance.ChangeScene("Hub Prototype", SceneManager.GetActiveScene().name);

		PlayerHP.ResetValue();
		ExpManager.ResetExp();

		uiManager.DisableAllUI();
		uiManager.SetUIActive(0, true);
		ChangeGameState(GameState.Hub);
		yield return null;
	}

	public delegate void OnGameStateChange(GameState gameState, GameState lastGameState);

	public event OnGameStateChange OnGameStateChanged;
}


public enum GameState
{
	Hub,//1
	Dungeon,//2
	GameOver,//3
	Menu,//4
}
