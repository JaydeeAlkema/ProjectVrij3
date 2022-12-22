using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

[CreateAssetMenu( fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade" )]
public class UpgradeScriptable : ScriptableObject
{
	[Header("Upgrade Info")]
	[SerializeField] private string upgradeName;
	[SerializeField] private Sprite upgradeImageMelee;
	[SerializeField] private Sprite upgradeImageRanged;
	[SerializeField] private string toolTipText;

	[Header("Upgrade Stats")]
	[SerializeField] private int damageUpgrade;
	[SerializeField] private float hitBoxUpgrade;
	[SerializeField] private float attackSpeedUpgrade;
	[SerializeField] private float critChanceUpgrade;
	[SerializeField] private float circleSizeUpgrade;
	[SerializeField] private float distanceUpgrade;
	[SerializeField, EnumFlags] private StatusEffectType statusEffect;
	public Sprite UpgradeImageMelee { get => upgradeImageMelee; set => upgradeImageMelee = value; }
	public Sprite UpgradeImageRanged { get => upgradeImageRanged; set => upgradeImageRanged = value; }
	public int DamageUpgrade { get => damageUpgrade; set => damageUpgrade = value; }
	public float HitBoxUpgrade { get => hitBoxUpgrade; set => hitBoxUpgrade = value; }
	public float AttackSpeedUpgrade { get => attackSpeedUpgrade; set => attackSpeedUpgrade = value; }
	public float CritChanceUpgrade { get => critChanceUpgrade; set => critChanceUpgrade = value; }
	public float CircleSizeUpgrade { get => circleSizeUpgrade; set => circleSizeUpgrade = value; }
	public float DistanceUpgrade { get => distanceUpgrade; set => distanceUpgrade = value; }
	public StatusEffectType StatusEffect { get => statusEffect; set => statusEffect = value; }
	public string ToolTipInfo { get => toolTipText; set => toolTipText = value; }
	public string UpgradeName { get => upgradeName; set => upgradeName = value; }
}
