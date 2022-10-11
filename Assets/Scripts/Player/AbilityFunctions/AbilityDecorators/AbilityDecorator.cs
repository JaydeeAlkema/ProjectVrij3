using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDecorator : IAbility
{
	private IAbility ability;
	protected Rigidbody2D rb2d;
	protected Vector3 mousePos;
	protected Vector2 lookDir;
	protected Transform castFromPoint;
	protected float angle;
	protected AbilityScriptable baseStats;
	public AbilityScriptable BaseStats { get => baseStats; set => baseStats = value; }
	public AbilityDecorator(IAbility _ability)
	{
		ability = _ability;
	}

	public virtual void CallAbility()
	{
		ability.CallAbility();
	}

	public virtual void AbilityBehavior()
	{
		ability.AbilityBehavior();
	}

	public virtual void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle )
	{
		rb2d = _rb2d;
		mousePos = _mousePos;
		lookDir = _lookDir;
		castFromPoint = _castFromPoint;
		angle = _angle;
	}
}
