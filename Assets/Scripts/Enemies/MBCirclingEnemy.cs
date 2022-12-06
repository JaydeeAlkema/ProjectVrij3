using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBCirclingEnemy : EnemyBase
{
	private int playerLayer = 1 << 8;
	[SerializeField] private GameObject crackedGround;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject boss;
	[SerializeField] private float explosionRadius;
	private Vector2 launchDestination;
	//private float checkMaxHP;

	public Vector3 center;
	public float orbitRadius;
	public float radiusSpeed = 10f;
	public float rotationSpeed = 6f;
	public bool agitated = false;
	public bool aggro = false;

	public GameObject Boss { get => boss; set => boss = value; }
	public Vector2 LaunchDestination { get => launchDestination; set => launchDestination = value; }


	private void Start()
	{
		base.Start();
		player = FindObjectOfType<PlayerControler>().gameObject;
		transform.position = (transform.position - boss.transform.position).normalized * orbitRadius + boss.transform.position;
	}

	void Update()
	{
		base.Update();
		HitBox();
		CircleAroundBoss();
		if (aggro)
		{
			LaunchToPlayer();
		}
	}

	//public override void TakeDamage(int damage, int damageType)
	//{
	//	if (damageType == 0 && meleeTarget)
	//	{
	//		HealthPoints -= damage;
	//		meleeTarget = false;
	//	}
	//	if (damageType == 1 && castTarget)
	//	{
	//		HealthPoints -= damage;
	//		castTarget = false;
	//	}
	//	DamagePopup(damage);
	//	HealthPoints -= damage;
	//	enemySprite.color = Color.white;
	//	StartCoroutine(FlashColor());
	//	if (HealthPoints <= 0) Die();
	//}

	void CircleAroundBoss()
	{
		if (!aggro)
		{
			transform.RotateAround(boss.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
			var desiredPosition = (transform.position - boss.transform.position).normalized * orbitRadius + boss.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed * Time.deltaTime);
			transform.rotation = Quaternion.identity;
		}
	}

	void HitBox()
	{
		Collider2D playerBody = Physics2D.OverlapCircle(this.transform.position, this.GetComponent<CircleCollider2D>().radius, playerLayer);
		if (playerBody != null && !aggro)
		{
			AttackPlayer(playerBody.gameObject);
		}
		else if (playerBody != null && aggro && !playerBody.gameObject.GetComponent<PlayerControler>().Invulnerable)
		{
			Explode();
		}
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>().TakeDamage(damage);
	}

	void LaunchToPlayer()
	{
		transform.position = Vector2.MoveTowards(transform.position, launchDestination, Speed * Time.deltaTime);
		if (Vector2.Distance(transform.position, launchDestination) < 0.5f)
		{
			Explode();
		}
	}

	void Explode()
	{
		Collider2D playerBody = Physics2D.OverlapCircle(this.transform.position, explosionRadius, playerLayer);
		if (playerBody != null)
		{
			AttackPlayer(playerBody.gameObject);
		}
		GameObject decal = Instantiate(crackedGround, transform.position, Quaternion.identity);
		decal.transform.localScale = Vector3.one * explosionRadius;
		Die();
	}

	public override void Die()
	{
		StopAllCoroutines();
		Time.timeScale = 1f;
		if (GameManager.Instance != null)
		{
			GameManager.Instance.EnemyAggroCount(false);
		}
		Destroy(this.gameObject);
	}

}
