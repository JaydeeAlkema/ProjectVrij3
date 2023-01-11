using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpFunctionality : MonoBehaviour
{
	[SerializeField] private Animator coilAnimator;

	private Vector2 boxSize;
	private Vector2 lookDir;
	private float angle;
	private LayerMask layerMask;
	private Vector3 abNormal;
	private Quaternion storedRotation;

	public Vector2 BoxSize { get => boxSize; set => boxSize = value; }
	public Vector2 LookDir { get => lookDir; set => lookDir = value; }
	public float Angle { get => angle; set => angle = value; }
	public LayerMask LayerMask { get => layerMask; set => layerMask = value; }


	// Start is called before the first frame update
	void Start()
	{
		storedRotation = transform.parent.rotation;
		coilAnimator.SetTrigger("StartCoiling");
	}

	public void LineUp()
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll(transform.position, boxSize, angle, layerMask);
		//Debug.Log("Enemies: " + enemiesInBox.Length);
		abNormal = transform.parent.rotation * Vector3.right;
		Debug.DrawRay(transform.position, abNormal, Color.blue, 10f);
		foreach (Collider2D enemy in enemiesInBox)
		{
			Vector3 enemyVec = enemy.transform.position - transform.position;

			float dotP = Vector2.Dot(enemyVec, abNormal);

			Vector3 newPoint = transform.position + (abNormal * dotP);
			enemy.GetComponent<ICrowdControllable>()?.Pull(newPoint);
		}
	}

	public void FlipSpriteX()
	{
		Vector2 flipDir = storedRotation * Vector3.right;
		GetComponent<SpriteRenderer>().flipX = flipDir.x < 0 ? true : false;
	}

	public void SetStoredRotationAndCheckFlipY()
	{
		transform.parent.rotation = storedRotation;
		Vector2 flipDir = storedRotation * Vector3.right;
		GetComponent<SpriteRenderer>().flipY = flipDir.x < 0 ? true : false;
	}

	public void ResetFlipSpriteX()
	{
		GetComponent<SpriteRenderer>().flipX = false;
	}	

	public void ResetFlipSpriteY()
	{
		GetComponent<SpriteRenderer>().flipY = false;
	}

	public void NoRotation()
	{
		transform.parent.rotation = Quaternion.identity;
	}

	public void DestroyAtEndOfAnimation()
	{
		Destroy(transform.parent.gameObject);
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.parent.rotation, boxSize);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		//Gizmos.DrawRay(transform.position, abNormal);
	}

}
