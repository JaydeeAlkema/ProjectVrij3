using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossWaitWithAnimation : BTNode
{
	private BossBase bossScript;
	private float waitDuration;
	private float timer = 0f;
	private string animationName;
	private int attackStep;

	public BossWaitWithAnimation(int attackStep, BossBase bossScript, float duration, string animationToPlay)
	{
		this.bossScript = bossScript;
		waitDuration = duration;
		animationName = animationToPlay;
		this.attackStep = attackStep;
	}

	public override BTNodeState Evaluate()
	{
		//Pass if current attack step is no longer this attack step
		int currentAttackStep = (int)GetData("currentAttackstep");

		if (currentAttackStep != attackStep)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}

		if (timer >= waitDuration)
		{
			timer = 0f;
			parent.SetData("currentAttackStep", currentAttackStep + 1);
			state = BTNodeState.SUCCESS;
			return state;
		}

		if (!bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{
			bossScript.enemyAnimator.Play(animationName);
		}

		timer += Time.deltaTime;
		state = BTNodeState.RUNNING;
		return state;
	}


}
