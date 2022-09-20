using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class AbilityScriptable : ScriptableObject
{
	[SerializeField]
	private bool damaging = false;
	[SerializeField]
	private bool moving = false;

	[SerializeField]
	private float coolDown = 0f;
	[SerializeField]
	private float damage = 0f;
	[SerializeField]
	private EdgeCollider2D shape;
	[SerializeField]
	private float distance = 0f;
	[SerializeField]
	private Vector2 boxSize = new Vector2( 4, 6 );
	[SerializeField]
	private float circleSize = 0f;
	[SerializeField]
	private LayerMask layerMask;

	private void DealDamage(GameObject target)
	{
		//target.hp -- damage;
	}

	private void Move(float dis, Vector3 dir, float spd)
	{
		//this.rb2d.velocity += movement;
	}

	public void LineUp(Rigidbody2D player, Transform castFromPoint, float angle, Vector2 lookDir)
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( player.transform.position + castFromPoint.transform.up * 3, boxSize, angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );

		foreach( Collider2D enemy in enemiesInBox )
		{
			Vector3 abNormal = lookDir.normalized;
			Vector3 enemyVec = enemy.transform.position - player.transform.position;

			float dotP = Vector2.Dot( enemyVec, abNormal );

			Vector3 newPoint = player.transform.position + ( abNormal * dotP );
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}

	public void BlackHole( Rigidbody2D player, Transform castFromPoint, float angle, Vector2 lookDir )
	{
		Vector2 circlePos = player.transform.position + castFromPoint.transform.up * 5;
		Collider2D[] enemiesInCircle = Physics2D.OverlapCircleAll( circlePos, circleSize, layerMask );
		Debug.Log( "Enemies: " + enemiesInCircle.Length );

		foreach( Collider2D enemy in enemiesInCircle )
		{
			Vector3 newPoint = circlePos;
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}

	}

}
