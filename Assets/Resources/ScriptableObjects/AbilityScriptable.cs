using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu( fileName = "Ability", menuName = "ScriptableObjects/Ability" )]
public class AbilityScriptable : ScriptableObject
{
	[SerializeField] private Sprite abilityImage;
	[SerializeField] private Ability ability;
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
	[SerializeField] private EffectType[] effects = new EffectType[3];
	[SerializeField] private bool trailUpgrade;
	[SerializeField] private float dashSpeed = 100f;
	[SerializeField] private float dashDuration = 0.2f;
	[SerializeField] private float attackTime = 200f;

	[SerializeField] private int burnDamage = 1;
	[SerializeField] private float slowAmount = 0.5f;
	[SerializeField] private float slowDuration = 4f;

	private Dictionary<StatusEffectType, bool> abilityUpgrades = new Dictionary<StatusEffectType, bool>();

	public Sprite AbilityImage { get => abilityImage; set => abilityImage = value; }
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
	public Ability Ability { get => ability; set => ability =  value ; }
	public EffectType[] Effects { get => effects; set => effects = value; }
	[SerializeField, EnumFlags] public StatusEffectType statusEffectType;
	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public int BurnDamage { get => burnDamage; set => burnDamage = value; }
	public float SlowAmount { get => slowAmount; set => slowAmount = value; }
	public float SlowDuration { get => slowDuration; set => slowDuration = value; }

	public Dictionary<StatusEffectType, bool> AbilityUpgrades { get => abilityUpgrades; set => abilityUpgrades = value; }
	public int markType;

	public void UpdateStatusEffects()
	{
		statusEffects.Clear();
		switch( statusEffectType )
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				statusEffects.Add( new StatusEffect_Burning(burnDamage) );
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				statusEffects.Add( new StatusEffect_Slow( slowAmount, slowDuration ) );
				break;
			case StatusEffectType.Marked:
				statusEffects.Add(new StatusEffect_Marked(markType));
				break;
			default:
				break;
		}
	}
}
