using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
	none = 0,
	Burn = 1 << 0,
	Stun = 1 << 1,
	Slow = 1 << 2,
	Marked = 1 << 3,
}

public class OnTriggerStatusEffectApply : MonoBehaviour
{
	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	private void Start()
	{
		switch (statusEffectType)
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				statusEffects.Add(new StatusEffect_Burning());
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				break;
			case StatusEffectType.Marked:
				break;
			default:
				break;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<IDamageable>() != null)
		{
			IDamageable damageable = collision.GetComponent<IDamageable>();
			foreach (IStatusEffect statusEffect in statusEffects)
			{
				if (statusEffect == null) return;
				damageable.ApplyStatusEffect(statusEffect);
			}
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<IDamageable>() != null)
		{
			IDamageable damageable = collision.GetComponent<IDamageable>();
			foreach (IStatusEffect statusEffect in statusEffects)
			{
				if (statusEffect != null)
				{
					damageable.RemoveStatusEffect(statusEffect);
				}
			}
		}
	}
}
