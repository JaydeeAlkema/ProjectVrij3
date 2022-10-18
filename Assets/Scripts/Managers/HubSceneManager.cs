using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneManager : MonoBehaviour
{
    public static HubSceneManager sceneManagerInstance { get; private set; }
	private string loadScene;

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
		SceneManager.UnloadSceneAsync( currentScene );
		SceneManager.LoadSceneAsync( sceneToLoad, LoadSceneMode.Additive ).completed += HubSceneManager_completed;
		//SceneManager.LoadSceneAsync( "Scene Manager" );
		//SceneManager.LoadSceneAsync("UITest");
	}

	private void HubSceneManager_completed( AsyncOperation obj )
	{
		SceneManager.SetActiveScene( SceneManager.GetSceneByName( loadScene ) );
	}

	public void StartFirstScenes()
	{
		SceneManager.LoadSceneAsync( "UITest", LoadSceneMode.Additive );
		SceneManager.LoadSceneAsync( "Hub Prototype", LoadSceneMode.Additive );
	}
}
