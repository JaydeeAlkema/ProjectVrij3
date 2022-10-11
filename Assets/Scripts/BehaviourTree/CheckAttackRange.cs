using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class CheckAttackRange : BTNode
{
	private static int layerMask = 1 << 6;

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;

	private float attackRange;

	public CheckAttackRange(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		rb2d = getRb2d;
		enemyScript = getEnemyScript;
		attackRange = enemyScript.AttackRange;
	}

	public override BTNodeState Evaluate()
	{
		object t = GetData("target");
		if(t == null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}

		Transform target = (Transform)t;
		if(Vector2.Distance(rb2d.position, target.position) <= attackRange)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}

		state = BTNodeState.FAILURE;
		return state;
	}

}
