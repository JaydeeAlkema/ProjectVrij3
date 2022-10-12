using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDecorator : IAbility
{
	private IAbility ability;

	protected AbilityScriptable baseStats;
	public AbilityScriptable BaseStats { get => baseStats; set => baseStats = value; }
	public PlayerControler player { get; set; }
	public Rigidbody2D rb2d { get; set; }
	public Vector3 mousePos { get; set; }
	public Vector2 lookDir { get; set; }
	public Transform castFromPoint { get; set; }
	public float angle { get; set; }
	public float CoolDown { get; set; }

	public AbilityDecorator(IAbility _ability)
	{
		ability = _ability;
	}

	public virtual void CallAbility(PlayerControler _player)
	{
		player = _player;
		ability.CallAbility(player);
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
