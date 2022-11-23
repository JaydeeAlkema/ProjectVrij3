using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpAbility : Ability
{
	private bool init = true;
	private GameObject lineUpObject;
	public override void CallAbility(PlayerControler _playerControler)
	{
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
		AbilityBehavior();
	}
	
	public override void AbilityBehavior()
	{
		GameObject lineUp = Object.Instantiate( lineUpObject, MousePos, CastFromPoint.rotation);
		LineUpFunctionality lU = lineUp.GetComponent<LineUpFunctionality>();
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
