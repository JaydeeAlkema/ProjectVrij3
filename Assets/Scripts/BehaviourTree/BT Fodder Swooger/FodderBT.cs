using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderBT : BTTree
{
	[SerializeField] private int enemyType;
	public Rigidbody2D rb2d;
	public EnemyBase enemyScript;

	protected override BTNode SetupTree()
	{
		BTNode root = new Selector(new List<BTNode>
		{
			new Sequence(new List<BTNode>
			{
				new GetCrowdControlled(enemyScript),
			}),
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
					new CheckBehindWall(),
					new FodderWindup(enemyScript, rb2d, enemyType),
					new FodderDashAttack(enemyScript, rb2d, enemyType),
					new FodderLanding(enemyScript, enemyType),
					//new TaskDashAttack(rb2d, enemyScript),
				})
			}),
			new Sequence(new List<BTNode>
			{
				new CheckPlayerAggro(rb2d, enemyScript),
				new TaskMoveToPlayer(rb2d, enemyScript),
			}),
			new TaskIdle(rb2d, enemyScript, enemyType),
		});

		return root;
	}


}
