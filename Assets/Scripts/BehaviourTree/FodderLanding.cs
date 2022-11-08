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

		bool ready = (bool)GetData("ready");

		if (ready)
		{
			if (counter >= fodderEnemyScript.EndLag)
			{
				counter = 0f;
				ClearData("ready");
				ClearData("target");
				ClearData("dashDestination");
				ClearData("dashDir");
			}
			else
			{
				if (!fodderEnemyScript.fodderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fodder1Landing"))
				{
					fodderEnemyScript.fodderAnimator.Play("Fodder1Landing");
				}
				fodderEnemyScript.StopMovingToTarget();
				Transform target = (Transform)GetData("target");
				fodderEnemyScript.enemySprite.flipX = (target.position - fodderEnemyScript.transform.position).normalized.x > 0 ? true : false;
				fodderEnemyScript.enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				fodderEnemyScript.hasHitbox = false;
				counter += Time.deltaTime;
				Debug.Log(counter);
				//fodderEnemyScript.coroutineText.text = "FodderLanding";
			}
			state = BTNodeState.RUNNING;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}
}
