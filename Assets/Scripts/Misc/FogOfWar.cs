using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{

	[SerializeField] private GameObject[] mmIndicatorsToShow;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			foreach(GameObject mmIndicator in mmIndicatorsToShow)
			{
				mmIndicator.SetActive(true);
			}
		}
	}
}
