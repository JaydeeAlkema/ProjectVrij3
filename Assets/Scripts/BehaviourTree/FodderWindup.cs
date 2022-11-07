using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderWindup : BTNode
{
	private float counter;
	private FodderEnemy fodderEnemyScript;

	public FodderWindup(EnemyBase enemyScript)
	{
		name = "FodderWindup";
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
		counter = 0f;

	}

	public override BTNodeState Evaluate()
	{
		if (!fodderEnemyScript.fodderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Windup"))
		{
			fodderEnemyScript.fodderAnimator.Play("Fodder1Windup");
		}

		if (counter < fodderEnemyScript.WindUpTime)
		{
			counter += Time.deltaTime;
			state = BTNodeState.RUNNING;
			return state;
		}
		else
		{
			counter = 0f;
			state = BTNodeState.SUCCESS;
			return state;
		}
	}

}
