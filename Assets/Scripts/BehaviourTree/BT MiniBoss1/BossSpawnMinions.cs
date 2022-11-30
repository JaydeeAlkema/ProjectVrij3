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
		bossScript.SpawnMobs();
		Debug.Log("Spawned minions.");
		parent.SetData("currentAttackStep", currentAttackStep + 1);
		Debug.Log("DONE. Our step: " + attackStep + ", current step: " + (currentAttackStep + 1));
		state = BTNodeState.SUCCESS;
		return state;
	}

}
