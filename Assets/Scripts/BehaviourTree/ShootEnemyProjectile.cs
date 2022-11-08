using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class ShootEnemyProjectile : BTNode
{

	private BobEnemy bobScript;
	private EnemyBase enemyScript;

	public ShootEnemyProjectile(EnemyBase enemyScript)
	{
		this.enemyScript = enemyScript;
		if(enemyScript.GetComponent<BobEnemy>() != null)
		{
			bobScript = enemyScript.GetComponent<BobEnemy>();
		}
	}

	public override BTNodeState Evaluate()
	{
		Transform target = (Transform)GetData("target");

		if(target == null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}

		Vector2 projectileDirection = (target.position - enemyScript.transform.position).normalized;
		float angle = Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg - 90;
		GameObject castedProjectile = Object.Instantiate(bobScript.EnemyProjectile, enemyScript.transform.position, Quaternion.Euler(0f, 0f, angle));

		if(castedProjectile != null)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		state = BTNodeState.RUNNING;
		return state;
	}

}
