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
		object c = GetData("currentAttackType");

		if (c == null)
		{
			parent.parent.SetData("currentAttackType", 0);
			state = BTNodeState.FAILURE;
			return state;
		}

		int currentAttack = (int)GetData("currentAttackType");
		if (currentAttack == checkAttack)
		{
			Debug.Log("Checked current attack, doing attack " + checkAttack);
			state = BTNodeState.SUCCESS;
			return state;
		}
		Debug.Log("Attack " + currentAttack + " is NOT equal to Attack " + checkAttack + ", returning FAILURE.");
		state = BTNodeState.FAILURE;
		return state;
	}
}
