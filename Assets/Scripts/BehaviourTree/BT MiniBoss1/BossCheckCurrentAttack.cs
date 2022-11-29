using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossCheckCurrentAttack : BTNode
{
	private int checkAttack;

	public BossCheckCurrentAttack(int checkForAttackNumber)
	{
		checkAttack = checkForAttackNumber;
	}

	public override BTNodeState Evaluate()
	{
		int currentAttack = (int)GetData("currentAttackType");
		if (currentAttack == checkAttack)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}
}
