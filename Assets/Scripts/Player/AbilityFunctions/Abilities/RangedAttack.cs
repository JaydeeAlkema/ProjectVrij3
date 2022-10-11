using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private bool trailUpgrade = false;
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

	//public override void SetPlayerValues( AbilityScriptable scriptable )
	//{
	//	abilityScriptable = scriptable;
	//	SetAbilityStats(scriptable.Rb2d, scriptable.CastFromPoint, scriptable.CastObject, scriptable.LifeSpan, scriptable.Force, scriptable.Damage, scriptable.statusEffectType);
	//}

	void SetAbilityStats( Rigidbody2D AbilityRB2D, Transform CastFromPoint, GameObject CastObject, float LifeSpan, float Force, float Damage, StatusEffectType burntrail )
	{
		rb2d = AbilityRB2D;
		castObject = CastObject;
		castFromPoint = CastFromPoint;
		lifeSpan = LifeSpan;
		force = Force;
		damage = Damage;
		if( burntrail != StatusEffectType.Burntrail )
		{
			trailUpgrade = false;
		}
		else { trailUpgrade = true; }
	}
}
