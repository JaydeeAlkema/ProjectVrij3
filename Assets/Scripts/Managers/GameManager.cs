using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;

	[SerializeField] private ScriptableInt playerHP;
	[SerializeField] private int playerMaxHP;

	[Header("Managers")]
	[SerializeField] private LevelGeneratorV2 levelGenerator = null;
	[SerializeField] private HubSceneManager HubSceneManager = null;
	[SerializeField] private ExpManager expManager = null;
	[SerializeField] private UIManager uiManager = null;

	[Header("Player")]
	[SerializeField] private GameObject playerInstance = null;

	public static GameManager Instance { get => instance; private set => instance = value; }
	public ExpManager ExpManager { get => expManager; private set => expManager = value; }
	public UIManager UiManager { get => uiManager; private set => uiManager = value; }
	public int PlayerHP { get => playerHP.value; set => playerHP.value = value; }

	#region Unity Callbacks
	private void Awake()
	{
		if (!instance || instance != this)
		{
			instance = this;
		}

		if (FindObjectOfType<LevelGeneratorV2>() == null)
		{
			SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
			SceneManager.LoadSceneAsync("Jaydee Testing Scene", LoadSceneMode.Additive).completed += FetchDungeonReferences;
		}
		else
		{
			// Use the awake method for fetching references.
			levelGenerator = FindObjectOfType<LevelGeneratorV2>();
			HubSceneManager = FindObjectOfType<HubSceneManager>();
			expManager = FindObjectOfType<ExpManager>();
			uiManager = FindObjectOfType<UIManager>();
			playerInstance = FindObjectOfType<PlayerControler>().gameObject;

			playerInstance.SetActive(false);
			StartCoroutine(SetupLevel());
		}

		//else
		//{
		//	FindObjectOfType<HubSceneManager>().StartFirstScenes();
		//}

		GameManager.Instance.SetHP(playerMaxHP);

	}
	#endregion

	public void RemoveHP(int hp)
	{
		playerHP.value -= hp;
	}

	public void SetHP(int hp)
	{
		playerHP.value = hp;
	}

	private void FetchDungeonReferences(AsyncOperation asyncOperation)
	{
		if (FindObjectOfType<LevelGeneratorV2>() != null)
		{
			// Use the awake method for fetching references.
			levelGenerator = FindObjectOfType<LevelGeneratorV2>();
			HubSceneManager = FindObjectOfType<HubSceneManager>();
			playerInstance = FindObjectOfType<PlayerControler>().gameObject;

			playerInstance.SetActive(false);
		}
		StartCoroutine(SetupLevel());
	}

	/// <summary>
	/// Basic method that handles the setup of the level and player.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SetupLevel()
	{
		yield return StartCoroutine(levelGenerator.GenerateLevel());
		GameObject startingRoom = levelGenerator.Rooms[0].gameObject;
		Vector3 playerSpawnPos = new Vector3(startingRoom.transform.position.x, startingRoom.transform.position.y, 0);
		playerInstance.transform.position = playerSpawnPos;
		playerInstance.SetActive(true);
	}



}
