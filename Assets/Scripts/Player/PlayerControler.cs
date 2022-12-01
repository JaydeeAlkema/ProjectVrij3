using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour, IDamageable
{
	[SerializeField] private ScriptableFloat moveSpeed;
	[SerializeField] private float vel = 0;

	private Vector3 mousePos;
	private float angle;
	private int horizontal = 0;
	private int vertical = 0;
	[SerializeField] private bool isDashing = false;
	[SerializeField] private bool invulnerable = false;
	[SerializeField] private float hitInvulTime = 1;
	[SerializeField] private float dashInvulTime = 1;
	//[SerializeField] private bool canMove = true;
	//[SerializeField]
	//private Camera cam;
	public Vector2 lookDir;
	[SerializeField] private Transform castFromPoint;
	[SerializeField] private Vector2 boxSize = new Vector2(4, 6);
	[SerializeField] private float circleSize = 3f;
	[SerializeField] private Rigidbody2D rb2d = default;
	[SerializeField] private SpriteRenderer playerSprite;
	[SerializeField] GameObject pivot_AttackAnimation;
	[SerializeField] GameObject attackAnimation;
	[SerializeField] Animator animAttack;
	[SerializeField] private TrailRenderer trail;
	[SerializeField] private GameObject playerDeathSpark = null;
	[SerializeField] private GameObject playerDeathPoof = null;
	public Animator AnimAttack { get => animAttack; set => animAttack = value; }

	[SerializeField] Animator animPlayer;

	//[SerializeField] PlayerHealthBar healthBar;
	private float selfSlowTime = 0.35f;
	private float selfSlowCounter = 0f;
	private float selfSlowMultiplier = 1f;
	private bool inCombat = false;
	private float outOfCombatCounter = 0f;
	[SerializeField] private float outOfCombatTimer = 3f;

	private float bufferCounterMelee = 0f;
	[SerializeField] private float bufferTimeMelee = 0.2f;
	private float bufferCounterCast = 0f;
	[SerializeField] private float bufferTimeCast = 0.2f;
	private float bufferCounterDash = 0f;
	[SerializeField] private float bufferTimeDash = 0.2f;


	public Material materialDefault = null;
	public Material materialHit = null;

	//Temporary, remove when implementing real death screen
	//[SerializeField] Transform deathScreenTest;
	//private bool isDying = false;

	public Animator AnimPlayer { get => animPlayer; set => animPlayer = value; }
	public GameObject AttackAnimation { get => attackAnimation; set => attackAnimation = value; }

	[SerializeField] private bool isAttackPositionLocked = false;

	//[SerializeField] private int maxHealthPoints = 500;
	//[SerializeField] private float currentHealthPoints;
	private AbilityController abilityController;
	public AbilityController AbilityController { get => abilityController; set => abilityController = value; }
	public ScriptableFloat MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
	public int Horizontal { get => horizontal; set => horizontal = value; }
	public int Vertical { get => vertical; set => vertical = value; }
	public bool IsDashing { get => isDashing; set => isDashing = value; }
	public TrailRenderer Trail { get => trail; set => trail = value; }
	public SpriteRenderer PlayerSprite { get => playerSprite; set => playerSprite = value; }

	[Header("Abilities")]
	#region ability fields
	[SerializeField] private AbilityScriptable meleeAttack;
	[SerializeField] private AbilityScriptable rangedAttack;
	[SerializeField] private AbilityScriptable dash;
	[SerializeField] private AbilityScriptable ability1;
	[SerializeField] private AbilityScriptable ability2;
	[SerializeField] private AbilityScriptable ability3;
	[SerializeField] private AbilityScriptable ability4;
	public AbilityScriptable MeleeAttackScr { get => meleeAttack; set => meleeAttack = value; }
	public AbilityScriptable RangedAttackScr { get => rangedAttack; set => rangedAttack = value; }
	public AbilityScriptable Dash { get => dash; set => dash = value; }
	public AbilityScriptable Ability1 { get => ability1; set => ability1 = value; }
	public AbilityScriptable Ability2 { get => ability2; set => ability2 = value; }
	public AbilityScriptable Ability3 { get => ability3; set => ability3 = value; }
	public AbilityScriptable Ability4 { get => ability4; set => ability4 = value; }

	private IAbility currentMeleeAttack = new MeleeAttack();
	private IAbility currentRangedAttack = new RangedAttack();
	private IAbility currentDash = new Dash();
	private IAbility currentAbility1;
	private IAbility currentAbility2;
	private IAbility currentAbility3;
	private IAbility currentAbility4;
	public IAbility CurrentMeleeAttack { get => currentMeleeAttack; set => currentMeleeAttack = value; }
	public IAbility CurrentRangedAttack { get => currentRangedAttack; set => currentRangedAttack = value; }
	public IAbility CurrentDash { get => currentDash; set => currentDash = value; }
	public IAbility CurrentAbility1 { get => currentAbility1; set => currentAbility1 = value; }
	public IAbility CurrentAbility2 { get => currentAbility2; set => currentAbility2 = value; }
	public IAbility CurrentAbility3 { get => currentAbility3; set => currentAbility3 = value; }
	public IAbility CurrentAbility4 { get => currentAbility4; set => currentAbility4 = value; }
	public float BufferCounterMelee { get => bufferCounterMelee; set => bufferCounterMelee = value; }
	public float BufferCounterCast { get => bufferCounterCast; set => bufferCounterCast = value; }
	public float BufferCounterDash { get => bufferCounterDash; set => bufferCounterDash = value; }
	public bool IsAttackPositionLocked { get => isAttackPositionLocked; set => isAttackPositionLocked = value; }
	public float SelfSlowCounter { get => selfSlowCounter; set => selfSlowCounter = value; }
	public bool Invulnerable { get => invulnerable; set => invulnerable = value; }
	public GameObject Pivot_AttackAnimation { get => pivot_AttackAnimation; set => pivot_AttackAnimation = value; }

	#endregion


	// Start is called before the first frame update
	void Start()
	{
		abilityController = AbilityController.AbilityControllerInstance;
		rb2d = GetComponent<Rigidbody2D>();
		abilityController.Player = this;
		currentMeleeAttack.BaseStats = meleeAttack;
		currentRangedAttack.BaseStats = rangedAttack;
		currentDash.BaseStats = dash;
		currentMeleeAttack.SetStartValues();
		currentRangedAttack.SetStartValues();
		currentDash.SetStartValues();
		abilityController.CurrentMeleeAttack = currentMeleeAttack;
		abilityController.CurrentRangedAttack = currentRangedAttack;
		abilityController.CurrentDash = currentDash;
		Debug.Log(currentMeleeAttack.BurnDamage);
		abilityController.SetAttacks();
		trail.emitting = false;

		materialDefault = playerSprite.material;

		selfSlowCounter = selfSlowTime;

		//currentHealthPoints = maxHealthPoints;
		//if (healthBar != null)
		//{
		//	healthBar.SetMaxHP(maxHealthPoints);
		//}

		//Death screen test, remove later
		//if (deathScreenTest != null)
		//{
		//	deathScreenTest.gameObject.SetActive(false);
		//}

	}

	public void ReloadAttacks()
	{
		currentMeleeAttack.BaseStats = meleeAttack;
		currentRangedAttack.BaseStats = rangedAttack;
		currentDash.BaseStats = dash;
		abilityController.CurrentMeleeAttack = currentMeleeAttack;
		abilityController.CurrentRangedAttack = currentRangedAttack;
		abilityController.CurrentDash = currentDash;
		//abilityController.SetAttacks();
	}

	public void initAbilities()
	{
		if (currentAbility1 != null) { currentAbility1.BaseStats = ability1; abilityController.CurrentAbility1 = currentAbility1; }
		if (currentAbility2 != null) { currentAbility2.BaseStats = ability2; abilityController.CurrentAbility2 = currentAbility2; }
		if (currentAbility3 != null) { currentAbility3.BaseStats = ability3; abilityController.CurrentAbility3 = currentAbility3; }
		if (currentAbility4 != null) { currentAbility4.BaseStats = ability4; abilityController.CurrentAbility4 = currentAbility4; }
		abilityController.SetAbility();
	}

	// Update is called once per frame
	void Update()
	{
		CheckAbilityUpdate();

		vel = rb2d.velocity.magnitude;

		if (GameManager.Instance == null || !GameManager.Instance.IsPaused)
		{
			//Melee input
			if (Input.GetMouseButtonDown(0))
			{
				bufferCounterMelee = bufferTimeMelee;
			}
			else
			{
				bufferCounterMelee -= Time.fixedDeltaTime;
			}
			if (bufferCounterMelee > 0f) MeleeAttack();

			//Cast input
			if (Input.GetMouseButtonDown(1))
			{
				bufferCounterCast = bufferTimeCast;
			}
			else
			{
				bufferCounterCast -= Time.fixedDeltaTime;
			}
			if (bufferCounterCast > 0f) RangedAttack();


			if (Input.GetKeyDown(KeyCode.Q)) AbilityOneAttack();
			if (Input.GetKeyDown(KeyCode.E)) AbilityTwoAttack();
			if (Input.GetKeyDown(KeyCode.R)) AbilityThreeAttack();
			if (Input.GetKeyDown(KeyCode.T)) AbilityFourAttack();

			//Dash input
			if (Input.GetKeyDown(KeyCode.Space))
			{
				bufferCounterDash = bufferTimeDash;
			}
			else
			{
				bufferCounterDash -= Time.fixedDeltaTime;
			}
			if (bufferCounterDash > 0f) DashAbility();

			MouseLook();

			playerSprite.flipX = lookDir.x > 0 ? true : false;
		}

		Debug.DrawRay(rb2d.position, lookDir, Color.magenta);
		if (!isDashing)
		{
			trail.emitting = false;
			horizontal = (int)Input.GetAxisRaw("Horizontal");
			vertical = (int)Input.GetAxisRaw("Vertical");
			rb2d.velocity = new Vector3(horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime).normalized * MoveSpeed.value * selfSlowMultiplier;
		}
		CastSelfSlow();
		OutOfCombatSpeed();
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = Matrix4x4.TRS(rb2d.transform.position + castFromPoint.transform.up * meleeAttack.Distance, castFromPoint.transform.rotation, meleeAttack.BoxSize);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		//Gizmos.color = Color.red;
		//Gizmos.matrix = Matrix4x4.TRS(rb2d.transform.position + castFromPoint.transform.up * 5, castFromPoint.transform.rotation, new Vector3(circleSize, circleSize, 0));
		//Gizmos.DrawWireSphere(Vector3.zero, 1);
	}

	void CastSelfSlow()
	{
		//Slow player while using ranged attack
		if (selfSlowCounter < selfSlowTime)
		{
			selfSlowCounter += Time.deltaTime;
			selfSlowMultiplier = 0.4f;
		}
		//Speed-up player while melee attacking
		else if (isAttackPositionLocked)
		{
			selfSlowMultiplier = 1.5f;
		}
		else
		{
			selfSlowMultiplier = 1f;
		}
	}

	void MouseLook()
	{
		if (!isAttackPositionLocked)
		{
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lookDir = mousePos - rb2d.transform.position;
			angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
			castFromPoint.transform.rotation = Quaternion.Euler(0f, 0f, angle);
			pivot_AttackAnimation.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180);

			attackAnimation.GetComponent<SpriteRenderer>().flipX = lookDir.x > 0 ? true : false;
		}
	}


	void MeleeAttack()
	{
		outOfCombatCounter = 0f;
		abilityController.CurrentMeleeAttack.BaseStats = meleeAttack;
		abilityController.CurrentMeleeAttack.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.MeleeAttacked(currentMeleeAttack);
	}

	void RangedAttack()
	{
		outOfCombatCounter = 0f;
		abilityController.CurrentRangedAttack.BaseStats = rangedAttack;
		abilityController.CurrentRangedAttack.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.RangeAttacked(currentRangedAttack);
	}

	void DashAbility()
	{
		abilityController.CurrentDash.BaseStats = dash;
		abilityController.CurrentDash.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.Dashing(currentDash);
		StartCoroutine(PlayerIFrames(dashInvulTime));
	}

	void AbilityOneAttack()
	{
		if (currentAbility1 != null)
		{
			outOfCombatCounter = 0f;
			Debug.Log(CurrentAbility1);
			abilityController.CurrentAbility1.BaseStats = ability1;
			abilityController.CurrentAbility1.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityOneAttacked(currentAbility1);
		}
	}

	void AbilityTwoAttack()
	{
		if (currentAbility2 != null)
		{
			outOfCombatCounter = 0f;
			Debug.Log(CurrentAbility2);
			abilityController.CurrentAbility2.BaseStats = ability2;
			abilityController.CurrentAbility2.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityTwoAttacked(currentAbility2);
		}
	}

	void AbilityThreeAttack()
	{
		if (currentAbility3 != null)
		{
			outOfCombatCounter = 0f;
			Debug.Log(CurrentAbility3);
			abilityController.CurrentAbility3.BaseStats = ability3;
			abilityController.CurrentAbility3.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityThreeAttacked(currentAbility3);
		}
	}

	void AbilityFourAttack()
	{
		if (currentAbility4 != null)
		{
			outOfCombatCounter = 0f;
			Debug.Log(currentAbility4);
			abilityController.CurrentAbility4.BaseStats = ability4;
			abilityController.CurrentAbility4.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityFourAttacked(currentAbility4);
		}
	}

	void CheckAbilityUpdate()
	{
		if (meleeAttack != null)
		{
			meleeAttack.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (rangedAttack != null)
		{
			rangedAttack.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (dash != null)
		{
			dash.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (ability1 != null)
		{
			ability1.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (ability2 != null)
		{
			ability2.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (ability3 != null)
		{
			ability3.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}

		if (ability4 != null)
		{
			ability4.UpdateStatusEffects();
			abilityController.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		}
		abilityController.UpdateCoolDown(meleeAttack, rangedAttack, ability1, ability2, ability3, ability4, dash);
	}

	public void TakeDamage(int damage)
	{
		if (GameManager.Instance != null)
		{
			if (!invulnerable && GameManager.Instance.currentGameState == GameManager.GameState.Dungeon)
			{
				AkSoundEngine.PostEvent("plr_dmg_npc", this.gameObject);
				GameObject onHitSpark = Instantiate(playerDeathSpark, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
				onHitSpark.GetComponent<SpriteRenderer>().color = new Color32(149, 43, 84, 255);
				StartCoroutine(HitSlow(0.12f));
				StartCoroutine(playerFlashColor());
				GameManager.Instance.PlayerHP.value -= damage;
				//healthBar.SetHP(currentHealthPoints);
				invulnerable = true;
				outOfCombatCounter = 0f;
				StartCoroutine(PlayerIFrames(hitInvulTime));
			}
		}
		else //If in testing scene, damage visuals without changing HP
		{
			if (!invulnerable)
			{
				AkSoundEngine.PostEvent("plr_dmg_npc", this.gameObject);
				GameObject onHitSpark = Instantiate(playerDeathSpark, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
				onHitSpark.GetComponent<SpriteRenderer>().color = new Color32(149, 43, 84, 255);
				StartCoroutine(HitSlow(0.12f));
				StartCoroutine(playerFlashColor());
				invulnerable = true;
				outOfCombatCounter = 0f;
				StartCoroutine(PlayerIFrames(hitInvulTime));
			}
		}
	}

	public void GameOverVFX(int vfx)
	{
		switch (vfx)
		{
			case 1:
				GameObject deathSpark = Instantiate(playerDeathSpark, transform.position, Quaternion.identity);
				deathSpark.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				deathSpark.GetComponent<SpriteRenderer>().color = Color.red;
				break;
			case 2:
				Instantiate(playerDeathPoof, transform.position, Quaternion.identity);
				break;

		}

	}

	public void OutOfCombatSpeed()
	{
		if (outOfCombatCounter < outOfCombatTimer)
		{
			outOfCombatCounter += Time.deltaTime;
			inCombat = true;
			Debug.Log("In combat, no boost");
		}
		else
		{
			inCombat = false;
		}

		if (GameManager.Instance != null)
		{
			if (!inCombat && GameManager.Instance.ExpManager.PlayerPoints >= 1)
			{
				selfSlowMultiplier = 2f;
				Debug.Log("Out of combat boost granted");
			}
		}
	}

	public void TakeDamage(int damage, int damageType)
	{

	}

	public void GetSlowed(float slowAmount)
	{

	}

	public void GetMarked(int markType)
	{

	}

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{

	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{

	}

	//void Die()
	//{
	//	isDying = true;
	//	//StartCoroutine(DeathSequence());
	//	Debug.Log("I HAVE DIED OH NO");
	//}

	//IEnumerator DeathSequence()
	//{
	//	Time.timeScale = 0f;    //Hitstop
	//	yield return new WaitForSecondsRealtime(1f);
	//	Sprite.gameObject.SetActive(false);
	//	Time.timeScale = 1f;
	//	yield return new WaitForSecondsRealtime(1.5f);

	//	//Deathscreen
	//	GameManager.Instance.UiManager.DisableAllUI();
	//	GameManager.Instance.UiManager.SetUIActive(4, true);
	//	yield return new WaitForSecondsRealtime(3f);

	//	Respawn();
	//	GameManager.Instance.UiManager.DisableAllUI();
	//	GameManager.Instance.UiManager.SetUIActive(0, true);
	//	isDying = false;
	//	yield return null;
	//}

	public IEnumerator HitSlow(float duration)
	{
		Time.timeScale = 0.2f;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1f;
		yield return null;
	}

	IEnumerator playerFlashColor()
	{
		playerSprite.material = materialHit;
		yield return new WaitForSeconds(0.09f);
		playerSprite.material = materialDefault;
	}

	IEnumerator PlayerIFrames(float invulTime)
	{
		yield return new WaitForSeconds(invulTime);
		invulnerable = false;
		yield return new WaitForEndOfFrame();
	}
	//IEnumerator PlayerIFrames(int iFrameAmount)
	//{
	//	for (int i = 0; i < iFrameAmount; i++)
	//	{
	//		yield return new WaitForEndOfFrame();
	//	}
	//	invulnerable = false;
	//	yield return new WaitForEndOfFrame();
	//}
}
