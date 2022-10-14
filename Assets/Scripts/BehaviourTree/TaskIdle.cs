using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class TaskIdle : BTNode
{
	//public float walkSpeed = 100f;

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;

	private float speed;
	private float waitTime = 2f;
	private float moveTime = 1f;
	private float waitCounter = 0f;
	private float moveCounter = 0f;
	private bool waiting = false;
	private bool moving = false;
	public Vector2 moveDir;

	public TaskIdle(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		rb2d = getRb2d;
		enemyScript = getEnemyScript;
		speed = enemyScript.Speed;
	}

	public override BTNodeState Evaluate()
	{
		if (waiting)
		{
			rb2d.velocity = new Vector2(0, 0);
			waitCounter += Time.deltaTime;
			if (waitCounter >= waitTime)
			{
				waiting = false;
				moveCounter = 0f;
				moving = true;
				moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			}
		}
		else
		{
			if (moving)
			{
				rb2d.velocity = moveDir.normalized * speed * Time.deltaTime;

				moveCounter += Time.deltaTime;
				if (moveCounter >= moveTime)
				{
					moving = false;
					waitCounter = 0f;
					waiting = true;
				}
			}
			else
			{
				waiting = true;
			}

		}

		state = BTNodeState.RUNNING;
		return state;
	}
}
