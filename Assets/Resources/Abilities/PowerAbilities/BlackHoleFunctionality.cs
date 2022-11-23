using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleFunctionality : MonoBehaviour
{
	private float circleRadius;
	private LayerMask layerMask;

	public float CircleRadius { get => circleRadius; set => circleRadius = value; }
	public LayerMask LayerMask { get => layerMask; set => layerMask =  value ; }

	private void Start()
	{
		//Destroy( transform.parent.gameObject, GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length );
		Destroy( transform.parent.gameObject, 5f );
		BlackHole();
		//animation event
	}

	public void BlackHole()
	{
		Collider2D[] enemiesInCircle = Physics2D.OverlapCircleAll( transform.position, circleRadius, layerMask );
		Debug.Log( "Enemies: " + enemiesInCircle.Length );

		foreach( Collider2D enemy in enemiesInCircle )
		{
			Vector3 newPoint = transform.position;
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}
}
