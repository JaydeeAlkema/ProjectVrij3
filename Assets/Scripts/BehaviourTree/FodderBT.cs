using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderBT : BTTree
{
	public Rigidbody2D rb2d;
	public EnemyBase enemyScript;
	public bool isSwooger = false;

	protected override BTNode SetupTree()
	{
		BTNode root = new Selector(new List<BTNode>
		{
			new Sequence(new List<BTNode>
			{
				new CheckStunned(enemyScript),
				new TaskStunned(enemyScript),
			}),
			new Sequence(new List<BTNode>
			{
				new CheckAttackRange(rb2d, enemyScript),
				new Sequence(new List<BTNode>
				{
					new RaycastToTarget(enemyScript),
					new FodderWindup(enemyScript, rb2d, isSwooger),
					new FodderDashAttack(enemyScript, rb2d, isSwooger),
					new FodderLanding(enemyScript, isSwooger),
					//new TaskDashAttack(rb2d, enemyScript),
				})
			}),
			new Sequence(new List<BTNode>
			{
				new CheckPlayerAggro(rb2d, enemyScript),
				new TaskMoveToPlayer(rb2d, enemyScript),
			}),
			new TaskIdle(rb2d, enemyScript),
		});

		return root;
	}


}
