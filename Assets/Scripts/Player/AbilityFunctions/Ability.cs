using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	protected float coolDown;
	protected float damage;
	protected EdgeCollider2D shape;
	protected float distance;
	protected Vector2 boxSize;
	protected float circleSize;
	protected LayerMask layerMask;
	protected Vector3 mousePos;
	protected float angle;
	protected Camera cam;
	protected Vector2 lookDir;
	protected Rigidbody2D rb2d;
	protected Transform castFromPoint;

	public abstract void AbilityBehavior();
}
