using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable, ICrowdControllable
{
	[SerializeField] private float healthPoints = 0;
	[SerializeField] private float speed = 0;
	[SerializeField] private float aggroRange = 0;
	[SerializeField] private float attackRange = 0;
	[SerializeField] private bool attacking = false;
	[SerializeField] private Rigidbody2D rb2d;

	private Transform target = null;

	[SerializeField] private Pathfinding.AIDestinationSetter destinationSetter;
	[SerializeField] private Pathfinding.AIPath aiPath;

	private Vector2[] path;
	private int targetIndex = 0;

	private Vector2 pullPoint = new Vector2();
	private Vector2 vel = new Vector2();
	public bool beingCrowdControlled = false;
	public bool meleeTarget = false;
	public bool castTarget = false;
	public LayerMask layerMask = default;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public float Speed { get => speed; set => speed = value; }
	public float AggroRange { get => aggroRange; set => aggroRange = value; }
	public float AttackRange { get => attackRange; set => attackRange = value; }
	public bool Attacking { get => attacking; set => attacking = value; }
	public Rigidbody2D Rb2d { get => rb2d; set => rb2d = value; }
	public Transform Target { get => target; set => target = value; }

	public void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		destinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
		aiPath = GetComponent<Pathfinding.AIPath>();
	}

	public void Update()
	{
		foreach (IStatusEffect statusEffect in statusEffects.ToArray())
		{
			IDamageable damageable = GetComponent<IDamageable>();
			if (statusEffect != null)
			{
				statusEffect.Process(damageable);
			}
		}

		if (healthPoints <= 0) { Die(); }

		if (beingCrowdControlled)
		{
			beingDisplaced();
		}
		else
		{
			//GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
		}
	}

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{
		if (statusEffects.Contains(statusEffect)) return;
		statusEffects.Add(statusEffect);
	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{
		if (!statusEffects.Contains(statusEffect)) return;
		statusEffects.Remove(statusEffect);
	}

	public void TakeDamage(float damage)
	{
		healthPoints -= damage;
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		StartCoroutine(FlashColor());
		if (healthPoints <= 0) Die();
	}

	public void TakeDamage(float damage, int damageType)
	{
		if (damageType == 0 && meleeTarget)
		{
			healthPoints -= damage;
			meleeTarget = false;
		}
		if (damageType == 1 && castTarget)
		{
			healthPoints -= damage;
			castTarget = false;
		}
		healthPoints -= damage;
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		StartCoroutine(FlashColor());
		if (healthPoints <= 0) Die();
	}

	public void GetMarked(int markType)
	{
		switch (markType)
		{
			case 0:
				meleeTarget = true;
				break;
			case 1:
				castTarget = true;
				break;
		}
	}

	public virtual void GetSlowed(float slowAmount)
	{

	}

	public virtual void StartAttack(Transform target)
	{

	}

	public virtual void MoveToTarget(Transform target)
	{
		aiPath.enabled = true;
		destinationSetter.target = target;
	}

	public virtual void StopMovingToTarget()
	{
		aiPath.enabled = false;
		destinationSetter.target = null;
	}

	public virtual IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.08f);
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public virtual void Die()
	{
		Destroy(this.gameObject);
	}

	private void beingDisplaced()
	{
		if (Vector2.Distance(transform.position, pullPoint) > 0.1f)
		{
			GetComponent<Rigidbody2D>().MovePosition(Vector2.SmoothDamp(transform.position, pullPoint, ref vel, 8f * Time.deltaTime));

		}
		else
		{
			beingCrowdControlled = false;
			Physics.IgnoreLayerCollision(layerMask.value, layerMask.value, true);
		}
	}

	public void Pull(Vector2 pullPoint)
	{
		beingCrowdControlled = true;
		Physics.IgnoreLayerCollision(layerMask.value, layerMask.value, false);
		this.pullPoint = pullPoint;
	}

	public void OnDrawGizmos()
	{


		if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				//Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}
