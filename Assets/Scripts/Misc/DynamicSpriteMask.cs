using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpriteMask : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteToFollow;


	private void Update()
	{
		GetComponent<SpriteMask>().sprite = spriteToFollow.sprite;
	}

}
