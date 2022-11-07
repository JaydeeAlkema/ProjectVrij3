using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderWindup : BTNode
{
	private float counter;
	private Rigidbody2D rb2d;
	private FodderEnemy fodderEnemyScript;

	public FodderWindup(EnemyBase enemyScript, Rigidbody2D rb2d)
	{
		name = "FodderWindup";
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
		this.rb2d = rb2d;
		counter = 0f;
	}

	public override BTNodeState Evaluate()
	{
		object r = GetData("ready");
		if(r == null)
		{
			parent.SetData("ready", false);
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
			if (counter >= fodderEnemyScript.WindUpTime)
			{
				parent.SetData("ready", true);
			}
			else
			{
				if (!fodderEnemyScript.fodderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Windup"))
				{
					fodderEnemyScript.fodderAnimator.Play("Fodder1Windup");
				}
				fodderEnemyScript.StopMovingToTarget();
				counter += Time.deltaTime;
				Debug.Log(counter);
				fodderEnemyScript.coroutineText.text = "FodderWindup";
			}
			state = BTNodeState.RUNNING;
			return state;
		}
	}

}
