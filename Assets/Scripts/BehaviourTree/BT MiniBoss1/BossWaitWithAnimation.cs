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
	private string orbAnimation = null;
	private int attackStep;

	public BossWaitWithAnimation(int attackStep, BossBase bossScript, float duration, string animationToPlay)
	{
		this.bossScript = bossScript;
		waitDuration = duration;
		animationName = animationToPlay;
		this.attackStep = attackStep;
	}
	public BossWaitWithAnimation(int attackStep, BossBase bossScript, float duration, string animationToPlay, string orbAnimationToPlay)
	{
		this.bossScript = bossScript;
		waitDuration = duration;
		animationName = animationToPlay;
		this.attackStep = attackStep;
		orbAnimation = orbAnimationToPlay;
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
			Debug.Log("PASS. Our step: " + attackStep + ", current step: " + currentAttackStep);
			state = BTNodeState.SUCCESS;
			return state;
		}

		Debug.Log("PROCEED. Our step: " + attackStep + ", current step: " + currentAttackStep);

		//----

		if (timer >= waitDuration)
		{
			timer = 0f;
			if (orbAnimation != null)
			{
				bossScript.WeakspotAnimator.Play("Nothing");
			}
			parent.SetData("currentAttackStep", currentAttackStep + 1);
			Debug.Log("DONE. Our step: " + attackStep + ", current step: " + (currentAttackStep + 1));
			state = BTNodeState.SUCCESS;
			return state;
		}

		if (!bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{
			bossScript.enemyAnimator.Play(animationName);
			if (orbAnimation != null)
			{
				bossScript.WeakspotAnimator.Play(orbAnimation);
			}
		}

		timer += Time.deltaTime;
		Debug.Log("Waiting with animation: " + animationName + " for " + waitDuration + " seconds.");
		state = BTNodeState.RUNNING;
		return state;
	}


}
