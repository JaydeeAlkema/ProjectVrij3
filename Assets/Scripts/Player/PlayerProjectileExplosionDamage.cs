using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileExplosionDamage : MonoBehaviour
{
	[SerializeField] private IAbility castedFrom;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private float radius;
	[SerializeField] private int damage;
	private Collider2D[] collidersInArea;
	private List<GameObject> enemiesInArea;

	public IAbility CastedFrom { get => castedFrom; set => castedFrom = value; }
	public int LayerMask { get => layerMask; set => layerMask = value; }
	public int Damage { get => damage; set => damage = value; }
	public float Radius { get => radius; set => radius = value; }

	private void Awake()
	{
		transform.localScale = new Vector3(radius, radius, radius) * 2;
	}

	void Start()
	{
		collidersInArea = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
		foreach(Collider2D collider2D in collidersInArea)
		{
			if (!enemiesInArea.Contains(collider2D.gameObject))
			{
				GameObject enemy = collider2D.gameObject;
				int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
				enemy.GetComponent<IDamageable>()?.TakeDamage(damageToDeal, 1);
				castedFrom.OnHitApplyStatusEffects(enemy.GetComponent<IDamageable>());
				enemiesInArea.Add(collider2D.gameObject); //Add enemy gameobject to list to prevent double hits on multiple colliders on the same object
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
