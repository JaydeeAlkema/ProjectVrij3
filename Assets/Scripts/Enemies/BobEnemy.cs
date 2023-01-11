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

	[SerializeField] private GameObject player;
	[SerializeField] private GameObject enemyProjectile;
	private float baseSpeed;

	[SerializeField] private GameObject runToObject = null;

	[SerializeField] private float hitDetectionRadius = 1f;
	//public CapsuleCollider2D hurtbox;

	public AK.Wwise.Event Event;

	[SerializeField] private AK.Wwise.Event bobDmg;
	[SerializeField] private AK.Wwise.Event bobAtk;

	public GameObject EnemyProjectile { get => enemyProjectile; set => enemyProjectile = value; }
	public AK.Wwise.Event BobAtk { get => bobAtk; set => bobAtk = value; }

	void Awake()
	{
		base.Awake();
		//hurtbox = this.GetComponent<CapsuleCollider2D>();

		baseSpeed = Speed;
	}

	void Update()
	{
		base.Update();
		//if (Target != null)
		//{
		//	//HitBox();
		//}
		LookAtTarget();
	}

	//void HitBox()
	//{
	//	if (Vector2.Distance(transform.position, Target.position) <= 1)
	//	{
	//		if (Target.gameObject != null && HasHitbox)
	//		{
	//			AttackPlayer(Target.gameObject);
	//			//HasHitbox = false;
	//		}
	//	}
	//	//Collider2D playerBody = Physics2D.OverlapCapsule((Vector2)this.transform.position, hurtbox.size, hurtbox.direction, playerLayerMask);
	//}

	void AttackPlayer(GameObject playerObject)
	{
		int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
		playerObject.GetComponent<PlayerControler>()?.TakeDamage(damageToDeal);
	}

	public override void TakeDamage(int damage, int damageType)
	{
		int damageToTake = damage;
		if (damageType == 0 && meleeTarget)
		{
			HealthPoints -= ((int)(damage * markHits));
			meleeTarget = false;
			meleeMarkDecal.SetActive(false);
			damageToTake = ((int)(damageToTake * markHits));
			damageToTake += damage;
		}
		if (damageType == 1 && castTarget)
		{
			HealthPoints -= ((int)(damage * markHits));
			castTarget = false;
			castMarkDecal.SetActive(false);
			damageToTake = ((int)(damageToTake * markHits));
			damageToTake += damage;
		}

		if (damageType == 0)
		{
			//AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
			if (Time.timeScale == 1f)
			{
				StartCoroutine(HitStop(0.02f));
			}
		}

		if (damageType == 1)
		{
		}
		OnHitVFX();
		//if (bobDmg != null)
		//{
		//	AudioManager.Instance.PostEventLocal(bobDmg, this.gameObject);
		//}
		DamagePopup(damageToTake);
		HealthPoints -= damage;
		if (!IsAggro)
		{
			IsAggro = true;
			if (GameManager.Instance != null)
			{
				GameManager.Instance.EnemyAggroCount(true);
			}
		}
		StartCoroutine(FlashColor());
		if (HealthPoints <= 0) Die();
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
		if (GameManager.Instance != null)
		{
			DropExpOrbsOnDeath();
			GameManager.Instance.EnemyAggroCount(false);
		}
		StopAllCoroutines();
		Time.timeScale = 1f;

		GameObject rewardObject = Instantiate(rewardInstance, this.transform.position, Quaternion.identity);
		rewardObject.GetComponent<RewardChoice>().AbilityStats = abilityStats;
		rewardObject.GetComponent<RewardChoice>().AbilityToGive = ability;
		rewardObject.GetComponent<RewardChoice>().Reward = reward;
		Debug.Log(rewardObject.GetComponent<RewardChoice>().AbilityToGive);
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
