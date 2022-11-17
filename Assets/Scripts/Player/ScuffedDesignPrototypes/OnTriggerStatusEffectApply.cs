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
	Burntrail = 1 << 4,
}

public class OnTriggerStatusEffectApply : MonoBehaviour
{
	[SerializeField] private int burnDamage = 1;
	[SerializeField] private float slowAmount = 0.5f;
	[SerializeField] private float slowDuration = 4f;

	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public int BurnDamage { get => burnDamage; set => burnDamage = value; }
	public float SlowAmount { get => slowAmount; set => slowAmount = value; }
	public float SlowDuration { get => slowDuration; set => slowDuration = value; }
	public int markType;	//0 = Melee, 1 = Cast

	public void Start()
	{
		UpdateStatusEffects();
	}

	public void SetBurnValue(int _burnDamage)
	{
		burnDamage = _burnDamage;
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
