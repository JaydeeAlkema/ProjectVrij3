using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossSpawnMinions : BTNode
{

	private BossBase bossScript;
	private int attackStep;

	public BossSpawnMinions(int attackStep, BossBase bossScript)
	{
		this.attackStep = attackStep;
		this.bossScript = bossScript;
	}

	public override BTNodeState Evaluate()
	{
		//Pass if current attack step is no longer this attack step
		int currentAttackStep = (int)GetData("currentAttackstep");

		if (currentAttackStep != attackStep)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}

		bossScript.SpawnMobs();
		parent.SetData("currentAttackStep", currentAttackStep + 1);

		state = BTNodeState.SUCCESS;
		return state;
	}

}
