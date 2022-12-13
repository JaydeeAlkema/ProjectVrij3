using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBCirclingEnemy : EnemyBase
{
	private int playerLayer = 1 << 8;
	[SerializeField] private LayerMask wallHitMask;
	[SerializeField] private GameObject crackedGround;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject boss;
	[SerializeField] private float explosionRadius;
	private Vector2 launchDestination;
	[SerializeField] private float launchLifeTime;
	private float lifeTimeTimer = 0f;
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
		launchLifeTime = Random.Range(1f, 2f);
	}

	void Update()
	{
		base.Update();
		HitBox();

		if (aggro)
		{
			LaunchToPlayer();
		}

		if (beingCrowdControlled)
		{
			BeingDisplaced();
		}
		else if (!aggro)
		{
			CircleAroundBoss();
		}
	}

	void CircleAroundBoss()
	{
		transform.RotateAround(boss.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
		var desiredPosition = (transform.position - boss.transform.position).normalized * orbitRadius + boss.transform.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed * Time.deltaTime);
		transform.rotation = Quaternion.identity;
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
			StartExploding();
		}
	}

	void AttackPlayer(GameObject playerObject)
	{
		playerObject.GetComponent<PlayerControler>().TakeDamage(damage);
	}

	void LaunchToPlayer()
	{
		transform.position = Vector2.MoveTowards(transform.position, launchDestination, Speed * Time.deltaTime);

		Collider2D wallHit = Physics2D.OverlapCircle(this.transform.position, this.GetComponent<CircleCollider2D>().radius, wallHitMask);
		if (wallHit != null)
		{
			StartExploding();
		}

		if (lifeTimeTimer >= launchLifeTime)
		{
			StartExploding();
		}
		else
		{
			lifeTimeTimer += Time.deltaTime;
		}
	}

	public void StartExploding()
	{
		enemyAnimator.SetTrigger("StartExplosion");
	}

	public void Explode()
	{
		Speed = 0f;
		Collider2D playerBody = Physics2D.OverlapCircle(this.transform.position, explosionRadius, playerLayer);
		if (playerBody != null)
		{
			AttackPlayer(playerBody.gameObject);
		}
		GameObject decal = Instantiate(crackedGround, transform.position, Quaternion.identity);
		decal.transform.localScale = Vector3.one * explosionRadius;
	}

	public override void BeingDisplaced()
	{
		StopMovingToTarget();
		if (Vector2.Distance(transform.position, pullPoint) > 0.5f)
		{
			//Debug.Log("actually gonna pull");
			GetComponent<Rigidbody2D>().MovePosition(Vector2.SmoothDamp(transform.position, pullPoint, ref ccVel, 8f * Time.deltaTime));
		}
		else
		{
			//Debug.Log("already at pullpoint: " + pullPoint + ", my position is: " + transform.position);
			beingCrowdControlled = false;
			this.pullPoint = Vector2.zero;
			Physics.IgnoreLayerCollision(avoidEnemyLayerMask.value, avoidEnemyLayerMask.value, true);
			if (aggro)
			{
				StartExploding();
			}
		}
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
