using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossPlayAnimationUntilCompletion : BTNode
{
	private BossBase bossScript;
	private string animationName;
	private int attackStep;

	public BossPlayAnimationUntilCompletion(int attackStep, BossBase bossScript, string animationToPlay)
	{
		this.bossScript = bossScript;
		animationName = animationToPlay;
		this.attackStep = attackStep;
	}

	public override BTNodeState Evaluate()
	{
		//Pass if current attack step is no longer this attack step

		object c = GetData("currentAttackStep");

		if (c == null)
		{
			parent.parent.SetData("currentAttackStep", 0);
			state = BTNodeState.FAILURE;
			return state;
		}

		int currentAttackStep = (int)GetData("currentAttackStep");

		if (currentAttackStep != attackStep)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}

		if (!bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{
			bossScript.enemyAnimator.Play(animationName);
		}

		if (bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			Debug.Log("Done animating: " + animationName);
			parent.SetData("currentAttackStep", currentAttackStep + 1);
			state = BTNodeState.SUCCESS;
			return state;
		}

		state = BTNodeState.RUNNING;
		return state;
	}
}
