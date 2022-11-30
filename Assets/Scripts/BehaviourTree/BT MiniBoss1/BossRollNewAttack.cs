using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossRollNewAttack : BTNode
{
	private int amountOfBossAttacks;

	public BossRollNewAttack(int attacksAmount)
	{
		amountOfBossAttacks = attacksAmount;
	}

	public override BTNodeState Evaluate()
	{
		int currentAttack = Random.Range(1, amountOfBossAttacks + 1);
		parent.parent.SetData("currentAttackType", currentAttack);
		Debug.Log("New attack type: " + currentAttack);
		state = BTNodeState.SUCCESS;
		return state;
	}
}
