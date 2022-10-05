using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade" )]
public class UpgradeScriptable : ScriptableObject
{
	[SerializeField] private float damageUpgrade;
	public float DamageUpgrade { get => damageUpgrade; set => damageUpgrade = value; }
	[SerializeField] private float hitBoxUpgrade;
	public float HitBoxUpgrade { get => hitBoxUpgrade; set => hitBoxUpgrade = value; }
	[SerializeField] private float attackSpeedUpgrade;
	public float AttackSpeedUpgrade { get => attackSpeedUpgrade; set => attackSpeedUpgrade = value; }
	[SerializeField] private float critChanceUpgrade;
	public float CritChanceUpgrade { get => critChanceUpgrade; set => critChanceUpgrade = value; }

	[SerializeField] private EffectType effect;
	public EffectType Effect { get => effect; set => effect = value; }
}

public enum EffectType
{
	Null,
	FireArea,
	SlowHit,
	MarkHit
}
