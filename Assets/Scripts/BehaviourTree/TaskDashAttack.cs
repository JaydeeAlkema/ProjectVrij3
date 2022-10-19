using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class TaskDashAttack : BTNode
{

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;

	public TaskDashAttack(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		rb2d = getRb2d;
		enemyScript = getEnemyScript;
	}

	public override BTNodeState Evaluate()
	{
		Transform target = (Transform)GetData("target");

		if (!enemyScript.Attacking)
		{
			enemyScript.StopMovingToTarget();
			enemyScript.StartAttack(target);
		}

		state = BTNodeState.RUNNING;
		return state;
	}

	
}
