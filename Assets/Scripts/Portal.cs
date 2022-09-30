using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
	[SerializeField]
	private string sceneToLoadName = "";
	[SerializeField]
	private string currentSceneName = "";
	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( collision.gameObject.GetComponent<PlayerControler>() )
		{
			Debug.Log( "load new scene" );
			HubSceneManager.sceneManagerInstance.ChangeScene( sceneToLoadName, currentSceneName );
		}
	}
}
