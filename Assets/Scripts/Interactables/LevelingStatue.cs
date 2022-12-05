using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingStatue : MonoBehaviour
{
    private ExpManager expMan;
    private LevelManager levelMan;
	private bool canInteract;
	[SerializeField] private int healOnPointSpend;

	private void Awake()
	{
		expMan = GameManager.Instance.ExpManager;
		levelMan = LevelManager.LevelManagerInstance;
	}

	private void Update()
	{
		if( canInteract )
		{
			if( Input.GetKeyDown( KeyCode.F ) && expMan.PlayerPoints >= levelMan.PointToLevel )
			{
				expMan.PlayerPoints -= levelMan.PointToLevel;
				GameManager.Instance.PlayerHP.value += healOnPointSpend;
				Debug.Log( "level up, current points: " + expMan.PlayerPoints + " Points to level was: " + levelMan.PointToLevel );
				levelMan.IncreaseLevel();
			}
		}
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( collision.GetComponent<PlayerControler>() )
		{
			canInteract = true;
		}
	}

	private void OnTriggerExit2D( Collider2D collision )
	{
		if( collision.GetComponent<PlayerControler>() )
		{
			canInteract = false;
		}
	}
}
