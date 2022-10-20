using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpAbility : Ability
{
	private bool init = true;
	public override void CallAbility(PlayerControler _playerControler)
	{
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
	}
	
	public override void AbilityBehavior()
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( Rb2d.transform.position + CastFromPoint.transform.up * 3, boxSize, Angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );

		foreach( Collider2D enemy in enemiesInBox )
		{
			Vector3 abNormal = LookDir.normalized;
			Vector3 enemyVec = enemy.transform.position - Rb2d.transform.position;

			float dotP = Vector2.Dot( enemyVec, abNormal );

			Vector3 newPoint = Rb2d.transform.position + ( abNormal * dotP );
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}

	public void SetAbilityStats()
	{
		boxSize = BaseStats.BoxSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
	}
}
