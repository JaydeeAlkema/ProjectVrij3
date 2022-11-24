using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAbility : Ability
{
	private GameObject blackHole;
	public override void CallAbility( PlayerControler _playerControler )
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
		GameObject blackHoleObject = Object.Instantiate( blackHole, new Vector3(MousePos.x, MousePos.y, 0f), Quaternion.identity );
		BlackHoleFunctionality bH = blackHoleObject.GetComponentInChildren<BlackHoleFunctionality>();
		bH.CircleRadius = circleSize;
		bH.LayerMask = layerMask;
	}

	public void SetAbilityStats()
	{
		circleSize = BaseStats.CircleSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
		blackHole = BaseStats.CastObject;
	}
}
