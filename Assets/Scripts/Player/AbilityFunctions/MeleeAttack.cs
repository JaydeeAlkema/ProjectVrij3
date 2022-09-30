using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
	private bool onCoolDown = false;
	public override void AbilityBehavior()
	{
		if( !onCoolDown )
		{
			StartCoroutine( CoolDown() );
			Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( rb2d.transform.position + castFromPoint.transform.up * distance, boxSize, angle, layerMask );
			Debug.Log( "Enemies: " + enemiesInBox.Length );

			foreach( Collider2D enemy in enemiesInBox )
			{
				enemy.GetComponent<IDamageable>()?.TakeDamage( damage );
			}
		}
	}

	public void SetAbilityStats( Rigidbody2D AbilityRB2D, Transform CastFromPoint, Vector2 BoxSize, Vector2 LookDir, float Angle, LayerMask layer, float Damage, float Distance, float Cooldown )
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
	}

	public override void SetScriptable( AbilityScriptable scriptable )
	{
		abilityScriptable = scriptable;
		SetAbilityStats( scriptable.Rb2d, scriptable.CastFromPoint, scriptable.BoxSize, scriptable.LookDir, scriptable.Angle, scriptable.Layer, scriptable.Damage, scriptable.Distance, scriptable.CoolDown );
	}

	IEnumerator CoolDown()
	{
		onCoolDown = true;
		yield return new WaitForSeconds(coolDown);
		onCoolDown = false;
	}
}
