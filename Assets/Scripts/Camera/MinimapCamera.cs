using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{

	void Start()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.UiManager.AssignRenderTextureToMinimapCamera(GetComponent<Camera>());
		}
		else if (GameManager.Instance == null)
		{
			this.enabled = false;
		}
	}

	void Update()
	{

	}
}
