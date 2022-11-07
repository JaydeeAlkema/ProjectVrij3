using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class FodderLanding : BTNode
{

	private FodderEnemy fodderEnemyScript;
	private float counter;


	public FodderLanding(EnemyBase enemyScript)
	{
		name = "FodderLanding";
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
		counter = 0f;
	}

	public override BTNodeState Evaluate()
	{

		Transform target = (Transform)GetData("target");

		if (counter == 0f)
		{
			fodderEnemyScript.enemySprite.flipX = (target.position - fodderEnemyScript.transform.position).normalized.x > 0 ? true : false;
			fodderEnemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			fodderEnemyScript.fodderAnimator.Play("Fodder1Landing");
			fodderEnemyScript.hasHitbox = false;
		}

		if (counter < fodderEnemyScript.EndLag)
		{
			counter += Time.deltaTime;
			state = BTNodeState.RUNNING;
			return state;
		}
		else
		{
			fodderEnemyScript.Attacking = false;
			counter = 0f;
			state = BTNodeState.SUCCESS;
			return state;
		}
	}
}
