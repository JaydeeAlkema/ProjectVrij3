using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class TaskStunned : BTNode
{

	private EnemyBase enemyScript;
	private float counter = 0f;
	private float stunDuration = 0.1f;

	public TaskStunned(EnemyBase getEnemyScript)
	{
		enemyScript = getEnemyScript;
	}

	public override BTNodeState Evaluate()
	{

		if(counter <= stunDuration)
		{
			counter += Time.fixedDeltaTime;
			if (!enemyScript.Attacking)
			{
				enemyScript.StopMovingToTarget();
			}
		}
		else
		{
			counter = 0;
			enemyScript.IsStunned = false;
		}

		state = BTNodeState.RUNNING;
		return state;
	}

}
