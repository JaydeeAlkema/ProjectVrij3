using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private AK.Wwise.Event abilitySound;
	private PlayerControler player;
	System.Timers.Timer attackTimer = new System.Timers.Timer();

	public override void CallAbility( PlayerControler _player )
	{
		if( init )
		{
			init = false;
		}
		SetAbilityStats();
		player = _player;
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		AudioManager.Instance.PostEventLocal( abilitySound, player.gameObject );
		CastedObject = Object.Instantiate( castObject, CastFromPoint.transform.position + ( Vector3 )LookDir.normalized, CastFromPoint.rotation, CastFromPoint.transform );
		Projectile proj = CastedObject.GetComponent<Projectile>();
		//TrailUpgrade = BaseStats.TrailUpgrade;
		proj.BurnDamage = BurnDamage;
		Debug.Log( "the burn damage i give is: " + BurnDamage + ", but the burn damage proj has is: " + proj.BurnDamage );
		proj.TrailUpgrade = TrailUpgrade;
		proj.TurnOnTrail();
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		proj.CastedFrom = this;
		CastedObject.transform.SetParent( null );
	}

	void SetAbilityStats()
	{
		castObject = BaseStats.CastObject;
		lifeSpan = BaseStats.LifeSpan;
		force = BaseStats.Force;
		damage = BaseStats.Damage;
		AttackTime = BaseStats.AttackTime;
		abilitySound = BaseStats.AbilitySound1;
		BurnDamage = BaseStats.BurnDamage;
		statusEffects = BaseStats.statusEffects;
	}
}
