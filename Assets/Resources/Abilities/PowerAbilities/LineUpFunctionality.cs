using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpFunctionality : MonoBehaviour
{
	private Vector2 boxSize;
	private Vector2 lookDir;
	private float angle;
	private LayerMask layerMask;

	public Vector2 BoxSize { get => boxSize; set => boxSize =  value ; }
	public Vector2 LookDir { get => lookDir; set => lookDir =  value ; }
	public float Angle { get => angle; set => angle =  value ; }
	public LayerMask LayerMask { get => layerMask; set => layerMask =  value ; }


	// Start is called before the first frame update
	void Start()
    {
		Destroy( this.gameObject, 5f );
		LineUp();
		//animation event
    }

    public void LineUp()
    {
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll( transform.position, boxSize, angle, layerMask );
		Debug.Log( "Enemies: " + enemiesInBox.Length );
		Debug.DrawRay( transform.position, new Vector3( transform.localRotation.z ,0f, 0f ), Color.blue, 10f );
		foreach( Collider2D enemy in enemiesInBox )
		{
			Vector3 abNormal = new Vector3( transform.localRotation.z, 0f, 0f  );
			Vector3 enemyVec = enemy.transform.position - transform.position;

			float dotP = Vector2.Dot( enemyVec, abNormal );

			Vector3 newPoint = transform.position + ( abNormal * dotP );
			enemy.GetComponent<ICrowdControllable>()?.Pull( newPoint );
		}
	}

}
