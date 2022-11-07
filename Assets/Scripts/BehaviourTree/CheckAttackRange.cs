using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class CheckAttackRange : BTNode
{
	private static int layerMask = 1 << 6;

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;
	private FodderEnemy fodderEnemyScript;

	private float attackRange;

	public CheckAttackRange(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		rb2d = getRb2d;
		enemyScript = getEnemyScript;
		attackRange = enemyScript.AttackRange;
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
	}

	public override BTNodeState Evaluate()
	{
		object t = GetData("target");
		if(t == null)
		{
			fodderEnemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			fodderEnemyScript.hasHitbox = false;
			ClearData("ready");
			ClearData("dashDestination");
			ClearData("dashDir");
			state = BTNodeState.FAILURE;
			return state;
		}

		Transform target = (Transform)t;

		if(Vector2.Distance(rb2d.position, target.position) <= attackRange)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		ClearData("ready");
		ClearData("dashDestination");
		ClearData("dashDir");
		fodderEnemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		fodderEnemyScript.hasHitbox = false;
		state = BTNodeState.FAILURE;
		return state;
	}

}
