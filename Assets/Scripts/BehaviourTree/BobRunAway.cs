using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BobRunAway : BTNode
{
	public static int layerMask = 1 << 6;

	private Rigidbody2D rb2d;
	private BobEnemy bobScript;
	private float tooCloseRange;
	private float aggroRange;

	public BobRunAway(Rigidbody2D rb2d, EnemyBase enemyScript, float tooCloseRange)
	{
		this.rb2d = rb2d;
		bobScript = enemyScript.GetComponent<BobEnemy>();
		this.tooCloseRange = tooCloseRange;
		aggroRange = enemyScript.AggroRange;
	}

	public override BTNodeState Evaluate()
	{
		//Vector2 runToPoint = new Vector2(10000f, 1000f);

		//bobScript.RunAwayFromTarget(runToPoint);

		Collider2D nearestEnemy = Physics2D.OverlapCircle(rb2d.transform.position, 1000, layerMask);

		if (nearestEnemy != null)
		{
			bobScript.Target = nearestEnemy.transform;
			Debug.Log("Running Away!");
			state = BTNodeState.RUNNING;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}

}
