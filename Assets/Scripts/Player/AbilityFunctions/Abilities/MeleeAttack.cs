using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
	private bool onCoolDown = false;
	public bool burnAreaUpgrade = false;
	public GameObject burningGround;

	public override void CallAbility(PlayerControler _player)
	{
		SetAbilityStats();
		//if( burnAreaUpgrade )
		//{
		//	new BurningMeleeDecorator( this, baseStats );
		//	return;
		//}
		//else { AbilityBehavior(); }
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
		boxSize = baseStats.BoxSize;
		layerMask = baseStats.Layer;
		damage = baseStats.Damage;
		distance = baseStats.Distance;
		coolDown = baseStats.CoolDown;
	}
}
