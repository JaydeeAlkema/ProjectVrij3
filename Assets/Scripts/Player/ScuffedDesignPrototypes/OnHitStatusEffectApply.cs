using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitStatusEffectApply : MonoBehaviour
{
	[SerializeField] private int burnDamage;
	[SerializeField] private float slowAmount;
	[SerializeField] private float slowDuration;

	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public int BurnDamage { get => burnDamage; set => burnDamage = value; }
	public float SlowAmount { get => slowAmount; set => slowAmount = value; }
	public float SlowDuration { get => slowDuration; set => slowDuration = value; }
	public int markType;    //0 = Melee, 1 = Cast

	public void Start()
	{
		UpdateStatusEffects();
	}

	public void UpdateStatusEffects()
	{
		statusEffects.Clear();
		switch( statusEffectType )
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				statusEffects.Add( new StatusEffect_Burning( burnDamage ) );
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				statusEffects.Add( new StatusEffect_Slow( slowAmount, slowDuration ) );
				break;
			case StatusEffectType.Marked:
				statusEffects.Add( new StatusEffect_Marked( markType ) );
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
