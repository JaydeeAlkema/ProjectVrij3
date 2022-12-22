using System.Collections;
using UnityEngine;

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
	[SerializeField] private GameObject pivot_AttackAnimation;
	[SerializeField] private GameObject pivot_DashAnimation;
	[SerializeField] private GameObject attackAnimation;
	[SerializeField] private GameObject dashVFX;
	[SerializeField] Animator animAttack;
	[SerializeField] private GameObject playerDeathSpark = null;
	[SerializeField] private GameObject playerDeathPoof = null;
	public Animator AnimAttack { get => animAttack; set => animAttack = value; }

	[SerializeField] Animator animPlayer;

	//hold right mousebutton
	[SerializeField] private float holdTime = 0;

	//[SerializeField] PlayerHealthBar healthBar;
	private float selfSlowTime = 0.25f;
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


	[SerializeField] private SpriteMask flashBlueMask;
	[SerializeField] private SpriteRenderer flashBlue;

	public Material materialDefault = null;
	public Material materialHit = null;


	public bool isDying = false;

	public Animator AnimPlayer { get => animPlayer; set => animPlayer = value; }
	public GameObject AttackAnimation { get => attackAnimation; set => attackAnimation = value; }

	[SerializeField] private bool isAttackPositionLocked = false;

	//[SerializeField] private int maxHealthPoints = 500;
	//[SerializeField] private float currentHealthPoints;
	[SerializeField] private AbilityController abilityController;
	public AbilityController PlayerAbilityController { get => abilityController; set => abilityController = value; }
	public ScriptableFloat MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
	public int Horizontal { get => horizontal; set => horizontal = value; }
	public int Vertical { get => vertical; set => vertical = value; }
	public bool IsDashing { get => isDashing; set => isDashing = value; }
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

	private IAbility currentMeleeAttack;
	private IAbility currentRangedAttack;
	private IAbility currentDash;
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
	public GameObject DashVFX { get => dashVFX; set => dashVFX = value; }
	public GameObject Pivot_DashAnimation { get => pivot_DashAnimation; set => pivot_DashAnimation = value; }

	#endregion

	private void OnDestroy()
	{
		GameManager.Instance.OnGameStateChanged -= OnStart;
	}

	private void Awake()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnGameStateChanged += OnStart;
			Debug.Log("subscribed to ongamestatechanged");
		}
	}

	// Start is called before the first frame update
	void Start()
	{

		materialDefault = playerSprite.material;

		selfSlowCounter = selfSlowTime;
		abilityController = AbilityController.AbilityControllerInstance;
		abilityController.CurrentMeleeAttack = currentMeleeAttack;
		abilityController.CurrentRangedAttack = currentRangedAttack;
		abilityController.CurrentDash = currentDash;
		rb2d = GetComponent<Rigidbody2D>();
		abilityController.Player = this;
		currentMeleeAttack.BaseStats = meleeAttack;
		currentRangedAttack.BaseStats = rangedAttack;
		currentDash.BaseStats = dash;
		currentMeleeAttack.SetStartValues();
		currentRangedAttack.SetStartValues();
		currentDash.SetStartValues();
		abilityController.SetAttacks();
		initAbilities();

		if (Time.timeScale != 1f)
		{
			StopCoroutine("HitSlow");
			Time.timeScale = 1f;
		}
	}

	public void OnStart(GameState gameState, GameState lastGameState)
	{
		Debug.Log("ongamestatechanged got called with state: " + gameState + " and with last state: " + lastGameState);
		if (gameState == GameState.Dungeon && lastGameState != gameState)
		{
			currentMeleeAttack = new MeleeAttack();
			currentRangedAttack = new RangedAttack();
			currentDash = new Dash();
			abilityController = AbilityController.AbilityControllerInstance;
			Debug.Log(abilityController);
			rb2d = GetComponent<Rigidbody2D>();
			abilityController.Player = this;
			currentMeleeAttack.BaseStats = meleeAttack;
			currentMeleeAttack.BaseStats.SetBaseStats();
			currentRangedAttack.BaseStats = rangedAttack;
			currentRangedAttack.BaseStats.SetBaseStats();
			currentDash.BaseStats = dash;
			currentDash.BaseStats.SetBaseStats();
			currentMeleeAttack.SetStartValues();
			currentRangedAttack.SetStartValues();
			currentDash.SetStartValues();
			abilityController.CurrentMeleeAttack = currentMeleeAttack;
			abilityController.CurrentRangedAttack = currentRangedAttack;
			abilityController.CurrentDash = currentDash;
			//Debug.Log(currentMeleeAttack.BurnDamage);
			abilityController.SetAttacks();
		}
		if (GameManager.Instance != null)
		{
			GameManager.Instance.UiManager.AssignPlayerCameraToMainCanvas(GetComponentInChildren<Camera>());
			GameManager.Instance.UiManager.AssignPlayerCameraToShaderCanvas(GetComponentInChildren<Camera>());
		}
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
		GameManager.Instance.UiManager.ResetAbilityUIValues();
		if (currentAbility1 != null) { currentAbility1.BaseStats = ability1; abilityController.CurrentAbility1 = currentAbility1; GameManager.Instance.UiManager.SetAbilityUIValues(0, currentAbility1.BaseStats.AbilityIcon); }
		if (currentAbility2 != null) { currentAbility2.BaseStats = ability2; abilityController.CurrentAbility2 = currentAbility2; GameManager.Instance.UiManager.SetAbilityUIValues(1, currentAbility2.BaseStats.AbilityIcon); }
		if (currentAbility3 != null) { currentAbility3.BaseStats = ability3; abilityController.CurrentAbility3 = currentAbility3; GameManager.Instance.UiManager.SetAbilityUIValues(2, currentAbility3.BaseStats.AbilityIcon); }
		if (currentAbility4 != null) { currentAbility4.BaseStats = ability4; abilityController.CurrentAbility4 = currentAbility4; GameManager.Instance.UiManager.SetAbilityUIValues(3, currentAbility4.BaseStats.AbilityIcon); }
		abilityController.SetAbility();
	}

	// Update is called once per frame
	void Update()
	{
		vel = rb2d.velocity.magnitude;

		if ((GameManager.Instance == null || !GameManager.Instance.IsPaused) && !isDying)
		{
			//Melee input
			if (Input.GetMouseButtonDown(0))
			{
				bufferCounterMelee = bufferTimeMelee;
			}
			else
			{
				bufferCounterMelee -= Time.deltaTime;
			}
			if (bufferCounterMelee > 0f) MeleeAttack();

			//Cast input
			if (Input.GetMouseButtonDown(1)) { holdTime = 0; }
			if (Input.GetMouseButton(1))
			{
				if (currentRangedAttack.CooledDown)
				{
					Debug.Log("Holding");
					currentRangedAttack.Charging = true;
					holdTime++;
					currentRangedAttack.ChargeTime = holdTime;

				}
				else
				{
					Debug.Log("fizzle******");
				}
			}
			else
			{
				if (holdTime <= 5) { currentRangedAttack.Charging = false; }
				if (Input.GetMouseButtonUp(1))
				{
					bufferCounterCast = bufferTimeCast;
				}
				else
				{
					bufferCounterCast -= Time.deltaTime;
				}
				if (bufferCounterCast > 0f) RangedAttack();
				holdTime = 0;
			}


			if (Input.GetKeyDown(KeyCode.Q)) AbilityOneAttack();
			if (Input.GetKeyDown(KeyCode.E)) AbilityTwoAttack();
			if (Input.GetKeyDown(KeyCode.R)) AbilityThreeAttack();
			if (Input.GetKeyDown(KeyCode.T)) AbilityFourAttack();

			//Dash input
			if (Input.GetKeyDown(KeyCode.Space) && vel != 0f)
			{
				bufferCounterDash = bufferTimeDash;
			}
			else
			{
				bufferCounterDash -= Time.deltaTime;
			}
			if (bufferCounterDash > 0f) DashAbility();

			MouseLook();
		}

		Debug.DrawRay(rb2d.position, lookDir, Color.magenta);
		if (!isDashing && !isDying)
		{
			playerSprite.flipX = lookDir.x > 0;
			horizontal = (int)Input.GetAxisRaw("Horizontal");
			vertical = (int)Input.GetAxisRaw("Vertical");
			rb2d.velocity = MoveSpeed.value * selfSlowMultiplier * new Vector3(horizontal, vertical).normalized;
		}
		CastSelfSlow();
		OutOfCombatSpeed();
		//CheckAbilityUpdate();
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
			selfSlowMultiplier = 0.0f;
		}
		//slow player on charging
		else if (holdTime > 5)
		{
			selfSlowMultiplier = 0.5f;
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

			attackAnimation.GetComponent<SpriteRenderer>().flipX = lookDir.x > 0;
		}
	}

	void MeleeAttack()
	{
		CheckAbilityUpdate();
		outOfCombatCounter = 0f;
		abilityController.CurrentMeleeAttack.BaseStats = meleeAttack;
		abilityController.CurrentMeleeAttack.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.MeleeAttacked(currentMeleeAttack);
	}

	void RangedAttack()
	{
		CheckAbilityUpdate();
		outOfCombatCounter = 0f;
		abilityController.CurrentRangedAttack.BaseStats = rangedAttack;
		abilityController.CurrentRangedAttack.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.RangeAttacked(currentRangedAttack);
	}

	void DashAbility()
	{
		CheckAbilityUpdate();
		abilityController.CurrentDash.BaseStats = dash;
		abilityController.CurrentDash.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		abilityController.Dashing(currentDash);
		StartCoroutine(PlayerIFrames(dashInvulTime));
	}

	void AbilityOneAttack()
	{
		if (currentAbility1 != null)
		{
			CheckAbilityUpdate();
			outOfCombatCounter = 0f;
			//Debug.Log(CurrentAbility1);
			abilityController.CurrentAbility1.BaseStats = ability1;
			abilityController.CurrentAbility1.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityOneAttacked(currentAbility1);
		}
	}

	void AbilityTwoAttack()
	{
		if (currentAbility2 != null)
		{
			CheckAbilityUpdate();
			outOfCombatCounter = 0f;
			//Debug.Log(CurrentAbility2);
			abilityController.CurrentAbility2.BaseStats = ability2;
			abilityController.CurrentAbility2.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityTwoAttacked(currentAbility2);
		}
	}

	void AbilityThreeAttack()
	{
		if (currentAbility3 != null)
		{
			CheckAbilityUpdate();
			outOfCombatCounter = 0f;
			//Debug.Log(CurrentAbility3);
			abilityController.CurrentAbility3.BaseStats = ability3;
			abilityController.CurrentAbility3.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityThreeAttacked(currentAbility3);
		}
	}

	void AbilityFourAttack()
	{
		if (currentAbility4 != null)
		{
			CheckAbilityUpdate();
			outOfCombatCounter = 0f;
			//Debug.Log(currentAbility4);
			abilityController.CurrentAbility4.BaseStats = ability4;
			abilityController.CurrentAbility4.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			abilityController.AbilityFourAttacked(currentAbility4);
		}
	}

	void CheckAbilityUpdate()
	{
		AbilityController.AbilityControllerInstance.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
		if (meleeAttack != null)
		{
			meleeAttack.UpdateStatusEffects();
		}

		if (rangedAttack != null)
		{
			rangedAttack.UpdateStatusEffects();
		}

		if (dash != null)
		{
			dash.UpdateStatusEffects();
		}

		if (ability1 != null)
		{
			ability1.UpdateStatusEffects();
		}

		if (ability2 != null)
		{
			ability2.UpdateStatusEffects();
		}

		if (ability3 != null)
		{
			ability3.UpdateStatusEffects();
		}

		if (ability4 != null)
		{
			ability4.UpdateStatusEffects();
		}
		AbilityController.AbilityControllerInstance.UpdateCoolDown(meleeAttack, rangedAttack, ability1, ability2, ability3, ability4, dash);
	}

	public void TakeDamage(int damage)
	{
		if (GameManager.Instance != null)
		{
			if (!invulnerable && GameManager.Instance.CurrentGameState == GameState.Dungeon)
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
				GameManager.Instance.UiManager.PlayerHitScreenEffect();
				CameraShake.Instance.ShakeCamera(1f, 0.05f);
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
			//Debug.Log("In combat, no boost");
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
				//Debug.Log("Out of combat boost granted");
			}
		}
	}

	void SetSpriteMaskSpriteToPlayerSprite()
	{
		if (flashBlueMask.sprite != playerSprite.sprite)
		{
			flashBlueMask.sprite = playerSprite.sprite;
		}
		if (playerSprite.flipX)
		{
			flashBlueMask.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		}
		else
		{
			flashBlueMask.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void TakeDamage(int damage, int damageType)
	{

	}

	public void GetSlowed(float slowAmount)
	{

	}

	public void GetMarked(int markType, float markHits)
	{

	}

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{

	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isDashing && collision.gameObject.layer == LayerMask.NameToLayer("Breakable Objects"))
		{
			collision.GetComponent<IDamageable>()?.TakeDamage(1);
		}
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

	public IEnumerator GetExpVFX(float flashSpeed)
	{
		Color flashBlueColor = flashBlue.color;
		flashBlueColor.a = 0.6f;
		flashBlue.color = flashBlueColor;
		while (flashBlue.color.a > 0f)
		{
			SetSpriteMaskSpriteToPlayerSprite();
			flashBlueColor = flashBlue.color;
			flashBlueColor.a -= flashSpeed * Time.deltaTime;
			flashBlue.color = flashBlueColor;
			yield return new WaitForEndOfFrame();
		}
		flashBlueColor.a = 0f;
		flashBlue.color = flashBlueColor;
		yield return new WaitForEndOfFrame();
	}

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
		invulnerable = true;
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
