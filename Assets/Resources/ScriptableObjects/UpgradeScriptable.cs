using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu( fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade" )]
public class UpgradeScriptable : ScriptableObject
{
	[SerializeField] private Sprite upgradeImageMelee;
	public Sprite UpgradeImageMelee { get => upgradeImageMelee; set => upgradeImageMelee = value; }
	[SerializeField] private Sprite upgradeImageRanged;
	public Sprite UpgradeImageRanged { get => upgradeImageRanged; set => upgradeImageRanged = value; }
	[SerializeField] private int damageUpgrade;
	public int DamageUpgrade { get => damageUpgrade; set => damageUpgrade = value; }
	[SerializeField] private float hitBoxUpgrade;
	public float HitBoxUpgrade { get => hitBoxUpgrade; set => hitBoxUpgrade = value; }
	[SerializeField] private float attackSpeedUpgrade;
	public float AttackSpeedUpgrade { get => attackSpeedUpgrade; set => attackSpeedUpgrade = value; }
	[SerializeField] private float critChanceUpgrade;
	public float CritChanceUpgrade { get => critChanceUpgrade; set => critChanceUpgrade = value; }
	[SerializeField] private float circleSizeUpgrade;
	public float CircleSizeUpgrade { get => circleSizeUpgrade; set => circleSizeUpgrade = value; }
	[SerializeField] private float distanceUpgrade;
	public float DistanceUpgrade { get => distanceUpgrade; set => distanceUpgrade = value; }

	[SerializeField, EnumFlags] private StatusEffectType statusEffect;
	public StatusEffectType StatusEffect { get => statusEffect; set => statusEffect = value; }
}
