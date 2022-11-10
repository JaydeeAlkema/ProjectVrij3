using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class TaskMoveToPlayer : BTNode
{
	//private Rigidbody2D rb2d;
	private EnemyBase enemyScript;
	private float speed;


	public TaskMoveToPlayer(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		//rb2d = getRb2d;
		enemyScript = getEnemyScript;
		speed = enemyScript.Speed;
	}

	public override BTNodeState Evaluate()
	{
		Transform target = (Transform)GetData("target");
		if (target != null)
		{
			enemyScript.MoveToTarget(target);
			enemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			enemyScript.enemySprite.flipY = false;
			Debug.Log("Moving to target");
		}
		state = BTNodeState.RUNNING;
		return state;
	}


}
