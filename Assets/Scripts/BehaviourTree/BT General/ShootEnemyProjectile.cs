using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class ShootEnemyProjectile : BTNode
{

	private BobEnemy bobScript;
	private EnemyBase enemyScript;
	private int projectileCounter = 0;
	private int maxAmountProjectiles = 7;
	private float attackCounter = 0;
	private float attackTime = 0.1f;

	public ShootEnemyProjectile(EnemyBase enemyScript)
	{
		this.enemyScript = enemyScript;
		if (enemyScript.GetComponent<BobEnemy>() != null)
		{
			bobScript = enemyScript.GetComponent<BobEnemy>();
		}
	}

	public override BTNodeState Evaluate()
	{
		object b = GetData("doneCasting");
		if(b != null)
		{
			bool doneAttacking = (bool)GetData("doneCasting");

			if (doneAttacking)
			{
				state = BTNodeState.SUCCESS;
				return state;
			}
		}
		else
		{
			parent.SetData("doneCasting", false);
		}



		Transform target = (Transform)GetData("target");

		if (target == null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}

		bool ready = (bool)GetData("ready");

		if (ready)
		{
			if (attackCounter < attackTime)
			{
				attackCounter += Time.deltaTime;
				state = BTNodeState.RUNNING;
				return state;
			}
			else
			{
				Vector2 projectileDirection = (target.position - enemyScript.transform.position).normalized;
				float angle = Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg - 90;
				GameObject castedProjectile = Object.Instantiate(bobScript.EnemyProjectile, enemyScript.transform.position, Quaternion.Euler(0f, 0f, angle));
				int damageToDeal = (int)(enemyScript.Damage * Random.Range(0.8f, 1.2f));
				castedProjectile.GetComponent<EnemyProjectile>().Damage = damageToDeal;
				projectileCounter++;
				attackCounter = 0f;

				if (projectileCounter < maxAmountProjectiles)
				{
					state = BTNodeState.RUNNING;
					return state;
				}
				else
				{
					projectileCounter = 0;
					parent.SetData("doneCasting", true);
					state = BTNodeState.SUCCESS;
					return state;
				}
			}
		}

		state = BTNodeState.RUNNING;
		return state;
		//Vector2 projectileDirection = (target.position - enemyScript.transform.position).normalized;
		//float angle = Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg - 90;
		//GameObject castedProjectile = Object.Instantiate(bobScript.EnemyProjectile, enemyScript.transform.position, Quaternion.Euler(0f, 0f, angle));
	}

}
