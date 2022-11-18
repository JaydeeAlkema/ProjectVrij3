using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderDashAttack : BTNode
{
	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;
	private int enemyType;

	public FodderDashAttack(EnemyBase enemyScript, Rigidbody2D rb2d, int enemyType)
	{
		name = "FodderDashAttack";
		this.enemyScript = enemyScript;
		this.rb2d = rb2d;
		this.enemyType = enemyType;
	}

	public override BTNodeState Evaluate()
	{
		Vector2 dashDir = (Vector2)GetData("dashDir");



		Vector2 dashDestination = (Vector2)GetData("dashDestination");

		if (Vector2.Distance(enemyScript.transform.position, dashDestination) >= 0.1f)
		{
			if (enemyType == 2)
			{
				if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SwoogerAttack"))
				{
					enemyScript.enemyAnimator.Play("SwoogerAttack");
					float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
					enemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
					enemyScript.enemySprite.flipX = false;
					enemyScript.enemySprite.flipY = (dashDestination - (Vector2)enemyScript.transform.position).normalized.x > 0 ? true : false;
					enemyScript.HasHitbox = true;
				}
			}
			else if(enemyType == 1)
			{
				if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Attack"))
				{
					enemyScript.enemyAnimator.Play("Fodder1Attack");
					float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
					enemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
					enemyScript.enemySprite.flipX = false;
					enemyScript.enemySprite.flipY = (dashDestination - (Vector2)enemyScript.transform.position).normalized.x > 0 ? true : false;
					enemyScript.HasHitbox = true;
				}
			}

			rb2d.transform.position = Vector2.MoveTowards(rb2d.transform.position, dashDestination, enemyScript.DashSpeed * Time.deltaTime);
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
