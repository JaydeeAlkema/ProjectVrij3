using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour , IDamageable
{
    [SerializeField]
    private float Hp = 100;

	public void TakeDamage( float damage )
	{
        Hp -= damage;
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		StartCoroutine( FlashColor() );
	}

	IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.2f);
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
	}
}
