using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class BossStompAttack : BTNode
{

	private MiniBoss1 bossScript;
	private int attackStep;

	public BossStompAttack(int attackStep, MiniBoss1 bossScript)
	{
		this.bossScript = bossScript;
		this.attackStep = attackStep;
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
			state = BTNodeState.SUCCESS;
			return state;
		}

		bossScript.ShockWave();
		state = BTNodeState.SUCCESS;
		return state;
	}
}
