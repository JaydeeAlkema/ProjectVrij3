using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingStatue : MonoBehaviour
{
	private bool canInteract;

	private void Update()
	{
		if (canInteract && Input.GetKeyDown(KeyCode.F))
		{
			if (GameManager.Instance != null)
			{
				if (GameManager.Instance.IsPaused)
				{
					return;
				}
			}
			GameManager.Instance.UiManager.SetUIActive(6, true);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerControler>())
		{
			canInteract = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerControler>())
		{
			canInteract = false;
		}
	}
}
