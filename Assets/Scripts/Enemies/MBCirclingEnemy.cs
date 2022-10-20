using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBCirclingEnemy : EnemyBase
{
	private int playerLayer = 1 << 8;

	[SerializeField] private float damage;
	[SerializeField] private GameObject player;
	[SerializeField] private bool aggro = false;
	[SerializeField] private bool hasHitbox = true;
	private float checkMaxHP;

	public Transform center;
	public float orbitRadius;
	public float radiusSpeed = 10f;
	public float rotationSpeed = 6f;

	void Awake()
	{
		checkMaxHP = HealthPoints;
		player = FindObjectOfType<PlayerControler>().gameObject;
		transform.position = (transform.position - center.position).normalized * orbitRadius + center.position;
	}

	void Update()
	{
		base.Update();
		HitBox();
		CircleAroundBoss();
		MoveToPlayer();
	}

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
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		StartCoroutine(FlashColor());
		aggro = true;
		if (HealthPoints <= 0) Die();
	}

	void CircleAroundBoss()
	{
		if (!aggro)
		{
			transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);
			var desiredPosition = (transform.position - center.position).normalized * orbitRadius + center.position;
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed * Time.deltaTime);
		}
	}

	void HitBox()
	{
		Collider2D playerBody = Physics2D.OverlapCircle(this.transform.position, this.GetComponent<CircleCollider2D>().radius, playerLayer);
		if (playerBody != null && hasHitbox)
		{
			AttackPlayer(playerBody.gameObject);
			hasHitbox = false;
			aggro = false;
		}
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>().TakeDamage(damage);
		StartCoroutine(AttackTimer());
	}

	IEnumerator AttackTimer()
	{
		yield return new WaitForSeconds(0.5f);
		hasHitbox = true;
		yield return null;
	}

	void MoveToPlayer()
	{
		if (aggro)
		{
			//Vector3 targetDir = player.transform.position - this.Rb2d.transform.position;
			//Rb2d.velocity = targetDir.normalized * Speed * Time.deltaTime;

			transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Speed*Time.deltaTime);

		}
	}

}
