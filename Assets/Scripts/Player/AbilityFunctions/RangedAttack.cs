using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	public override void AbilityBehavior()
	{
		GameObject projectile = Instantiate( castObject, castFromPoint.transform.position, castFromPoint.rotation, castFromPoint.transform );
		Projectile proj = projectile.GetComponent<Projectile>();
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		projectile.transform.SetParent( null );
	}

	public override void SetScriptable( AbilityScriptable scriptable )
	{
		abilityScriptable = scriptable;
		SetAbilityStats(scriptable.Rb2d, scriptable.CastFromPoint, scriptable.CastObject, scriptable.LifeSpan, scriptable.Force, scriptable.Damage);
	}

	void SetAbilityStats( Rigidbody2D AbilityRB2D, Transform CastFromPoint, GameObject CastObject, float LifeSpan, float Force, float Damage)
	{
		rb2d = AbilityRB2D;
		castObject = CastObject;
		castFromPoint = CastFromPoint;
		lifeSpan = LifeSpan;
		force = Force;
		damage = Damage;
	}
}
