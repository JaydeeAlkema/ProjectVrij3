using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
	private bool init = true;
	public bool burnAreaUpgrade = false;
	public GameObject burningGround;
	System.Timers.Timer attackTimer = new System.Timers.Timer();

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
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( Rb2d.transform.position + CastFromPoint.transform.up * distance, boxSize, Angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );

		foreach( Collider2D enemy in enemiesInBox )
		{
			enemy.GetComponent<IDamageable>()?.TakeDamage(damage, 0);
			OnHitApplyStatusEffects( enemy.GetComponent<IDamageable>());
		}
	}

	public void SetAbilityStats()
	{
		boxSize = BaseStats.BoxSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
		AttackTime = BaseStats.AttackTime;
	}
}
