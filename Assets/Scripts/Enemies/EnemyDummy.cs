using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour , IDamageable
{
    [SerializeField]
    private float Hp;

	public void TakeDamage( float damage )
	{
        Hp -= damage;
	}
}
