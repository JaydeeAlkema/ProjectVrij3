using UnityEngine;

public class Portal : MonoBehaviour
{
	[SerializeField] private string sceneToLoadName = "";
	[SerializeField] private string currentSceneName = "";

	public string SceneToLoadName { get => sceneToLoadName; set => sceneToLoadName = value; }
	public string CurrentSceneName { get => currentSceneName; set => currentSceneName = value; }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.GetComponent<PlayerControler>())
		{
			Debug.Log("load new scene");
			HubSceneManager.sceneManagerInstance.ChangeScene(sceneToLoadName, currentSceneName);
		}
	}
}
