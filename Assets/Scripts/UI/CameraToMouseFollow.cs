using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToMouseFollow : MonoBehaviour
{
	[SerializeField] Camera cam;
	[SerializeField] Transform player;
	[SerializeField] float threshold;


	void Update()
	{
		if (GameManager.Instance != null){
		if (!GameManager.Instance.IsPaused && GameManager.Instance.currentGameState != GameState.GameOver)
		{
			if (!GameManager.Instance.IsPaused && GameManager.Instance.currentGameState != GameManager.GameState.GameOver)
			{
				Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
				Vector3 followDir = mousePos - player.transform.localPosition;
				this.transform.position = player.localPosition + Vector3.ClampMagnitude(followDir, threshold);
			}
		}
		else
		{
			Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
			Vector3 followDir = mousePos - player.transform.localPosition;
			this.transform.position = player.localPosition + Vector3.ClampMagnitude(followDir, threshold);
		}
        }
	}
}
