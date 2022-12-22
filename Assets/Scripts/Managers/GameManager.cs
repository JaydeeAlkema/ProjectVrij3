using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;

	[SerializeField, BoxGroup("Audio")] private bool isPaused = false;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.State SoundStateCalm;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.State SoundStateCrowded;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.State CurrentSoundState;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.Event startMusic;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.Event stopMusic;
	[SerializeField, BoxGroup("Audio")] private AK.Wwise.Event dungeonAmbience;

	[SerializeField, BoxGroup("Managers")] private LevelGeneratorV3 levelGenerator = null;
	[SerializeField, BoxGroup("Managers")] private HubSceneManager HubSceneManager = null;
	[SerializeField, BoxGroup("Managers")] private ExpManager expManager = null;
	[SerializeField, BoxGroup("Managers")] private UIManager uiManager = null;
	[SerializeField, BoxGroup("Managers")] private CheatsManager cheatsManager = null;

	[SerializeField, BoxGroup("Player")] private GameObject playerInstance = null;
	[SerializeField, BoxGroup("Player")] private ScriptablePlayer scriptablePlayer = null;
	[SerializeField, BoxGroup("Player")] private ScriptableInt playerHP;
	[SerializeField, BoxGroup("Player")] private ScriptableFloat playerSpeed;

	[SerializeField, BoxGroup("Minimap")] private GameObject minimapCamera = null;

	[SerializeField, BoxGroup("Runtime References")] private GameState currentGameState;
	[SerializeField, BoxGroup("Runtime References")] private GameState lastGamestate;
	[SerializeField, BoxGroup("Runtime References")] private int numberOfEnemiesAggrod = 0;
	[SerializeField, BoxGroup("Runtime References")] private int maxDungeonFloor = 3;
	[SerializeField, BoxGroup("Runtime References")] private int currentDungeonFloor = 1;

	#region Properties
	public static GameManager Instance { get => instance; private set => instance = value; }
	public ExpManager ExpManager { get => expManager; private set => expManager = value; }
	public UIManager UiManager { get => uiManager; private set => uiManager = value; }
	public GameObject PlayerInstance { get => playerInstance; set => playerInstance = value; }
	public ScriptablePlayer ScriptablePlayer { get => scriptablePlayer; set => scriptablePlayer = value; }
	public ScriptableInt PlayerHP { get => playerHP; set => playerHP = value; }
	public ScriptableFloat PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }
	public bool IsPaused { get => isPaused; private set => isPaused = value; }
	public int NumberOfEnemiesAggrod { get => numberOfEnemiesAggrod; set => numberOfEnemiesAggrod = value; }
	public GameState CurrentGameState { get => currentGameState; set => currentGameState = value; }
	public GameState LastGamestate { get => lastGamestate; set => lastGamestate = value; }
	public int MaxDungeonFloor { get => maxDungeonFloor; set => maxDungeonFloor = value; }
	public int CurrentDungeonFloor { get => currentDungeonFloor; set => currentDungeonFloor = value; }
	#endregion

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

		if (PlayerHP.value > playerHP.startValue)
		{
			playerHP.value = playerHP.startValue;
		}

		if (isPaused)
		{
			Time.timeScale = 0f;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseGame();
			uiManager.SetUIActive(3, isPaused);
			uiManager.ToggleMapOverlay(false);
		}

		StopMusicForTesting();
		//Hold tab to show static dungeon map
		if (Input.GetKeyDown(KeyCode.Tab) && !isPaused)
		{
			uiManager.ToggleMapOverlay(true);
		}

		if (Input.GetKeyUp(KeyCode.Tab))
		{
			uiManager.ToggleMapOverlay(false);
		}
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

	private void SetupMinimapCamera()
	{
		Bounds levelBounds = new Bounds();
		foreach (KeyValuePair<GameObject, Vector2> mappiece in levelGenerator.MapPiecesInScene)
		{
			Bounds mappieceBounds = new Bounds()
			{
				size = levelGenerator.OverlapSize,
				center = mappiece.Value
			};
			levelBounds.Encapsulate(mappieceBounds);
		}
		minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");
		minimapCamera.transform.position = new Vector3(levelBounds.center.x, levelBounds.center.y, -100);
		minimapCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(levelBounds.size.x, levelBounds.size.y) * 0.5f;
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
				//if (dungeonAmbience != null)
				//{
				//	AudioManager.Instance.PostEventGlobal(dungeonAmbience); //Start ambience, also implement stop ambience when ready
				//}
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
		SetupMinimapCamera();
		uiManager.ToggleMapOverlay(false);
		ChangeGameState(GameState.Dungeon);

		//Show dungeon HUD
		uiManager.DisableAllUI();
		//uiManager.ResetAbilityUIValues();
		uiManager.SetUIActive(1, true);
	}

	private IEnumerator SetupSetRoom()
	{
		//playerInstance.SetActive(true);

		//Show dungeon HUD
		uiManager.DisableAllUI();
		//uiManager.ResetAbilityUIValues();
		uiManager.SetUIActive(1, true);
		yield return new WaitForEndOfFrame();
	}

	IEnumerator GameOver()
	{
		PlayerControler playerScript = playerInstance.GetComponent<PlayerControler>();
		playerScript.Invulnerable = true;
		playerScript.isDying = true;
		uiManager.DisableAllUI();
		playerInstance.GetComponentInChildren<CameraToMouseFollow>().gameObject.transform.localPosition = Vector3.zero;
		playerScript.GameOverVFX(1);
		Time.timeScale = 0f;    //Hitstop
		yield return new WaitForSecondsRealtime(0.5f);

		playerScript.GameOverVFX(2);
		playerScript.enabled = false;
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
