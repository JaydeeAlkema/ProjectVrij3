using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAbility : Ability
{
	private bool init = true;
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
		GameObject blackHoleObject = Instantiate( blackHole, MousePos, Quaternion.identity );
		BlackHoleFunctionality bH = blackHoleObject.GetComponent<BlackHoleFunctionality>();
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
