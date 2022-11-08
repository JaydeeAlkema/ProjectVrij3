using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderWindup : BTNode
{
	private float counter;
	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;
	private bool isSwooger;

	public FodderWindup(EnemyBase enemyScript, Rigidbody2D rb2d, bool isSwooger)
	{
		name = "FodderWindup";
		this.enemyScript = enemyScript;
		this.rb2d = rb2d;
		counter = 0f;
		this.isSwooger = isSwooger;
	}

	public override BTNodeState Evaluate()
	{
		object r = GetData("ready");
		if(r == null)
		{
			parent.parent.parent.SetData("ready", false);
			counter = 0f;
		}

		bool ready = (bool)GetData("ready");

		if (ready)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		else
		{
			if (counter >= enemyScript.WindUpTime)
			{
				parent.parent.parent.SetData("ready", true);
			}
			else
			{
				if (isSwooger)
				{
					if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SwoogerWindup"))
					{
						enemyScript.enemyAnimator.Play("SwoogerWindup");
					}
				}
				else
				{
					if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Windup"))
					{
						enemyScript.enemyAnimator.Play("Fodder1Windup");
					}
				}

				enemyScript.StopMovingToTarget();
				counter += Time.deltaTime;
				//fodderEnemyScript.coroutineText.text = "FodderWindup";
			}
			state = BTNodeState.RUNNING;
			return state;
		}
	}

}
