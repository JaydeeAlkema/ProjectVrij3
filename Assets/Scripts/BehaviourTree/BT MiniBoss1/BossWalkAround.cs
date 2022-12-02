using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossWalkAround : BTNode
{
	private Transform player;
	private BossBase bossScript;
	private Transform[] wayPoints;
	private int attackStep;
	private int currentWayPoint = 0;
	private float timer;
	private float walkDuration;
	private string animationName;

	public BossWalkAround(int attackStep, BossBase bossScript, float duration, string animationToPlay, Transform[] waypoints)
	{
		this.attackStep = attackStep;
		this.bossScript = bossScript;
		wayPoints = waypoints;
		walkDuration = duration;
		animationName = animationToPlay;
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

		if (!bossScript.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{
			bossScript.enemyAnimator.Play(animationName);
		}

		if (bossScript.GetComponent<Pathfinding.AIDestinationSetter>().target == null)
		{
			bossScript.MoveToTarget(player);
			//Debug.Log("Moving to waypoint: " + currentWayPoint);
			//bossScript.MoveToTarget(wayPoints[currentWayPoint]);
		}

		//if (Vector2.Distance(bossScript.gameObject.transform.position, bossScript.GetComponent<Pathfinding.AIDestinationSetter>().target.position) < 2f)
		//{
		//	bossScript.GetComponent<Pathfinding.AIDestinationSetter>().target = null;
		//	bossScript.StopMovingToTarget();
		//	//currentWayPoint = Random.Range(0, wayPoints.Length);
		//	//while (currentWayPoint == ignoreWaypoint)
		//	//{
		//	//	Debug.Log("Rolled into old waypoint, rerolling.");
		//	//	currentWayPoint = Random.Range(0, wayPoints.Length);
		//	//	break;
		//	//}
		//	//Debug.Log("New waypoint: " + currentWayPoint);
		//}

		timer += Time.deltaTime;
		Debug.Log("Walking with animation: " + animationName + " for " + walkDuration + " seconds.");
		state = BTNodeState.RUNNING;
		return state;
	}

}
