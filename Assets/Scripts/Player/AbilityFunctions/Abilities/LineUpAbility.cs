using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpAbility : Ability
{
	public override void AbilityBehavior()
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( rb2d.transform.position + castFromPoint.transform.up * 3, boxSize, angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );

		foreach( Collider2D enemy in enemiesInBox )
		{
			Vector3 abNormal = lookDir.normalized;
			Vector3 enemyVec = enemy.transform.position - rb2d.transform.position;

			float dotP = Vector2.Dot( enemyVec, abNormal );

			Vector3 newPoint = rb2d.transform.position + ( abNormal * dotP );
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}

	public void SetAbilityStats(Rigidbody2D AbilityRB2D, Transform CastFromPoint, Vector2 BoxSize, Vector2 LookDir, float Angle, LayerMask layer)
	{
		rb2d = AbilityRB2D;
		castFromPoint = CastFromPoint;
		boxSize = BoxSize;
		angle = Angle;
		layerMask = layer;
		lookDir = LookDir;
	}
}
