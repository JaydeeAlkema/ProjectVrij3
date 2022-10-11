using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderEnemy : EnemyBase
{
	private int playerLayer = 1 << 8;

	[SerializeField] private float damage;
	[SerializeField] private GameObject player;
	[SerializeField] private Rigidbody2D rb2d;
	private float baseSpeed;

	[SerializeField] private float windUpTime = 0.5f;
	[SerializeField] private float dashSpeed = 15;
	[SerializeField] private float dashDuration = 0.4f;
	[SerializeField] private float endLag = 2f;
	public bool hasHitbox = false;
	void Awake()
	{
		player = FindObjectOfType<PlayerControler>().gameObject;
		rb2d = GetComponent<Rigidbody2D>();

		baseSpeed = Speed;
	}
	// Update is called once per frame
	void Update()
	{
		base.Update();
		//Vector3 targetDir = player.transform.position - this.rb2d.transform.position;
		//rb2d.velocity = targetDir.normalized * speed * Time.deltaTime;
		HitBox();
	}

	void HitBox()
	{
		Collider2D playerBody = Physics2D.OverlapCircle(this.transform.position, this.GetComponent<CircleCollider2D>().radius, playerLayer);
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
		StartCoroutine(DashAttack(target));
	}

	public IEnumerator DashAttack(Transform target)
	{
		Attacking = true;
		hasHitbox = true;
		rb2d.velocity = new Vector2(0, 0);
		yield return new WaitForSeconds(windUpTime);
		Vector2 dashDir = target.transform.position - rb2d.transform.position;
		rb2d.velocity = dashDir.normalized * dashSpeed;
		yield return new WaitForSeconds(dashDuration);
		rb2d.velocity = new Vector2(0, 0);
		hasHitbox = false;
		yield return new WaitForSeconds(endLag);
		Attacking = false;
	}
}
