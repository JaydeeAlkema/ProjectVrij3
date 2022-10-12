using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private bool trailUpgrade = false;
	private bool init = true;

	public override void CallAbility(PlayerControler _player)
	{
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
	}
	public override void AbilityBehavior()
	{
		GameObject projectile = Object.Instantiate( castObject, CastFromPoint.transform.position, CastFromPoint.rotation, CastFromPoint.transform );
		Projectile proj = projectile.GetComponent<Projectile>();
		if(trailUpgrade) { proj.TrailUpgrade = true; Debug.Log( "trailupgrade on" ); }
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		projectile.transform.SetParent( null );
	}

	void SetAbilityStats()
	{
		castObject = BaseStats.CastObject;
		lifeSpan = BaseStats.LifeSpan;
		force = BaseStats.Force;
		damage = BaseStats.Damage;
	}
}
