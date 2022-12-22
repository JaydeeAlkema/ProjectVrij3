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
				Vector2 dir = playerCollider.transform.position - rb2d.transform.position;
				RaycastHit2D hit = Physics2D.Raycast(rb2d.transform.position, dir, dir.magnitude, enemyScript.UnwalkableDetection);
				//Debug.DrawRay(rb2d.transform.position, dir, Color.yellow, 0.1f);

				if (hit.point != Vector2.zero)
				{
					state = BTNodeState.FAILURE;
					//Debug.Log("TARGET NOT FOUND (RANGE: " + aggroRange + ")");
					return state;
				}

				//Debug.DrawRay(rb2d.transform.position, dir, Color.white, 1f);
				parent.parent.SetData("target", playerCollider.transform);
				enemyScript.Target = playerCollider.transform;
				if (!enemyScript.IsAggro)
				{
					enemyScript.IsAggro = true;
					GameManager.Instance.EnemyAggroCount(true);
				}
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
