using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobEnemy : EnemyBase
{
	//Reward
	[SerializeField] private GameObject rewardInstance;
	[SerializeField] private Ability ability;
	[SerializeField] private AbilityScriptable abilityStats;

	//Enemy
	public LayerMask playerLayerMask;

	[SerializeField] private int damage;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject enemyProjectile;
	private float baseSpeed;

	[SerializeField] private GameObject runToObject = null;

	[SerializeField] private float hitDetectionRadius = 1f;
	public CapsuleCollider2D hurtbox;

	public AK.Wwise.Event Event;

	public GameObject EnemyProjectile { get => enemyProjectile; set => enemyProjectile = value; }

	void Awake()
	{
		base.Awake();
		hurtbox = this.GetComponent<CapsuleCollider2D>();

		baseSpeed = Speed;
	}

	void Update()
	{
		base.Update();
		if (Target != null)
		{
			HitBox();
		}
		LookAtTarget();
	}

	void HitBox()
	{
		if (Vector2.Distance(transform.position, Target.position) <= 1)
		{
			if (Target.gameObject != null && HasHitbox)
			{
				AttackPlayer(Target.gameObject);
				//HasHitbox = false;
			}
		}
		//Collider2D playerBody = Physics2D.OverlapCapsule((Vector2)this.transform.position, hurtbox.size, hurtbox.direction, playerLayerMask);
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>()?.TakeDamage(damage);
	}

	public override void MoveToTarget(Transform target)
	{
		base.MoveToTarget(target);
		enemyAnimator.Play("BobIdle");
	}

	public void RunAwayFromTarget(Vector2 runToPoint)
	{
		if (runToObject == null)
		{
			runToObject = new GameObject("RunObj");
			Instantiate(runToObject, runToPoint, Quaternion.identity);
		}
		MoveToTarget(runToObject.transform);
	}

	public override void GetSlowed(float slowAmount)
	{
		if (baseSpeed == Speed)
		{
			Speed *= slowAmount;
		}
		if (slowAmount >= 1)
		{
			Speed = baseSpeed;
		}
	}

	public override void Die()
	{
		GameManager.Instance.ExpManager.AddExp(ExpAmount);
		StopAllCoroutines();
		Time.timeScale = 1f;

		GameObject reward = Instantiate(rewardInstance, this.transform.position, Quaternion.identity);
		reward.GetComponent<RewardChoice>().AbilityStats = abilityStats;
		reward.GetComponent<RewardChoice>().AbilityToGive = ability;
		Debug.Log(reward.GetComponent<RewardChoice>().AbilityToGive);
		Destroy(this.gameObject);
	}

	public override IEnumerator FlashColor()
	{
		enemySprite.material = MaterialHit;
		yield return new WaitForSeconds(0.09f);
		enemySprite.material = MaterialDefault;
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, hitDetectionRadius);
	}
}
