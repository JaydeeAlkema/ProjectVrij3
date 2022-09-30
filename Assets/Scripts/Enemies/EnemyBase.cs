using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float healthPoints = 0;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public void Update()
	{
		foreach (IStatusEffect statusEffect in statusEffects)
		{
			IDamageable damageable = GetComponent<IDamageable>();
			statusEffect.Process(damageable);
		}

		if( healthPoints <= 0 ) { Die(); }
	}

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{
		if (statusEffects.Contains(statusEffect)) return;
		statusEffects.Add(statusEffect);
	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{
		if (!statusEffects.Contains(statusEffect)) return;
		statusEffects.Remove(statusEffect);
	}

	public void TakeDamage(float damage)
	{
		healthPoints -= damage;
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		StartCoroutine( FlashColor() );
	}

	public virtual IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.2f);
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public virtual void Die()
	{
		
	}
}
