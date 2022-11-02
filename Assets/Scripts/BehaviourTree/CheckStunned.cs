using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CheckStunned : BTNode
{
    private EnemyBase enemyScript;

    public CheckStunned(EnemyBase getEnemyScript)
	{
		enemyScript = getEnemyScript;
	}

	public override BTNodeState Evaluate()
	{
		if (enemyScript.IsStunned)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		else
		{
			state = BTNodeState.FAILURE;
			return state;
		}
	}
}
