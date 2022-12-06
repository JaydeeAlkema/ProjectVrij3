using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BobWindup : BTNode
{
	private float counter;
	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;

	public BobWindup(EnemyBase enemyScript, Rigidbody2D rb2d)
	{
		this.enemyScript = enemyScript;
		this.rb2d = rb2d;
		counter = 0f;
	}

	public override BTNodeState Evaluate()
	{
		object r = GetData("ready");
		if (r == null)
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
				object a = GetData("inAttackSequence");
				if(a == null)
				{
					bool inAttackSequence = true;
					parent.parent.parent.SetData("inAttackSequence", inAttackSequence);
				}

				if (!enemyScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("BobAttack"))
				{
					enemyScript.enemyAnimator.Play("BobAttack");
				}
				enemyScript.StopMovingToTarget();
				counter += Time.deltaTime;
			}
			state = BTNodeState.RUNNING;
			return state;
		}
	}
}
