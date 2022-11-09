using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class RaycastToTarget : BTNode
{
	private EnemyBase enemyScript;
	private Transform transform;

	public RaycastToTarget(EnemyBase enemyScript)
	{
		name = "RaycastToTarget";
		transform = enemyScript.gameObject.transform;
		this.enemyScript = enemyScript;
	}

	public override BTNodeState Evaluate()
	{
		Transform target = (Transform)GetData("target");

		Vector2 dashDir = (target.position - transform.position).normalized;


		RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDir, enemyScript.DashDistance, enemyScript.UnwalkableDetection);
		Vector2 maxDistanceTarget = (Vector2)transform.position + dashDir * enemyScript.DashDistance;

		object d1 = GetData("dashDestination");
		object d2 = GetData("dashDir");
		//object d3 = GetData("hitWallPoint");

		if(d1 == null || d2 == null)
		{
			if (hit.point != Vector2.zero)
			{
				//Store for caster-type enemies
				parent.parent.parent.SetData("hitWallPoint", hit.point);

				//Proceed
				Debug.DrawRay(this.transform.position, dashDir * enemyScript.DashDistance, Color.red, 1f);
				parent.parent.parent.SetData("dashDestination", hit.point - dashDir / 10f);
				parent.parent.parent.SetData("dashDir", dashDir);
				state = BTNodeState.FAILURE;
				return state;
			}
			else
			{
				ClearData("hitWallPoint");

				Debug.DrawRay(this.transform.position, dashDir * enemyScript.DashDistance, Color.red, 1f);
				parent.parent.parent.SetData("dashDestination", maxDistanceTarget);
				parent.parent.parent.SetData("dashDir", dashDir);
				state = BTNodeState.FAILURE;
				return state;
			}
		}
		state = BTNodeState.SUCCESS;
		return state;

	}

}
