using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float Hp = 100;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public void Update()
	{
		foreach (IStatusEffect statusEffect in statusEffects)
		{
			IDamageable damageable = GetComponent<IDamageable>();
			statusEffect.Process(damageable);
		}
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
