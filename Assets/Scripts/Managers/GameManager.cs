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

	[Header("Managers")]
	[SerializeField] private LevelGeneratorV2 levelGenerator = null;
	[SerializeField] private HubSceneManager HubSceneManager = null;
	[SerializeField] private ExpManager expManager = null;
	[SerializeField] private UIManager uiManager = null;
	[SerializeField] private CheatsManager cheatsManager = null;

	[Header("Player")]
	[SerializeField] private GameObject playerInstance = null;

	public static GameManager Instance { get => instance; private set => instance = value; }
	public ExpManager ExpManager { get => expManager; private set => expManager = value; }
	public UIManager UiManager { get => uiManager; private set => uiManager = value; }
	public GameObject PlayerInstance { get => playerInstance; set => playerInstance = value; }
	public ScriptableInt PlayerHP { get => playerHP; set => playerHP = value; }
	public ScriptableFloat PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }
	public bool IsPaused { get => isPaused; private set => isPaused = value; }

	#region Unity Callbacks
	private void Awake()
	{
		if (!instance || instance != this)
		{
			instance = this;
		}

		QualitySettings.vSyncCount = 1;

		if (FindObjectOfType<LevelGeneratorV2>() == null)
		{
			SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
			SceneManager.LoadSceneAsync("Jaydee Testing Scene", LoadSceneMode.Additive);
		}
		else
		{
			FetchDungeonReferences();
		}
	}
	#endregion

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseGame();
			UiManager.SetUIActive(3, isPaused);
		}
	}

	public void TogglePauseGame()
	{
		isPaused = !isPaused;
		Time.timeScale = isPaused ? 0f : 1f;
	}

	public void FetchDungeonReferences()
	{
		if (FindObjectOfType<LevelGeneratorV2>() != null)
		{
			// Use the awake method for fetching references.
			levelGenerator = FindObjectOfType<LevelGeneratorV2>();
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

	/// <summary>
	/// Basic method that handles the setup of the level and player.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SetupLevel()
	{
		//Show loading screen
		uiManager.SetUIActive(2, true);

		yield return StartCoroutine(levelGenerator.GenerateLevel());
		GameObject startingRoom = levelGenerator.Rooms[0].gameObject;
		Vector3 playerSpawnPos = new Vector3(startingRoom.transform.position.x, startingRoom.transform.position.y, 0);
		playerInstance.transform.position = playerSpawnPos;
		playerInstance.SetActive(true);

		//Show dungeon HUD
		uiManager.DisableAllUI();
		uiManager.SetUIActive(1, true);
	}
}
