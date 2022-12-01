using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Slow : IStatusEffect
{
	readonly float slowAmount;
	readonly float slowDuration;
	private float counter = 0f;
	private bool isSlowed = false;

	public StatusEffect_Slow(float getSlowAmount, float getSlowDuration)
	{
		slowAmount = getSlowAmount;
		slowDuration = getSlowDuration;
	}

	public void Process(IDamageable damageable)
	{
		if (counter >= slowDuration)
		{
			damageable.GetSlowed(5);
			damageable.RemoveStatusEffect(this);
		}
		else
		{
			damageable.GetSlowed(slowAmount);
			counter += Time.deltaTime;
		}
	}
}
