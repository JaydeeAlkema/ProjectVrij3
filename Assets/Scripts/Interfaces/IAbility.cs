using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
	AbilityScriptable BaseStats { get; set; }

	virtual void CallAbility() { }
	virtual void AbilityBehavior() { }
	virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle ) { }
}
