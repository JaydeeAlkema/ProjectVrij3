using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
	AbilityScriptable BaseStats { get; set; }
	PlayerControler player { get; set; }
	public Rigidbody2D rb2d { get; set; }
	public Vector3 mousePos { get; set; }
	public Vector2 lookDir { get; set; }

	public float CoolDown { get; set; }
	public Transform castFromPoint { get; set; }
	public float angle { get; set; }

	virtual void CallAbility(PlayerControler _player) { }
	virtual void AbilityBehavior() { }
	virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle ) 
	{
		rb2d = _rb2d;
		mousePos = _mousePos;
		lookDir = _lookDir;
		castFromPoint = _castFromPoint;
		angle = _angle;
	}
}
