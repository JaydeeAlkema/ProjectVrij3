using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FodderEnemy : EnemyBase
{
	public LayerMask playerLayerMask;

	//[SerializeField] public TMP_Text coroutineText;

	[SerializeField] private int damage;
	[SerializeField] private GameObject player;

	[SerializeField] private GameObject SplatterDecal;

	private float baseSpeed;

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
			if (Target.gameObject != null && HasHitbox)
			{
				AttackPlayer(Target.gameObject);
				HasHitbox = false;
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
	public override void TakeDamage(int damage, int damageType)
	{
		int damageToTake = damage;
		if (damageType == 0 && meleeTarget)
		{
			HealthPoints -= damage;
			meleeTarget = false;
			damageToTake *= 2;
		}
		if (damageType == 1 && castTarget)
		{
			HealthPoints -= damage;
			castTarget = false;
			damageToTake *= 2;
		}

		if (damageType == 0)
		{
			IsStunned = true;
			//AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
			//StartCoroutine(HitStop());
		}

		if (damageType == 1)
		{
			//AkSoundEngine.PostEvent("npc_dmg_cast", this.gameObject);
		}
		OnHitVFX();
		StartCoroutine(HitStop(0.03f));
		Debug.Log("i took " + damage + " damage");
		DamagePopup(damageToTake);
		HealthPoints -= damage;
		IsAggro = true;
		//this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		StartCoroutine(FlashColor());
		if (HealthPoints <= 0) Die();
	}

	public override void MoveToTarget(Transform target)
	{
		base.MoveToTarget(target);
		enemyAnimator.Play("Fodder1Walk");
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
		Attacking = true;
		//StartCoroutine(WindupDashAttack(target));
	}

	public override IEnumerator FlashColor()
	{
		enemySprite.material = MaterialHit;
		enemyAnimator.Play("Fodder1Hit");
		yield return new WaitForSeconds(0.09f);
		enemySprite.material = MaterialDefault;
	}

	public IEnumerator Stunned()
	{

		//StopCoroutine("WindupDashAttack");
		//StopCoroutine("DashAttack");
		//StopCoroutine("DashToTarget");
		//StopCoroutine("LandingDashAttack");

		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		enemyAnimator.Play("Fodder1Landing");
		Rb2d.velocity = new Vector2(0, 0);
		HasHitbox = false;
		Attacking = false;



		yield return new WaitForSeconds(0.1f);

		IsStunned = false;

		yield return new WaitForEndOfFrame();
	}

	public void OnDestroy()
	{
		//Change name to dungeon name if changed later
		if (SceneManager.GetActiveScene().name == "Jaydee Testing Scene")
		{
			GameObject splatter = Instantiate(SplatterDecal, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 20)));
			splatter.GetComponent<SpriteRenderer>().flipX = (Random.value > 0.5f);
		}
	}

	//public IEnumerator WindupDashAttack(Transform target)
	//{
	//	coroutineText.text = "WindupDashAttack";

	//	//Windup starts
	//	while (!IsStunned)
	//	{
	//		Debug.Log("DOING THE WINDUP");
	//		fodderAnimator.Play("Fodder1Windup");
	//		enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
	//		//Rb2d.velocity = new Vector2(0, 0);
	//		Vector2 dashDir = (target.position - transform.position).normalized;
	//		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dashDir, dashDistance, unwalkableDetection);
	//		Vector2 maxDistanceTarget = (Vector2)transform.position + dashDir * dashDistance;
	//		Debug.Log(hit.collider);


	//		yield return new WaitForSeconds(windUpTime);
	//		Debug.Log("DONE WITH WINDUP, STARTING ATTACK");
	//		StartCoroutine(DashAttack(target, dashDir, hit, maxDistanceTarget));
	//		coroutineText.text = "Nothing";
	//		break;
	//	}
	//	coroutineText.text = "Nothing";
	//	yield return new WaitForEndOfFrame();
	//}

	//public IEnumerator DashAttack(Transform target, Vector2 dashDir, RaycastHit2D hit, Vector2 maxDistanceTarget)
	//{
	//	coroutineText.text = "DashAttack";
	//	Debug.Log("DOING THE DASH ATTACK");
	//	while (!IsStunned)
	//	{
	//		Debug.DrawRay(this.transform.position, dashDir * dashDistance, Color.red, 1f);
	//		//Dash starts
	//		hasHitbox = true;
	//		fodderAnimator.Play("Fodder1Attack");
	//		float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 180;
	//		enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
	//		enemySprite.flipX = false;
	//		//Rb2d.velocity = dashDir.normalized * dashSpeed;

	//		if (hit.point != Vector2.zero)
	//		{
	//			Debug.Log("i hit wall");
	//			StartCoroutine(DashToTarget(hit.point - dashDir / 10f, target));
	//			coroutineText.text = "Nothing";
	//			break;
	//		}
	//		else
	//		{
	//			Debug.Log("DASH MAX DISTANCE");
	//			StartCoroutine(DashToTarget(maxDistanceTarget, target));
	//			coroutineText.text = "Nothing";
	//			break;
	//		}
	//	}
	//	coroutineText.text = "Nothing";
	//	yield return new WaitForEndOfFrame();
	//}

	//IEnumerator DashToTarget(Vector2 target, Transform targetPlayer)
	//{
	//	coroutineText.text = "DashToTarget";
	//	Debug.Log("DASHING TO TARGET");
	//	while (Vector2.Distance(transform.position, target) >= 0.01f)
	//	{
	//		Rb2d.transform.position = Vector2.MoveTowards(Rb2d.transform.position, target, dashSpeed * Time.deltaTime);
	//		//if (IsStunned)
	//		//{
	//		//	target = transform.position;
	//		//	Attacking = false;
	//		//	StartCoroutine(LandingDashAttack(targetPlayer));
	//		//	break;
	//		//}
	//		yield return new WaitForEndOfFrame();
	//	}
	//	StartCoroutine(LandingDashAttack(targetPlayer));
	//	coroutineText.text = "Nothing";
	//	yield return new WaitForEndOfFrame();
	//}

	//IEnumerator LandingDashAttack(Transform target)
	//{
	//	coroutineText.text = "LandingDashAttack";
	//	Debug.Log("STARTING THE LANDING");
	//	//Landing starts
	//	enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
	//	enemySprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
	//	fodderAnimator.Play("Fodder1Landing");
	//	Rb2d.velocity = new Vector2(0, 0);
	//	hasHitbox = false;
	//	yield return new WaitForSeconds(endLag);
	//	enemySprite.flipX = (target.position - transform.position).normalized.x > 0 ? true : false;
	//	Attacking = false;
	//	Debug.Log("DONE WITH DASHATTACK");
	//	coroutineText.text = "Nothing";
	//	yield return new WaitForEndOfFrame();
	//}
}
