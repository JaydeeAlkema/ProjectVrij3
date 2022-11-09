using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CastEndLag : BTNode
{
	private EnemyBase enemyScript;
	private float counter;

	public CastEndLag(EnemyBase enemyScript)
	{
		this.enemyScript = enemyScript;
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
				ClearData("inAttackSequence");
				ClearData("target");
				ClearData("dashDir");
				ClearData("dashDestination");
				state = BTNodeState.FAILURE;
				return state;
			}
			else
			{
				enemyScript.StopMovingToTarget();
				counter += Time.deltaTime;
			}
			state = BTNodeState.RUNNING;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}

}
