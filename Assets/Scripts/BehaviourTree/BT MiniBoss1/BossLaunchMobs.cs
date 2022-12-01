using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossLaunchMobs : BTNode
{
	private int attackStep;
	private MiniBoss1 miniBoss1Script;

	public BossLaunchMobs(int attackStep, BossBase bossScript)
	{
		this.attackStep = attackStep;
		miniBoss1Script = (MiniBoss1)bossScript;
	}

	public override BTNodeState Evaluate()
	{
		//Pass if current attack step is no longer this attack step

		object c = GetData("currentAttackStep");

		if (c == null)
		{
			parent.parent.SetData("currentAttackStep", 0);
			state = BTNodeState.FAILURE;
			return state;
		}

		int currentAttackStep = (int)GetData("currentAttackStep");

		if (currentAttackStep != attackStep)
		{
			Debug.Log("PASS. Our step: " + attackStep + ", current step: " + currentAttackStep);
			state = BTNodeState.SUCCESS;
			return state;
		}

		Debug.Log("PROCEED. Our step: " + attackStep + ", current step: " + currentAttackStep);

		//----

		miniBoss1Script.StartCoroutine(miniBoss1Script.LaunchMobs());
		parent.SetData("currentAttackStep", currentAttackStep + 1);
		state = BTNodeState.SUCCESS;
		return state;
	}
}
