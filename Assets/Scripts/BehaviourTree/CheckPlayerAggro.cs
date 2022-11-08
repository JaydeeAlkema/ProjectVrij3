using BehaviourTree;
using UnityEngine;

public class CheckPlayerAggro : BTNode
{
	public static int layerMask = 1 << 8;

	private Rigidbody2D rb2d;
	private EnemyBase enemyScript;

	private float aggroRange;

	public CheckPlayerAggro(Rigidbody2D getRb2d, EnemyBase getEnemyScript)
	{
		rb2d = getRb2d;
		enemyScript = getEnemyScript;
		aggroRange = enemyScript.AggroRange;
	}

	public override BTNodeState Evaluate()
	{
		object t = GetData("target");
		if (t == null)
		{
			if (enemyScript.IsAggro)
			{
				aggroRange = 1000;
			}

			Collider2D playerCollider = Physics2D.OverlapCircle(rb2d.transform.position, aggroRange, layerMask);

			if (playerCollider != null)
			{
				parent.parent.SetData("target", playerCollider.transform);
				enemyScript.Target = playerCollider.transform;
			}

			state = BTNodeState.FAILURE;
			//Debug.Log("TARGET NOT FOUND (RANGE: " + aggroRange + ")");
			return state;
		}

		state = BTNodeState.SUCCESS;
		//Debug.Log("TARGET ACQUIRED");
		return state;
	}

}
