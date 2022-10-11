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
		SetAbilityStats();
		if( burnAreaUpgrade )
		{
			new BurningMeleeDecorator( this, burningGround, rb2d, castFromPoint, distance );
			return;
		}
		else { AbilityBehavior(); }
	}
	public override void AbilityBehavior()
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( rb2d.transform.position + castFromPoint.transform.up * distance, boxSize, angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );

		foreach( Collider2D enemy in enemiesInBox )
		{
			enemy.GetComponent<IDamageable>()?.TakeDamage(damage, 0);
			baseStats.OnHitApplyStatusEffects( enemy.GetComponent<IDamageable>());
		}
	}

	public void SetAbilityStats()
	{
		rb2d = baseStats.Rb2d;
		castFromPoint = baseStats.CastFromPoint;
		boxSize = baseStats.BoxSize;
		angle = baseStats.Angle;
		layerMask = baseStats.Layer;
		lookDir = baseStats.LookDir;
		damage = baseStats.Damage;
		distance = baseStats.Distance;
		coolDown = baseStats.CoolDown;
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
}
