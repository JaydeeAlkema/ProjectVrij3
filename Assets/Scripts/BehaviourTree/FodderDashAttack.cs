using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderDashAttack : BTNode
{
	private Rigidbody2D rb2d;
	private FodderEnemy fodderEnemyScript;

	public FodderDashAttack(EnemyBase enemyScript, Rigidbody2D rb2d)
	{
		name = "FodderDashAttack";
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
		this.rb2d = rb2d;
	}

	public override BTNodeState Evaluate()
	{
		Vector2 dashDir = (Vector2)GetData("dashDir");



		Vector2 dashDestination = (Vector2)GetData("dashDestination");

		if (Vector2.Distance(fodderEnemyScript.transform.position, dashDestination) >= 0.1f)
		{
			if (!fodderEnemyScript.fodderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Attack"))
			{
				fodderEnemyScript.fodderAnimator.Play("Fodder1Attack");
				float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
				fodderEnemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
				fodderEnemyScript.enemySprite.flipX = false;
				fodderEnemyScript.hasHitbox = true;
			}

			rb2d.transform.position = Vector2.MoveTowards(rb2d.transform.position, dashDestination, fodderEnemyScript.DashSpeed * Time.deltaTime);
			//fodderEnemyScript.coroutineText.text = "FodderDashAttack";
			state = BTNodeState.RUNNING;
			return state;
		}
		else
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
	}

}
