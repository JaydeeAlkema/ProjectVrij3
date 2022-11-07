using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class RaycastToTarget : BTNode
{
	private FodderEnemy fodderEnemyScript;
	private Transform transform;

	public RaycastToTarget(EnemyBase enemyScript)
	{
		name = "RaycastToTarget";
		transform = enemyScript.gameObject.transform;
		fodderEnemyScript = enemyScript.GetComponent<FodderEnemy>();
	}

	public override BTNodeState Evaluate()
	{
		Transform target = (Transform)GetData("target");

		Vector2 dashDir = (target.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dashDir, fodderEnemyScript.DashDistance, fodderEnemyScript.UnwalkableDetection);
		Vector2 maxDistanceTarget = (Vector2)transform.position + dashDir * fodderEnemyScript.DashDistance;
		Debug.DrawRay(this.transform.position, dashDir * fodderEnemyScript.DashDistance, Color.red, 1f);

		if (hit.point != Vector2.zero)
		{
			parent.SetData("dashDestination", hit.point - dashDir / 10f);
			parent.SetData("dashDir", dashDir);
			state = BTNodeState.SUCCESS;
			return state;
		}
		else
		{
			parent.SetData("dashDestination", maxDistanceTarget);
			parent.SetData("dashDir", dashDir);
			state = BTNodeState.SUCCESS;
			return state;
		}
	}

}
