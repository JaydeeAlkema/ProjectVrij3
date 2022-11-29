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
		int currentAttack = Random.Range(0, amountOfBossAttacks);
		parent.parent.SetData("currentAttackType", currentAttack);
		state = BTNodeState.SUCCESS;
		return state;
	}
}
