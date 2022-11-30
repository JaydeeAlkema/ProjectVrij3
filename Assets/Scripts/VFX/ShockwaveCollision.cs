using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveCollision : MonoBehaviour
{
	[SerializeField] private int damage;

	public int Damage { get => damage; set => damage = value; }

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<IDamageable>() != null && collision.GetComponent<PlayerControler>() != null)
		{
			IDamageable damageable = collision.GetComponent<IDamageable>();
			int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
			damageable.TakeDamage(damageToDeal);
		}

		if (collision.GetComponent<CrackedGround>() != null)
		{
			collision.GetComponent<CrackedGround>().StartEruption();
		}
	}
}
