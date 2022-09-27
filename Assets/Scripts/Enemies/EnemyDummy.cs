using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : EnemyBase
{
	private void Start()
	{
		this.GetComponent<SpriteRenderer>().color = Color.green;
	}

	public override IEnumerator FlashColor()
	{
		yield return new WaitForSeconds( 0.2f );
		this.GetComponent<SpriteRenderer>().color = Color.green;
	}
}
