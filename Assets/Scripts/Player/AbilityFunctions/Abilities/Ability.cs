using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : IAbility
{
	protected float coolDown;
	protected float damage;
	protected EdgeCollider2D shape;
	protected float distance;
	protected Vector2 boxSize;
	protected float circleSize;
	protected LayerMask layerMask;
	protected Camera cam;
	protected AbilityScriptable abilityScriptable;
	protected GameObject castObject;
	protected float lifeSpan = 10f;
	protected float force = 30f;
	protected AbilityScriptable baseStats;
	public AbilityScriptable BaseStats { get => baseStats; set => baseStats = value; }
	public float Angle { get => angle; set => angle = value; }
	public Vector2 LookDir { get => lookDir; set => lookDir = value; }
	public Transform CastFromPoint { get => castFromPoint; set => castFromPoint = value; }
	public PlayerControler player { get; set; }
	public Rigidbody2D rb2d { get; set; }
	public Vector3 mousePos { get; set; }
	public Vector2 lookDir { get; set; }
	public Transform castFromPoint { get; set; }
	public float angle { get; set; }
	public float CoolDown { get; set; }

	public virtual void CallAbility(PlayerControler _player) { }
	public virtual void AbilityBehavior(){ }
	public virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle ) 
	{
		rb2d = _rb2d;
		mousePos = _mousePos;
		lookDir = _lookDir;
		castFromPoint = _castFromPoint;
		angle = _angle;
	}

}
