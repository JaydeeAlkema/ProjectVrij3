using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour ,IAbility
{
	protected float coolDown;
	protected int damage;
	protected EdgeCollider2D shape;
	protected float distance;
	protected Vector2 boxSize;
	protected float circleSize;
	protected LayerMask layerMask;
	protected Camera cam;
	protected AbilityScriptable abilityScriptable;
	protected GameObject castObject;
	protected float lifeSpan = 10f;
	protected float force = 30f;
	protected AbilityScriptable baseStats;
	protected float critChance;
	protected Dictionary<StatusEffectType, bool> abilityUpgrades = new Dictionary<StatusEffectType, bool>();
	public AbilityScriptable BaseStats { get => baseStats; set => baseStats = value; }
	public PlayerControler Player { get; set; }
	public Rigidbody2D Rb2d { get; set; }
	public Vector3 MousePos { get; set; }
	public Vector2 LookDir { get; set; }
	public Transform CastFromPoint { get; set; }
	public float Angle { get; set; }
	public float CoolDown { get => coolDown; set => coolDown = value; }
	public int Damage { get => damage; set => damage = value; }
	public float CritChance { get => critChance; set => critChance = value; }
	public Vector2 BoxSize { get => boxSize; set => boxSize = value; }
	public GameObject CastObject { get => castObject; set => castObject = value; }
	public Dictionary<StatusEffectType, bool> AbilityUpgrades { get => abilityUpgrades; set => abilityUpgrades = value; }
	public GameObject CastedObject { get; set; }
	public bool TrailUpgrade { get; set; }
	public int abilityID { get; set; }
	public AbilityController Controller { get; set; }
	public float AttackTime { get; set; }
	public int BurnDamage { get; set; }
	public float SlowAmount { get; set; }
	public float SlowDuration { get; set; }
	public CoroutineCaller caller { get; set; }

	public virtual void CallAbility(PlayerControler _player) { }
	public virtual void AbilityBehavior(){ }
	public virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle, bool _trailUpgrade ) 
	{
		Rb2d = _rb2d;
		MousePos = _mousePos;
		LookDir = _lookDir;
		CastFromPoint = _castFromPoint;
		Angle = _angle;
		TrailUpgrade = _trailUpgrade;
		caller = CoroutineCaller.CallerInstance;
	}

	public void OnHitApplyStatusEffects( IDamageable damageable )
	{
		foreach( IStatusEffect statusEffect in BaseStats.statusEffects )
		{
			if( statusEffect == null ) return;
			damageable.ApplyStatusEffect( statusEffect );
		}
	}
}
