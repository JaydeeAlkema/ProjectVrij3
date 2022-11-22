using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileExplosionDamage : MonoBehaviour
{
	[SerializeField] private IAbility castedFrom;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private float radius = 0.2f;
	[SerializeField] private int damage;
	private Collider2D[] enemiesInArea;


	public IAbility CastedFrom { get => castedFrom; set => castedFrom = value; }
	public int LayerMask { get => layerMask; set => layerMask = value; }
	public int Damage { get => damage; set => damage = value; }
	public float Radius { get => radius; set => radius = value; }

	private void Awake()
	{
		transform.localScale = new Vector3(radius, radius, radius);
	}

	void Start()
	{
		enemiesInArea = Physics2D.OverlapCircleAll(transform.position, radius * 6, layerMask);
		foreach (Collider2D enemy in enemiesInArea)
		{
			int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
			enemy.GetComponent<IDamageable>()?.TakeDamage(damageToDeal, 1);
			castedFrom.OnHitApplyStatusEffects(enemy.gameObject.GetComponent<IDamageable>());
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius * 6);
	}
}
