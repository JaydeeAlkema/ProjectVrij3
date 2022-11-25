using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public interface IAbility
{
	AbilityScriptable BaseStats { get; set; }
	PlayerControler Player { get; set; }
	AbilityController Controller { get; set; }
	public IAbility ability { get; set; }
	public bool CooledDown { get; set; }
	public bool Init { get; set; }
	public Rigidbody2D Rb2d { get; set; }
	public Vector3 MousePos { get; set; }
	public Vector2 LookDir { get; set; }
	public Transform CastFromPoint { get; set; }
	public int abilityID { get; set; }
	public float Angle { get; set; }
	public float CoolDown { get; set; }
	public float AttackTime { get; set; }
	public int Damage { get; set; }
	public float CritChance { get; set; }
	public bool TrailUpgrade { get; set; }
	public Vector2 BoxSize { get; set; }
	public GameObject CastObject { get; set; }
	public GameObject CastedObject { get; set; }
	public Dictionary<StatusEffectType, bool> AbilityUpgrades { get; set; }
	public CoroutineCaller caller { get; set; }
	public int MarkType { get; set; }
	public int BurnDamage { get; set; }
	public float SlowAmount { get; set; }
	public float SlowDuration { get; set; }
	public StatusEffectType statusEffectType { get; set; }
	public List<IStatusEffect> statusEffects { get; set; }

	virtual void CallAbility(PlayerControler _player) { }
	virtual void CallAbility( bool resetCooldown ) { }
	virtual void AbilityBehavior() { }
	virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle) 
	{
		Rb2d = _rb2d;
		MousePos = _mousePos;
		LookDir = _lookDir;
		CastFromPoint = _castFromPoint;
		Angle = _angle;
	}

	virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle, bool _trailUpgrade )
	{
		Rb2d = _rb2d;
		MousePos = _mousePos;
		LookDir = _lookDir;
		CastFromPoint = _castFromPoint;
		Angle = _angle;
		TrailUpgrade = _trailUpgrade;
	}

	virtual void OnHitApplyStatusEffects( IDamageable damageable )
	{
		foreach( IStatusEffect statusEffect in statusEffects )
		{
			if( statusEffect == null ) return;
			damageable.ApplyStatusEffect( statusEffect );
		}
	}

	virtual void SetStartValues()
	{
		CoolDown = BaseStats.CoolDown;
		AttackTime = BaseStats.AttackTime;
		Damage = BaseStats.Damage;
		CritChance = BaseStats.CritChance;
		BoxSize = BaseStats.BoxSize;
		MarkType = BaseStats.markType;
		BurnDamage = BaseStats.BurnDamage;
		SlowAmount = BaseStats.SlowAmount;
		SlowDuration = BaseStats.SlowDuration;
		statusEffects = BaseStats.statusEffects;
		statusEffectType = BaseStats.statusEffectType;
	}

	public void UpdateStatusEffects()
	{
		statusEffects.Clear();
		switch( statusEffectType )
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				statusEffects.Add( new StatusEffect_Burning( BurnDamage ) );
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				statusEffects.Add( new StatusEffect_Slow( SlowAmount, SlowDuration ) );
				break;
			case StatusEffectType.Marked:
				statusEffects.Add( new StatusEffect_Marked( MarkType ) );
				break;
			default:
				break;
		}
	}
}
