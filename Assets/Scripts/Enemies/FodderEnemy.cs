using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderEnemy : EnemyBase
{
	private int playerLayer = 1 << 8;

	[SerializeField] public Animator fodderAnimator;

	[SerializeField] private float damage;
	[SerializeField] private GameObject player;
	private float baseSpeed;

	[SerializeField] private float windUpTime = 0.3f;
	[SerializeField] private float dashSpeed = 9;
	[SerializeField] private float dashDuration = 0.5f;
	[SerializeField] private float endLag = 0.8f;
	public bool hasHitbox = false;
	public CapsuleCollider2D hurtbox;

	void Awake()
	{
		player = FindObjectOfType<PlayerControler>().gameObject;
		hurtbox = this.GetComponent<CapsuleCollider2D>();

		baseSpeed = Speed;
	}
	// Update is called once per frame
	void Update()
	{
		base.Update();
		//Vector3 targetDir = player.transform.position - this.rb2d.transform.position;
		//rb2d.velocity = targetDir.normalized * speed * Time.deltaTime;
		HitBox();
		LookAtTarget();
	}

	void HitBox()
	{
		Collider2D playerBody = Physics2D.OverlapCapsule((Vector2)this.transform.position, hurtbox.size, hurtbox.direction, playerLayer);
		if(playerBody != null & hasHitbox)
		{
			AttackPlayer(playerBody.gameObject);
			hasHitbox = false;
		}
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>().TakeDamage(damage);
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//	if (collision.gameObject.layer == 8)
	//	{
	//		AttackPlayer(collision.gameObject);
	//	}
	//}

	public override void TakeDamage(float damage, int damageType)
	{
		if (damageType == 0 && meleeTarget)
		{
			HealthPoints -= damage;
			meleeTarget = false;
		}
		if (damageType == 1 && castTarget)
		{
			HealthPoints -= damage;
			castTarget = false;
		}
		DamagePopup(damage);
		HealthPoints -= damage;
		StartCoroutine(FlashColor());
		if (HealthPoints <= 0) Die();
	}

	public override void MoveToTarget(Transform target)
	{
		base.MoveToTarget(target);
		fodderAnimator.Play("Fodder1Walk");
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

	public override IEnumerator FlashColor()
	{
		enemySprite.material = MaterialHit;
		yield return new WaitForSeconds(0.09f);
		enemySprite.material = MaterialDefault;
	}

	public IEnumerator DashAttack(Transform target)
	{
		Attacking = true;
		hasHitbox = true;
		fodderAnimator.Play("Fodder1Windup");
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Rb2d.velocity = new Vector2(0, 0);
		yield return new WaitForSeconds(windUpTime);
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		fodderAnimator.Play("Fodder1Attack");
		Vector2 dashDir = target.transform.position - Rb2d.transform.position;
		Rb2d.velocity = dashDir.normalized * dashSpeed;
		yield return new WaitForSeconds(dashDuration);
		fodderAnimator.Play("Fodder1Landing");
		Rb2d.velocity = new Vector2(0, 0);
		hasHitbox = false;
		yield return new WaitForSeconds(endLag);
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Attacking = false;
	}
}
