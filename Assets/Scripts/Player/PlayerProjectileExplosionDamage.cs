using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileExplosionDamage : MonoBehaviour
{
	[SerializeField] private IAbility castedFrom;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private float radius = 0.2f;
	[SerializeField] private int damage;
	private List<Collider2D> enemyList = new List<Collider2D>();
	private Collider2D[] enemiesInArea;
	private bool hitDetecting = false;
	

	public IAbility CastedFrom { get => castedFrom; set => castedFrom = value; }
	public int LayerMask { get => layerMask; set => layerMask = value; }
	public int Damage { get => damage; set => damage = value; }
	public float Radius { get => radius; set => radius = value; }

	private void Awake()
	{
		transform.localScale = new Vector3(radius, radius, radius);
	}

	void Update()
	{
		enemiesInArea = Physics2D.OverlapCircleAll(transform.position, radius * 6, layerMask);
		foreach (Collider2D enemy in enemiesInArea)
		{
			if (!enemyList.Contains(enemy))
			{
				enemyList.Add(enemy);
				DamageEnemiesInAoE(enemy);
			}
		}
	}

	void DamageEnemiesInAoE(Collider2D enemy)
	{
		if (enemyList != null)
		{
			Debug.Log("Damaged enemy with: " + damage);
			enemy.GetComponent<IDamageable>()?.TakeDamage(damage, 1);
			castedFrom.OnHitApplyStatusEffects(enemy.gameObject.GetComponent<IDamageable>());
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius * 6);
	}
}
