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
		Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		Vector3 followPos = (player.position + mousePos) / 2f;

		followPos.x = Mathf.Clamp(followPos.x, -threshold + player.position.x, threshold + player.position.x);
		followPos.y = Mathf.Clamp(followPos.y, -threshold + player.position.y, threshold + player.position.y);

		this.transform.position = followPos;
	}
}
