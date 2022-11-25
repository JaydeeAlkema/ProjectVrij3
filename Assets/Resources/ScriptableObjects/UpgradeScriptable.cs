using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu( fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade" )]
public class UpgradeScriptable : ScriptableObject
{
	[SerializeField] private Sprite upgradeImage;
	public Sprite UpgradeImage { get => upgradeImage; set => upgradeImage = value; }
	[SerializeField] private int damageUpgrade;
	public int DamageUpgrade { get => damageUpgrade; set => damageUpgrade = value; }
	[SerializeField] private float hitBoxUpgrade;
	public float HitBoxUpgrade { get => hitBoxUpgrade; set => hitBoxUpgrade = value; }
	[SerializeField] private float attackSpeedUpgrade;
	public float AttackSpeedUpgrade { get => attackSpeedUpgrade; set => attackSpeedUpgrade = value; }
	[SerializeField] private float critChanceUpgrade;
	public float CritChanceUpgrade { get => critChanceUpgrade; set => critChanceUpgrade = value; }

	[SerializeField, EnumFlags] private StatusEffectType statusEffect;
	public StatusEffectType StatusEffect { get => statusEffect; set => statusEffect = value; }
}
