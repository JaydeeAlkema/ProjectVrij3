using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwoogerEnemy : EnemyBase
{
	//Reward
	[SerializeField] private GameObject rewardInstance;
	[SerializeField] private Ability ability;
	[SerializeField] private AbilityScriptable abilityStats;

	//Enemy
	public LayerMask playerLayerMask;
	[SerializeField] private LayerMask unwalkableDetection;

	public Animator swoogerAnimator;

	[SerializeField] private int damage;
	[SerializeField] private GameObject player;
	private float baseSpeed;

	[SerializeField] private float hitDetectionRadius = 1f;
	[SerializeField] private float windUpTime = 0.3f;
	[SerializeField] private float dashSpeed = 9;
	[SerializeField] private float dashDistance = 0.5f;
	[SerializeField] private float endLag = 0.8f;
	public bool hasHitbox = false;
	public CapsuleCollider2D hurtbox;

	public AK.Wwise.Event Event;

	void Awake()
	{
		//player = FindObjectOfType<PlayerControler>().gameObject;
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
			if (Target.gameObject != null && hasHitbox)
			{
				AttackPlayer(Target.gameObject);
				hasHitbox = false;
			}
		}
		//Collider2D playerBody = Physics2D.OverlapCapsule((Vector2)this.transform.position, hurtbox.size, hurtbox.direction, playerLayerMask);
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>()?.TakeDamage(damage);
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//	if (collision.gameObject.layer == 8)
	//	{
	//		AttackPlayer(collision.gameObject);
	//	}
	//}

	//public override void TakeDamage(int damage, int damageType)
	//{
	//	if (damageType == 0 && meleeTarget)
	//	{
	//		damage *= 2;
	//		meleeTarget = false;
	//	}
	//	if (damageType == 1 && castTarget)
	//	{
	//		damage *= 2;
	//		castTarget = false;
	//	}

	//	if (damageType == 0) //On melee hit
	//	{
	//		//AudioManager.PostEvent( , this.gameObject);   dit aanpassen als we wwise hebben enzo
	//		AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
	//		StartCoroutine(HitStop());
	//	}

	//	if (damageType == 1) //On cast hit
	//	{
	//		AkSoundEngine.PostEvent("npc_dmg_cast", this.gameObject);
	//	}

	//	DamagePopup(damage);
	//	HealthPoints -= damage;

	//	StartCoroutine(FlashColor());
	//	if (HealthPoints <= 0) Die();
	//}

	public override void MoveToTarget(Transform target)
	{
		base.MoveToTarget(target);
		swoogerAnimator.Play("SwoogerWalk");
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

	public override void StartAttack(Transform target)
	{
		//StopCoroutine(FollowPath());
		StartCoroutine(DashAttack(target));
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

	public IEnumerator DashAttack(Transform target)
	{
		//Windup starts
		Attacking = true;
		swoogerAnimator.Play("SwoogerWindup");
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Rb2d.velocity = new Vector2(0, 0);
		Vector2 dashDir = (target.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dashDir, dashDistance, unwalkableDetection);
		Vector2 playerTarget = (Vector2)transform.position + dashDir * dashDistance;
		Debug.Log(hit.collider);

		yield return new WaitForSeconds(windUpTime);
		Debug.DrawRay(this.transform.position, dashDir * dashDistance, Color.red, 1f);
		//Dash starts
		hasHitbox = true;
		swoogerAnimator.Play("SwoogerAttack");
		float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		enemySprite.flipX = false;
		//Rb2d.velocity = dashDir.normalized * dashSpeed;
		if (hit.point != Vector2.zero)
		{
			Debug.Log("i hit wall");
			yield return StartCoroutine(DashToTarget(hit.point - dashDir / 10f));
		}
		else
		{
			yield return StartCoroutine(DashToTarget(playerTarget));
		}


		//Landing starts
		//enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		swoogerAnimator.Play("SwoogerLanding");
		Rb2d.velocity = new Vector2(0, 0);
		hasHitbox = false;
		yield return new WaitForSeconds(endLag);
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Attacking = false;
	}

	IEnumerator DashToTarget(Vector2 target)
	{
		while (Vector2.Distance(transform.position, target) >= 0.01f)
		{
			Rb2d.transform.position = Vector2.MoveTowards(Rb2d.transform.position, target, dashSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, hitDetectionRadius);
	}
}
