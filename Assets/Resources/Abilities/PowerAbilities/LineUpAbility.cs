using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpAbility : Ability
{
	private GameObject lineUpObject;
	private Transform player;
	public override void CallAbility(PlayerControler _playerControler)
	{
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
		player = _playerControler.transform;
		AbilityBehavior();
	}
	
	public override void AbilityBehavior()
	{
		GameObject lineUp = Object.Instantiate( lineUpObject, player.position, CastFromPoint.rotation * Quaternion.Euler(0, 0, -90));
		LineUpFunctionality lU = lineUp.GetComponentInChildren<LineUpFunctionality>();
		lU.BoxSize = boxSize;
		lU.LookDir = LookDir;
		lU.Angle = Angle;
		lU.LayerMask = layerMask;
	}

	public void SetAbilityStats()
	{
		boxSize = BaseStats.BoxSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
		lineUpObject = baseStats.CastObject;
	}
}
