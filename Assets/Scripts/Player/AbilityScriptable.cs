using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class AbilityScriptable : ScriptableObject
{
	[SerializeField]
	private float coolDown = 0f;
	public float CoolDown { get => coolDown; set => coolDown = value; }
	[SerializeField]
	private float damage = 0f;
	public float Damage { get => damage; set => damage = value; }
	[SerializeField]
	private float distance = 0f;
	public float Distance { get => distance; set => distance = value; }
	[SerializeField]
	private Vector2 boxSize = new Vector2( 4, 6 );
	public Vector2 BoxSize { get => boxSize; set => boxSize = value; }
	[SerializeField]
	private float circleSize = 0f;
	public float CircleSize { get => circleSize; set => circleSize = value; }
	[SerializeField]
	private LayerMask layerMask;
	public LayerMask Layer { get => layerMask; set => layerMask = value; }

	private float angle = default;
	public float Angle { get => angle; set => angle = value; }
	[SerializeField]
	private Camera cam = default;
	public Camera Cam { get => cam; set => cam = value; }
	private Vector2 lookDir = default;
	public Vector2 LookDir { get => lookDir; set => lookDir = value; }
	[SerializeField]
	private Rigidbody2D rb2d = default;
	public Rigidbody2D Rb2d { get => rb2d; set => rb2d = value; }
	[SerializeField]
	private Transform castFromPoint = default;
	public Transform CastFromPoint { get => castFromPoint; set => castFromPoint = value; }
	private Vector3 mousePos = default;
	public Vector3 MousePos { get => mousePos; set => mousePos = value; }
	[SerializeField]
	private Ability ability;

	public Ability Ability { get => ability; set => ability =  value ; }


	private void OnEnable()
	{
		cam = Camera.main;
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
