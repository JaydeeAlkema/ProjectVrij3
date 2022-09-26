using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardInteraction_ConceptA : MonoBehaviour
{
	public Transform popup;

	private void Awake()
	{
		popup.gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			popup.gameObject.SetActive(true);
		}
	}	
	
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			popup.gameObject.SetActive(false);
		}
	}
}
