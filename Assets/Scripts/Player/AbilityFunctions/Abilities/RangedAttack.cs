using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private bool trailUpgrade = false;

	public override void CallAbility(PlayerControler _player)
	{
		SetAbilityStats();
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		GameObject projectile = Object.Instantiate( castObject, castFromPoint.transform.position, castFromPoint.rotation, castFromPoint.transform );
		Projectile proj = projectile.GetComponent<Projectile>();
		if(trailUpgrade) { proj.TrailUpgrade = true; Debug.Log( "trailupgrade on" ); }
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		projectile.transform.SetParent( null );
	}

	void SetAbilityStats()
	{
		castObject = baseStats.CastObject;
		lifeSpan = baseStats.LifeSpan;
		force = baseStats.Force;
		damage = baseStats.Damage;
	}
}
