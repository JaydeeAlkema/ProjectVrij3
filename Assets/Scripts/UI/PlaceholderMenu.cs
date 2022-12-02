using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlaceholderMenu : MonoBehaviour
{
	public void MainMenu()
	{
		GameManager.Instance.ChangeGameState(GameState.Menu);
		Time.timeScale = 1f;
		SceneManager.LoadScene(0);
	}

	public void StartGame()
	{
		SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
		SceneManager.LoadScene("Jaydee Testing Scene", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("MainMenu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
