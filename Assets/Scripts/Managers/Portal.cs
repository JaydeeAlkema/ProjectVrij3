using UnityEngine;

public class Portal : MonoBehaviour
{
	[SerializeField] private string sceneToLoadName = "";
	[SerializeField] private string currentSceneName = "";
	[SerializeField] private GameState state;

	private bool loadingNewScene = false;

	public string SceneToLoadName { get => sceneToLoadName; set => sceneToLoadName = value; }
	public string CurrentSceneName { get => currentSceneName; set => currentSceneName = value; }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!loadingNewScene)
		{
			if (collision.gameObject.GetComponent<PlayerControler>())
			{
				if (currentSceneName == "Jaydee Testing Scene" && GameManager.Instance.CurrentDungeonFloor < GameManager.Instance.MaxDungeonFloor)
				{
					sceneToLoadName = currentSceneName;
					GameManager.Instance.CurrentDungeonFloor++;
				}
				loadingNewScene = true;
				Debug.Log("load new scene");
				HubSceneManager.sceneManagerInstance.ChangeScene(sceneToLoadName, currentSceneName, state);
			}
		}
	}
}
