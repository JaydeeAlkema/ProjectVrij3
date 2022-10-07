using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitStatusEffectApply : MonoBehaviour
{
	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public float slowAmount;
	public float slowDuration;
	public int markType;    //0 = Melee, 1 = Cast

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
				statusEffects.Add(new StatusEffect_Slow(slowAmount, slowDuration));
				break;
			case StatusEffectType.Marked:
				statusEffects.Add(new StatusEffect_Marked(markType));
				break;
			default:
				break;
		}
	}

	public void OnHitApplyStatusEffects(IDamageable damageable)
	{
		foreach (IStatusEffect statusEffect in statusEffects)
		{
			if (statusEffect == null) return;
			damageable.ApplyStatusEffect(statusEffect);
		}
	}

}
