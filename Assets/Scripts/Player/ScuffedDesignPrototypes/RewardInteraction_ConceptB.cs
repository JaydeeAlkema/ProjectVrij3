using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInteraction_ConceptB : MonoBehaviour
{
	public Transform buttonPrompt;
	public Transform ChooseUI;
	[SerializeField] private Transform chooseAbilityButton;
	public bool canInteract = false;

	private void Awake()
	{
		buttonPrompt.gameObject.SetActive(false);
		ChooseUI.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (canInteract)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				ChooseUI.gameObject.SetActive(true);
				GameManager.Instance.TogglePauseGame();
				if (GameManager.Instance != null)
				{
					GameManager.Instance.SetCursorImage(1);
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerControler>())
		{
			canInteract = true;
			buttonPrompt.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ( collision.GetComponent<PlayerControler>() )
		{
			canInteract = false;
			buttonPrompt.gameObject.SetActive(false);
			ChooseUI.gameObject.SetActive(false);
		}
	}

	public void CloseRewardScreen()
	{
		buttonPrompt.gameObject.SetActive( false );
		ChooseUI.gameObject.SetActive( false );
		chooseAbilityButton.gameObject.SetActive( false );
		GameManager.Instance.TogglePauseGame();
	}
}
