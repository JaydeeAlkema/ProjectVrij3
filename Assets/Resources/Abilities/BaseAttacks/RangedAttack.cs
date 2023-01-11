using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private AK.Wwise.Event abilitySound;
	private PlayerControler player;
	System.Timers.Timer attackTimer = new System.Timers.Timer();

	public override void CallAbility(PlayerControler _player)
	{
		if (init)
		{
			init = false;
		}
		SetAbilityStats();
		player = _player;
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		if( !Charging )
		{
			if( AudioManager.Instance != null )
			{
				AudioManager.Instance.PostEventLocal( abilitySound, player.gameObject );
			}
			player.CastParticles();
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
			AbilityController.AbilityControllerInstance.IsAttacking = false;
		}
		else if(Charging)
		{
			if( AudioManager.Instance != null )
			{
				AudioManager.Instance.PostEventLocal( abilitySound, player.gameObject );
			}
			player.CastParticles();
			CastedObject = Object.Instantiate( castObject, CastFromPoint.transform.position + ( Vector3 )LookDir.normalized, CastFromPoint.rotation, CastFromPoint.transform );
			Projectile proj = CastedObject.GetComponent<Projectile>();
			//TrailUpgrade = BaseStats.TrailUpgrade;
			proj.BurnDamage = BurnDamage;
			Debug.Log( "the burn damage i give is: " + BurnDamage + ", but the burn damage proj has is: " + proj.BurnDamage );
			proj.TrailUpgrade = TrailUpgrade;
			proj.TurnOnTrail();
			if(chargeTime > 100) 
			{ 
				chargeTime = 100; 
			}

			proj.Damage = damage + (int)chargeTime;
			proj.LifeSpan = lifeSpan + chargeTime;
			proj.Force = force + (chargeTime/2);
			proj.ChargeRadius = chargeTime / 50;
			proj.transform.localScale *= ( 1 + chargeTime / 100 );
			proj.CastedFrom = this;
			CastedObject.transform.SetParent( null );
			AbilityController.AbilityControllerInstance.IsAttacking = false;
		}
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
