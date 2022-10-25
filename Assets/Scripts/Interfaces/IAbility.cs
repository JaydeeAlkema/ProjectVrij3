using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
	AbilityScriptable BaseStats { get; set; }
	PlayerControler Player { get; set; }
	AbilityController Controller { get; set; }
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

	virtual void CallAbility(PlayerControler _player) { }
	virtual void AbilityBehavior() { }
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
		foreach( IStatusEffect statusEffect in BaseStats.statusEffects )
		{
			if( statusEffect == null ) return;
			damageable.ApplyStatusEffect( statusEffect );
		}
	}
}
