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

		//if (enemyScript.IsStunned)
		//{
		enemyScript.StopMovingToTarget();
		ClearData("target");
		enemyScript.StartCoroutine("Stunned");
		
		//}
		//else
		//{
		//	state = BTNodeState.FAILURE;
		//	return state;
		//}

		state = BTNodeState.RUNNING;
		return state;
	}

}
