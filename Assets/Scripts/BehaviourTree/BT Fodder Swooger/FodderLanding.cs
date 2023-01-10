using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class FodderLanding : BTNode
{

	private EnemyBase enemyScript;
	private float counter;
	private int enemyType;

	public FodderLanding(EnemyBase enemyScript, int enemyType)
	{
		name = "FodderLanding";
		this.enemyScript = enemyScript;
		this.enemyType = enemyType;
		counter = 0f;
	}

	public override BTNodeState Evaluate()
	{

		bool ready = (bool)GetData("ready");

		if (ready)
		{
			if (counter >= enemyScript.EndLag)
			{
				counter = 0f;
				ClearData("ready");
				//("target");
				ClearData("dashDestination");
				ClearData("dashDir");
				ClearData("hitWall");
			}
			else
			{
				if (enemyType == 2)
				{
					if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SwoogerLanding"))
					{
						enemyScript.enemyAnimator.Play("SwoogerLanding");
					}
				}
				else if (enemyType == 1)
				{
					if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Landing"))
					{
						enemyScript.enemyAnimator.Play("Fodder1Landing");
					}
				}
				enemyScript.StopMovingToTarget();
				Transform target = (Transform)GetData("target");
				//enemyScript.enemySprite.flipX = (target.position - enemyScript.transform.position).normalized.x > 0 ? true : false;
				enemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				enemyScript.enemySprite.flipY = false;
				enemyScript.HasHitbox = false;
				counter += Time.deltaTime;
				//fodderEnemyScript.coroutineText.text = "FodderLanding";
			}
			state = BTNodeState.RUNNING;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}
}
