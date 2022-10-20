using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAbility : Ability
{
	private bool init = true;
	public override void CallAbility( PlayerControler _playerControler )
	{
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
	}

	public override void AbilityBehavior()
	{
		Vector2 circlePos = Rb2d.transform.position + CastFromPoint.transform.up * 5;
		Collider2D[] enemiesInCircle = Physics2D.OverlapCircleAll( circlePos, circleSize, layerMask );
		Debug.Log( "Enemies: " + enemiesInCircle.Length );

		foreach( Collider2D enemy in enemiesInCircle )
		{
			Vector3 newPoint = circlePos;
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}

	public void SetAbilityStats()
	{
		circleSize = BaseStats.CircleSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
	}
}
