using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
	private bool onCoolDown = false;
	public bool burnAreaUpgrade = false;
	public GameObject burningGround;

	public override void CallAbility()
	{
		if( burnAreaUpgrade )
		{
			new BurningMeleeDecorator( this, burningGround, rb2d, castFromPoint, distance );
			return;
		}
		else { AbilityBehavior(); }
	}
	public override void AbilityBehavior()
	{
		if( !onCoolDown )
		{
			
			Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( rb2d.transform.position + castFromPoint.transform.up * distance, boxSize, angle, layerMask );
			Debug.Log( "Enemies: " + enemiesInBox.Length );

			foreach( Collider2D enemy in enemiesInBox )
			{
				enemy.GetComponent<IDamageable>()?.TakeDamage(damage, 0);
				abilityScriptable.OnHitApplyStatusEffects( enemy.GetComponent<IDamageable>());
			}

		}
	}

	public void SetAbilityStats( Rigidbody2D AbilityRB2D, Transform CastFromPoint, Vector2 BoxSize, Vector2 LookDir, float Angle, LayerMask layer, float Damage, float Distance, float Cooldown, StatusEffectType burntrail )
	{
		rb2d = AbilityRB2D;
		castFromPoint = CastFromPoint;
		boxSize = BoxSize;
		angle = Angle;
		layerMask = layer;
		lookDir = LookDir;
		damage = Damage;
		distance = Distance;
		coolDown = Cooldown;
		if( burntrail == StatusEffectType.Burntrail )
		{
			burnAreaUpgrade = true;
		}
	}

	public override void SetScriptable( AbilityScriptable scriptable )
	{
		abilityScriptable = scriptable;
		SetAbilityStats( scriptable.Rb2d, scriptable.CastFromPoint, scriptable.BoxSize, scriptable.LookDir, scriptable.Angle, scriptable.Layer, scriptable.Damage, scriptable.Distance, scriptable.CoolDown, scriptable.statusEffectType );
	}
}
