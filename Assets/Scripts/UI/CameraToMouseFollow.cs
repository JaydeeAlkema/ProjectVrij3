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
		if (!GameManager.Instance.IsPaused && GameManager.Instance.currentGameState != GameManager.GameState.GameOver)
		{
			Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
			Vector3 followDir = mousePos - player.transform.localPosition;

			//followPos.x = Mathf.Clamp(followPos.x, -threshold + player.position.x, threshold + player.position.x);
			//followPos.y = Mathf.Clamp(followPos.y, -threshold + player.position.y, threshold + player.position.y);

			this.transform.position = player.localPosition + Vector3.ClampMagnitude(followDir, threshold);
		}
	}
}
