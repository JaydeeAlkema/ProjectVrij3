using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOffset : MonoBehaviour
{

    public Vector2 spriteOffset;
	private Vector2 basePosition;
	private int flipModifierX;
	private int flipModifierY;

	private void Start()
	{
		basePosition = transform.localPosition;
	}

	private void Update()
	{
		if (GetComponent<SpriteRenderer>().flipX)
		{
			flipModifierX = -1;
		}
		else
		{
			flipModifierX = 1;
		}		

		if (GetComponent<SpriteRenderer>().flipY)
		{
			flipModifierY = -1;
		}
		else
		{
			flipModifierY = 1;
		}

		transform.localPosition = basePosition + new Vector2(spriteOffset.x * flipModifierX, spriteOffset.y * flipModifierY);
	}

}
