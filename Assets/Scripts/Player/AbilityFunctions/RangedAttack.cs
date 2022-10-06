using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private bool trailUpgrade = false;
	public override void AbilityBehavior()
	{
		GameObject projectile = Instantiate( castObject, castFromPoint.transform.position, castFromPoint.rotation, castFromPoint.transform );
		Projectile proj = projectile.GetComponent<Projectile>();
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		if(trailUpgrade) { proj.TrailUpgrade = true; }
		projectile.transform.SetParent( null );
	}

	public override void SetScriptable( AbilityScriptable scriptable )
	{
		abilityScriptable = scriptable;
		SetAbilityStats(scriptable.Rb2d, scriptable.CastFromPoint, scriptable.CastObject, scriptable.LifeSpan, scriptable.Force, scriptable.Damage, scriptable.statusEffectType);
	}

	void SetAbilityStats( Rigidbody2D AbilityRB2D, Transform CastFromPoint, GameObject CastObject, float LifeSpan, float Force, float Damage, StatusEffectType burntrail )
	{
		rb2d = AbilityRB2D;
		castObject = CastObject;
		castFromPoint = CastFromPoint;
		lifeSpan = LifeSpan;
		force = Force;
		damage = Damage;
		if( burntrail == StatusEffectType.Burntrail )
		{
			trailUpgrade = true;
		}
	}
}
