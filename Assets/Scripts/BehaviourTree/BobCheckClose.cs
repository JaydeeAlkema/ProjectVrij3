using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BobCheckClose : BTNode
{
	public static int layerMask = 1 << 8;

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;
	private float tooCloseRange;
	private float aggroRange;

	public BobCheckClose(Rigidbody2D rb2d, EnemyBase enemyScript, float tooCloseRange)
	{
		this.rb2d = rb2d;
		this.enemyScript = enemyScript;
		this.tooCloseRange = tooCloseRange;
		aggroRange = enemyScript.AggroRange;
	}

	public override BTNodeState Evaluate()
	{
		object t = GetData("target");
		if (t == null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}

		Transform target = (Transform)t;

		if (Vector2.Distance(rb2d.position, target.position) <= tooCloseRange)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}

		object i = GetData("inAttackSequence");
		if (i != null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}
}
