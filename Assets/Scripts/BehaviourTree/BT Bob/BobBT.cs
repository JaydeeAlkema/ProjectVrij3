using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BobBT : BTTree
{
	[SerializeField] private int enemyType = 3;
	public Rigidbody2D rb2d;
	public EnemyBase enemyScript;
	public float tooCloseRange = 3f;
	

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
				new BobCheckClose(rb2d, enemyScript, tooCloseRange),
				new BobRunAway(rb2d, enemyScript, tooCloseRange),
			}),
			new Sequence(new List<BTNode>
			{
				new CheckAttackRange(rb2d, enemyScript),
				new Sequence(new List<BTNode>
				{
					new RaycastToTarget(enemyScript),
					new CheckBehindWall(),
					new BobWindup(enemyScript, rb2d),
					new ShootEnemyProjectile(enemyScript),
					new CastEndLag(enemyScript),
				}),
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
