using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderEnemy : EnemyBase
{
	public LayerMask playerLayerMask;

	public Animator fodderAnimator;

	[SerializeField] private int damage;
	[SerializeField] private GameObject player;
	private float baseSpeed;

	[SerializeField] private float hitDetectionRadius = 1f;
	[SerializeField] private float windUpTime = 0.3f;
	[SerializeField] private float dashSpeed = 9;
	[SerializeField] private float dashDuration = 0.5f;
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

	public override void TakeDamage(int damage, int damageType)
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

		if (damageType == 0)
		{
			//AudioManager.PostEvent( , this.gameObject);   dit aanpassen als we wwise hebben enzo
			AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
		}

		if (damageType == 1)
		{
			AkSoundEngine.PostEvent("npc_dmg_cast", this.gameObject);
		}

		DamagePopup(damage);
		HealthPoints -= damage;
		StartCoroutine(HitStop());
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
		//Windup starts
		Attacking = true;
		fodderAnimator.Play("Fodder1Windup");
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Rb2d.velocity = new Vector2(0, 0);
		yield return new WaitForSeconds(windUpTime);

		//Dash starts
		hasHitbox = true;
		fodderAnimator.Play("Fodder1Attack");
		Vector2 dashDir = target.transform.position - Rb2d.transform.position;
		float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		enemySprite.flipX = false;
		Rb2d.velocity = dashDir.normalized * dashSpeed;
		yield return new WaitForSeconds(dashDuration);

		//Landing starts
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		fodderAnimator.Play("Fodder1Landing");
		Rb2d.velocity = new Vector2(0, 0);
		hasHitbox = false;
		yield return new WaitForSeconds(endLag);
		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
		Attacking = false;
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, hitDetectionRadius);
	}
}
