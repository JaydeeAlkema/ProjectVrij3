using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable, ICrowdControllable
{
	[SerializeField] private int healthPoints = 0;
	[SerializeField] private float speed = 0;
	[SerializeField] private float aggroRange = 0;
	[SerializeField] private float attackRange = 0;
	[SerializeField] private int expAmount = 0;
	[SerializeField] private int expAmountBase;
	[SerializeField] private bool attacking = false;
	[SerializeField] private Rigidbody2D rb2d;
	[SerializeField] public Transform damageNumberText;
	[SerializeField] public SpriteRenderer enemySprite = null;
	[SerializeField] private GameObject vfxHitSpark = null;

	public Animator enemyAnimator;

	[SerializeField] private bool hasHitbox = false;

	[SerializeField] private LayerMask unwalkableDetection;

	[SerializeField] private float windUpTime = 0.3f;
	[SerializeField] private float dashSpeed = 9;
	[SerializeField] private float dashDistance = 0.5f;
	[SerializeField] private float endLag = 0.8f;

	[SerializeField] public Material materialDefault = null;
	[SerializeField] public Material materialHit = null;

	private bool isStunned = false;
	private bool isAggro = false;

	private Transform target = null;

	[SerializeField] private Pathfinding.AIDestinationSetter destinationSetter;
	[SerializeField] private Pathfinding.AIPath aiPath;

	private Vector2[] path;
	private int targetIndex = 0;

	private Vector2 pullPoint = new Vector2();
	private Vector2 vel = new Vector2();
	public bool beingCrowdControlled = false;
	public bool meleeTarget = false;
	public bool castTarget = false;
	public LayerMask avoidEnemyLayerMask = default;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public float Speed { get => speed; set => speed = value; }
	public float AggroRange { get => aggroRange; set => aggroRange = value; }
	public float AttackRange { get => attackRange; set => attackRange = value; }
	public bool Attacking { get => attacking; set => attacking = value; }
	public Rigidbody2D Rb2d { get => rb2d; set => rb2d = value; }
	public Transform Target { get => target; set => target = value; }
	public int HealthPoints { get => healthPoints; set => healthPoints = value; }
	public Material MaterialDefault { get => materialDefault; set => materialDefault = value; }
	public Material MaterialHit { get => materialHit; set => materialHit = value; }
	public int ExpAmount { get => expAmount; set => expAmount = value; }
	public int ExpAmountBase { get => expAmountBase; set => expAmountBase = value; }
	public bool IsStunned { get => isStunned; set => isStunned = value; }
	public float DashDistance { get => dashDistance; private set => dashDistance = value; }
	public float DashSpeed { get => dashSpeed; private set => dashSpeed = value; }
	public float EndLag { get => endLag; private set => endLag = value; }
	public float WindUpTime { get => windUpTime; private set => windUpTime = value; }
	public LayerMask UnwalkableDetection { get => unwalkableDetection; private set => unwalkableDetection = value; }
	public bool HasHitbox { get => hasHitbox; set => hasHitbox = value; }
	public bool IsAggro { get => isAggro; set => isAggro = value; }

	public void Start()
	{
		if (enemySprite != null)
		{
			materialDefault = enemySprite.material;
		}
	}

	public void Awake()
	{

		rb2d = GetComponent<Rigidbody2D>();
		destinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
		aiPath = GetComponent<Pathfinding.AIPath>();
		expAmount = expAmountBase;
	}

	public void Update()
	{
		if(enemySprite != null)
		{
			enemySprite.sortingOrder = Mathf.CeilToInt(transform.position.y) - 2;
		}

		foreach (IStatusEffect statusEffect in statusEffects.ToArray())
		{
			IDamageable damageable = GetComponent<IDamageable>();
			if (statusEffect != null)
			{
				statusEffect.Process(damageable);
			}
		}

		if (aiPath != null)
		{
			aiPath.maxSpeed = speed;
		}

		if (healthPoints <= 0) { Die(); }

		if (beingCrowdControlled)
		{
			beingDisplaced();
			StopMovingToTarget();
		}
		else
		{
			//GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

		}
	}

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{
		if (statusEffects.Contains(statusEffect)) return;
		statusEffects.Add(statusEffect);
	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{
		if (!statusEffects.Contains(statusEffect)) return;
		statusEffects.Remove(statusEffect);
	}

	public void TakeDamage(int damage)
	{
		Debug.Log("i took " + damage + " damage without type");
		DamagePopup(damage);
		healthPoints -= damage;
		isAggro = true;
		//this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		StartCoroutine(FlashColor());
		if (healthPoints <= 0) Die();
	}

	public virtual void TakeDamage(int damage, int damageType)
	{
		int damageToTake = damage;
		if (damageType == 0 && meleeTarget)
		{
			healthPoints -= damage;
			meleeTarget = false;
			damageToTake *= 2;
		}
		if (damageType == 1 && castTarget)
		{
			healthPoints -= damage;
			castTarget = false;
			damageToTake *= 2;
		}

		if (damageType == 0)
		{
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
		healthPoints -= damage;
		isAggro = true;
		//this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		StartCoroutine(FlashColor());
		if (healthPoints <= 0) Die();
	}

	public void OnHitVFX()
	{
		Instantiate(vfxHitSpark, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
	}

	public void GetMarked(int markType)
	{
		switch (markType)
		{
			case 0:
				meleeTarget = true;
				break;
			case 1:
				castTarget = true;
				break;
		}
	}

	public virtual void GetSlowed(float slowAmount)
	{

	}

	public virtual void LookAtTarget()
	{
		if (destinationSetter.target != null && enemySprite != null)
		{
			enemySprite.flipX = (destinationSetter.target.position - transform.position).normalized.x > 0 ? true : false;
		}
	}

	public virtual void StartAttack(Transform target)
	{

	}

	public virtual void MoveToTarget(Transform target)
	{
		aiPath.enabled = true;
		destinationSetter.target = target;
	}

	public virtual void StopMovingToTarget()
	{
		aiPath.enabled = false;
		destinationSetter.target = null;
	}

	public virtual void DamagePopup(int damage)
	{
		Transform damageNumber = Instantiate(damageNumberText, transform.position, Quaternion.identity);
		damageNumber.GetComponent<DamageNumberPopup>()?.SetDamageText(damage);
		Debug.Log("TEXT CREATED WITH DAMAGE: " + damage);
	}

	public virtual IEnumerator FlashColor()
	{
		enemySprite.material = MaterialHit;
		yield return new WaitForSeconds(0.09f);
		enemySprite.material = MaterialDefault;
	}

	public virtual void Die()
	{
		GameManager.Instance.ExpManager.AddExp(expAmount);
		StopAllCoroutines();
		Time.timeScale = 1f;
		Destroy(this.gameObject);
	}

	private void beingDisplaced()
	{
		if (Vector2.Distance(transform.position, pullPoint) > 0.5f)
		{
			GetComponent<Rigidbody2D>().MovePosition(Vector2.SmoothDamp(transform.position, pullPoint, ref vel, 8f * Time.deltaTime));

		}
		else
		{
			beingCrowdControlled = false;
			Physics.IgnoreLayerCollision(avoidEnemyLayerMask.value, avoidEnemyLayerMask.value, true);
		}
	}

	public void Pull(Vector2 pullPoint)
	{
		beingCrowdControlled = true;
		Physics.IgnoreLayerCollision(avoidEnemyLayerMask.value, avoidEnemyLayerMask.value, false);
		this.pullPoint = pullPoint;
	}

	public IEnumerator HitStop(float duration)
	{
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1f;
		yield return null;
	}

	public void OnDrawGizmos()
	{


		if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				//Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}
