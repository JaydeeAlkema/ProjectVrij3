using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;


[CreateAssetMenu( fileName = "Ability", menuName = "ScriptableObjects/Ability" )]
public class AbilityScriptable : ScriptableObject
{
	#region Ability Info
	[Header( "Ability Info" )]
	[SerializeField] private string abilityName;
	[SerializeField] private Sprite abilityIcon;
	[SerializeField] private string toolTipText;
	#endregion
	#region basestats
	[Header("BaseStats")]
	[SerializeField] private float baseCooldown = 0.001f;
	[SerializeField] private int baseDamage = 0;
	[SerializeField] private int basePierce = 1;
	[SerializeField] private float baseCritChance = 0f;
	[SerializeField] private float baseDistance = 0f;
	[SerializeField] private float baseLifeSpan = 10f;
	[SerializeField] private float baseForce = 30f;
	[SerializeField] private Vector2 baseBoxSize = new Vector2( 4, 6 );
	[SerializeField] private float baseCircleSize = 0f;
	[SerializeField] private float baseDashSpeed = 100f;
	[SerializeField] private float baseDashDuration = 0.2f;
	[SerializeField] private float baseAttackTime = 200f;
	[SerializeField] private bool baseTrailUpgrade = false;
	[SerializeField] private int baseBurnDamage = 1;
	[SerializeField] private float baseSlowAmount = 0.5f;
	[SerializeField] private float baseSlowDuration = 4f;
	[SerializeField] private float baseMarkHits = 1;
	//[SerializeField] private List<IStatusEffect> baseStatusEffects = new List<IStatusEffect>();
	[SerializeField] private StatusEffectType baseStatusEffectType;
	#endregion
	[SerializeField] private float coolDown = 0.001f;
	[SerializeField] private int damage = 0;
	[SerializeField] private int pierce = 1;
	[SerializeField] private float critChance = 0f;
	[SerializeField] private float distance = 0f;
	[SerializeField] private Vector2 boxSize = new Vector2( 4, 6 );
	[SerializeField] private float circleSize = 0f;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private GameObject castObject;
	[SerializeField] private float lifeSpan = 10f;
	[SerializeField] private float force = 30f;
	[SerializeField] private GameObject burnObject;
	[SerializeField] private bool trailUpgrade;
	[SerializeField] private float dashSpeed = 100f;
	[SerializeField] private float dashDuration = 0.2f;
	[SerializeField] private float attackTime = 200f;
	[SerializeField] private float markHits = 1;

	[SerializeField] private int burnDamage = 1;
	[SerializeField] private float slowAmount = 0.5f;
	[SerializeField] private float slowDuration = 4f;

	[SerializeField] private AK.Wwise.Event abilitySound1;

	private Dictionary<StatusEffectType, bool> abilityUpgrades = new Dictionary<StatusEffectType, bool>();

	public float CoolDown { get => coolDown; set => coolDown = value; }
	public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
	public float CritChance { get => critChance; set => critChance = value; }
	public int Damage { get => damage; set => damage = value; }
	public float Force { get => force; set => force = value; }
	public float Distance { get => distance; set => distance = value; }
	public Vector2 BoxSize { get => boxSize; set => boxSize = value; }
	public float CircleSize { get => circleSize; set => circleSize = value; }
	public LayerMask Layer { get => layerMask; set => layerMask = value; }
	public GameObject CastObject { get => castObject; set => castObject = value; }
	public GameObject BurnObject { get => burnObject; set => burnObject = value; }
	public bool TrailUpgrade { get => trailUpgrade; set => trailUpgrade = value; }
	public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
	public float DashDuration { get => dashDuration; set => dashDuration = value; }
	public float AttackTime { get => attackTime; set => attackTime = value; }
	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;
	public List<IStatusEffect> statusEffects;

	public int BurnDamage { get => burnDamage; set => burnDamage = value; }
	public float SlowAmount { get => slowAmount; set => slowAmount = value; }
	public float SlowDuration { get => slowDuration; set => slowDuration = value; }
	public float MarkHits { get => markHits; set => markHits = value; }

	public Dictionary<StatusEffectType, bool> AbilityUpgrades { get => abilityUpgrades; set => abilityUpgrades = value; }
	public AK.Wwise.Event AbilitySound1 { get => abilitySound1; set => abilitySound1 = value; }
	public Sprite AbilityIcon { get => abilityIcon; set => abilityIcon = value; }
	public string ToolTipText { get => toolTipText; set => toolTipText = value; }
	public string AbilityName { get => abilityName; set => abilityName = value; }

	public int markType;

	public void UpdateStatusEffects()
	{
		if( statusEffects != null )
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
					statusEffects.Add( new StatusEffect_Marked( markType, markHits ) );
					break;
				default:
					break;
			}
		}
	}

	public void SetBaseStats()
	{
		coolDown = baseCooldown;
		damage = baseDamage;
		pierce = basePierce;
		critChance = baseCritChance;
		distance = baseDistance;
		lifeSpan = baseLifeSpan;
		force = baseForce;
		boxSize = baseBoxSize;
		circleSize = baseCircleSize;
		dashSpeed = baseDashSpeed;
		dashDuration = baseDashDuration;
		attackTime = baseAttackTime;
		trailUpgrade = baseTrailUpgrade;
		burnDamage = baseBurnDamage;
		slowAmount = baseSlowAmount;
		slowDuration = baseSlowDuration;
		markHits = baseMarkHits;
		statusEffects = new List<IStatusEffect>();
		statusEffectType = baseStatusEffectType;
	}

	public void SetHoldStats(AbilityScriptable stats)
	{
		coolDown = stats.coolDown;
		damage = stats.damage;
		pierce = stats.pierce;
		critChance = stats.critChance;
		distance = stats.distance;
		lifeSpan = stats.lifeSpan;
		force = stats.force;
		boxSize = stats.boxSize;
		circleSize = stats.circleSize;
		dashSpeed = stats.dashSpeed;
		dashDuration = stats.dashDuration;
		attackTime = stats.attackTime;
		trailUpgrade = stats.trailUpgrade;
		burnDamage = stats.burnDamage;
		slowAmount = stats.slowAmount;
		slowDuration = stats.slowDuration;
		markHits = stats.markHits;
		statusEffects = stats.statusEffects;
		statusEffectType = stats.statusEffectType;
	}
}
