using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneManager : MonoBehaviour
{
    public static HubSceneManager sceneManagerInstance { get; private set; }

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
		SceneManager.LoadSceneAsync( sceneToLoad );
		SceneManager.LoadSceneAsync( "Scene Manager" );
		SceneManager.LoadSceneAsync("UITest");
		SceneManager.UnloadSceneAsync( currentScene );
	}
}
