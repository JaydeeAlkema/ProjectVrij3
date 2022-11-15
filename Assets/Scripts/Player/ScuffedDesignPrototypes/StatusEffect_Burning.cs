using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Burning : IStatusEffect
{
	private int damageNumber;
	private float interval = 0.5f;
	private float counter = 0f;

	public StatusEffect_Burning(int getDamage)
	{
		damageNumber = getDamage;
	}

	void Start()
	{
		counter = interval;
	}

	public void Process(IDamageable damageable)
	{
		counter += Time.deltaTime;
		if (counter >= interval)
		{
			damageable.TakeDamage(damageNumber);
			//Debug.Log($"Ouch! ({damageNumber} damage!)");
			counter = 0f;
		}
	}
}
