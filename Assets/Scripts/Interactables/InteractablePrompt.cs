using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePrompt : MonoBehaviour
{
	[SerializeField] private GameObject buttonPrompt;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerControler>())
		{
			buttonPrompt.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerControler>())
		{
			buttonPrompt.SetActive(false);
		}
	}
}
