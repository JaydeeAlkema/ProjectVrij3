using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Burning : IStatusEffect
{
	private float interval = 0.5f;
	private float counter = 0f;

	void Start()
	{
		counter = interval;
	}

	public void Process(IDamageable damageable)
	{
		counter += Time.deltaTime;
		if (counter >= interval)
		{
			int damageNumber = 1;
			damageable.TakeDamage(damageNumber);
			Debug.Log($"Ouch! ({damageNumber} damage!)");
			counter = 0f;
		}
	}
}
