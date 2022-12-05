using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossWalkAround : BTNode
{
	private Transform player;
	private BossBase bossScript;
	private int attackStep;
	private float timer;
	private float walkDuration;
	private List<string> animationNames;

	public BossWalkAround(int attackStep, BossBase bossScript, float duration, List<string> animationsToPlay)
	{
		this.attackStep = attackStep;
		this.bossScript = bossScript;
		walkDuration = duration;
		animationNames = animationsToPlay;
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
			bossScript.StopMovingToTarget();
			state = BTNodeState.SUCCESS;
			return state;
		}

		Debug.Log("PROCEED. Our step: " + attackStep + ", current step: " + currentAttackStep);

		//-----

		if (player == null)
		{
			player = GameObject.FindObjectOfType<PlayerControler>().transform;
		}

		if (timer >= walkDuration)
		{
			timer = 0f;
			bossScript.StopMovingToTarget();
			parent.SetData("currentAttackStep", currentAttackStep + 1);
			Debug.Log("DONE. Our step: " + attackStep + ", current step: " + (currentAttackStep + 1));
			state = BTNodeState.SUCCESS;
			return state;
		}

		if (!bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationNames[0]))
		{
			bossScript.enemyAnimator.Play(animationNames[0]);
		}

		if (bossScript.GetComponent<Pathfinding.AIDestinationSetter>().target == null)
		{
			bossScript.MoveToTarget(player);
			bossScript.enemyAnimator.Play(animationNames[1]);
		}

		timer += Time.deltaTime;
		state = BTNodeState.RUNNING;
		return state;
	}

}
