using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour, IDamageable
{
	private int horizontal = 0;
	private int vertical = 0;
	[SerializeField]
	private float moveSpeed = 1;
	[SerializeField]
	private float vel = 0;

	private Vector3 mousePos;
	private float angle;
	//[SerializeField]
	//private Camera cam;
	public Vector2 lookDir; // <-- private this after Dash is implemented correctly!
	[SerializeField]
	private Transform castFromPoint;
	[SerializeField]
	private Vector2 boxSize = new Vector2( 4, 6 );
	[SerializeField]
	private float circleSize = 3f;
	[SerializeField]
	private Rigidbody2D rb2d = default;
	[SerializeField] SpriteRenderer Sprite;
	[SerializeField] GameObject Pivot_AttackAnimation;
	[SerializeField] GameObject AttackAnimation;
	[SerializeField] Animator animAttack;
	public Animator AnimAttack { get => animAttack; set => animAttack = value; }
	[SerializeField] Animator animPlayer;

	[SerializeField] PlayerHealthBar healthBar;

	//Temporary, remove when implementing real death screen
	[SerializeField] Transform deathScreenTest;
	private bool isDying = false;

	public Animator AnimPlayer { get => animPlayer; set => animPlayer = value; }
	private bool isAttacking = false;

	public float dashSpeed = 100f;
	public float dashDuration = 0.2f;
	private bool isDashing = false;
	public TrailRenderer dashTrail;

	[SerializeField] private float maxHealthPoints = 500;
	[SerializeField] private float currentHealthPoints;
	[SerializeField] private AbilityController abilityController;

	[Header( "Abilities" )]
	#region ability fields
	[SerializeField] private AbilityScriptable meleeAttack;
	[SerializeField] private AbilityScriptable rangedAttack;
	[SerializeField] private AbilityScriptable ability1;
	public AbilityScriptable Ability1 { get => ability1; set => ability1 = value; }
	[SerializeField] private AbilityScriptable ability2;
	public AbilityScriptable Ability2 { get => ability2; set => ability2 = value; }
	[SerializeField] private AbilityScriptable ability3;
	public AbilityScriptable Ability3 { get => ability3; set => ability3 = value; }

	private IAbility currentMeleeAttack = new MeleeAttack();
	private IAbility currentRangedAttack = new RangedAttack();
	private IAbility currentAbility1;
	private IAbility currentAbility2;
	private IAbility currentAbility3;
	public IAbility CurrentMeleeAttack { get => currentMeleeAttack; set => currentMeleeAttack = value; }
	public IAbility CurrentRangedAttack { get => currentRangedAttack; set => currentRangedAttack = value; }
	public IAbility CurrentAbility1 { get => currentAbility1; set => currentAbility1 = value; }
	public IAbility CurrentAbility2 { get => currentAbility2; set => currentAbility2 = value; }
	public IAbility CurrentAbility3 { get => currentAbility3; set => currentAbility3 = value; }
	#endregion


	// Start is called before the first frame update
	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		dashTrail.emitting = false;
		//currentMeleeAttack = new CoolDownDecorator( currentMeleeAttack, meleeAttack.CoolDown );
		currentMeleeAttack.BaseStats = meleeAttack;
		abilityController.CurrentMeleeAttack = currentMeleeAttack;
		currentRangedAttack.BaseStats = rangedAttack;
		abilityController.CurrentRangedAttack = currentRangedAttack;
		if( currentAbility1 != null ) { currentAbility1.BaseStats = ability1; abilityController.CurrentAbility1 = currentAbility1; }
		if( currentAbility2 != null ) { currentAbility2.BaseStats = ability2; abilityController.CurrentAbility2 = currentAbility2; }
		if( currentAbility3 != null ) { currentAbility3.BaseStats = ability3; abilityController.CurrentAbility3 = currentAbility3; }
		abilityController.SetAttacks();

		currentHealthPoints = maxHealthPoints;
		if (healthBar != null)
		{
			healthBar.SetMaxHP(maxHealthPoints);
		}

		//Death screen test, remove later
		if (deathScreenTest != null)
		{
			deathScreenTest.gameObject.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update()
	{
		CheckAbilityUpdate();


		if (!isDashing)
		{
			horizontal = (int)Input.GetAxisRaw("Horizontal");
			vertical = (int)Input.GetAxisRaw("Vertical");
			rb2d.velocity = new Vector3(horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime).normalized * moveSpeed;
		}

		vel = rb2d.velocity.magnitude;

		MouseLook();

		Dash();

		Sprite.flipX = lookDir.x > 0 ? true : false;
		AttackAnimation.GetComponent<SpriteRenderer>().flipX = lookDir.x > 0 ? true : false;

		Debug.DrawRay(rb2d.position, lookDir, Color.magenta);

		if (Input.GetMouseButtonDown(0)) MeleeAttack();
		if (Input.GetMouseButtonDown(1)) RangedAttack();
		if (Input.GetKeyDown(KeyCode.Q)) AbilityOneAttack();
		if (Input.GetKeyDown(KeyCode.E)) AbilityTwoAttack();
		if (Input.GetKeyDown(KeyCode.R)) AbilityThreeAttack();
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

	void Dash()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
		{
			StartCoroutine(Dashing(new Vector3(horizontal, vertical).normalized, dashDuration));
		}
	}

	public IEnumerator Dashing(Vector2 dashDir, float dashDuration)
	{
		isDashing = true;
		rb2d.velocity = dashDir.normalized * dashSpeed;
		dashTrail.emitting = true;
		yield return new WaitForSeconds(dashDuration);
		isDashing = false;
		dashTrail.emitting = false;
	}

	void MouseLook()
	{
		if (animAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack"))
		{
			isAttacking = true;
		}
		else
		{
			isAttacking = false;
		}

		if (!isAttacking)
		{
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lookDir = mousePos - rb2d.transform.position;
			angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
			castFromPoint.transform.rotation = Quaternion.Euler(0f, 0f, angle);
			Pivot_AttackAnimation.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180);
		}
	}


	void MeleeAttack()
	{
		abilityController.CurrentMeleeAttack.BaseStats = meleeAttack;
		abilityController.CurrentMeleeAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
		abilityController.MeleeAttacked( currentMeleeAttack ).CallAbility(this);
	}

	void RangedAttack()
	{
		abilityController.CurrentRangedAttack.BaseStats = rangedAttack;
		abilityController.CurrentRangedAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
		abilityController.RangeAttacked( currentRangedAttack ).CallAbility( this );
		//animPlayer.SetTrigger("isAttacking");
	}

	void AbilityOneAttack()
	{
		//ability1.Ability.SetPlayerValues(ability1);
		ability1.Ability.AbilityBehavior();
	}

	void AbilityTwoAttack()
	{
		//ability2.Ability.SetPlayerValues(ability2);
		ability2.Ability.AbilityBehavior();
	}

	void AbilityThreeAttack()
	{
		//ability3.Ability.SetPlayerValues(ability3);
		ability3.Ability.AbilityBehavior();
	}

	void CheckAbilityUpdate()
	{
		if (meleeAttack != null)
		{
			meleeAttack.Start();
			abilityController.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
		}

		if (rangedAttack != null)
		{
			rangedAttack.Start();
			abilityController.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
		}

		if (ability1 != null)
		{
			//ability1.Rb2d = rb2d;
			//ability1.CastFromPoint = castFromPoint;
			//ability1.MousePos = mousePos;
			//ability1.LookDir = lookDir;
			//ability1.Angle = angle;
			//ability1.Ability.SetScriptable();
		}

		if (ability2 != null)
		{
			//ability2.Rb2d = rb2d;
			//ability2.CastFromPoint = castFromPoint;
			//ability2.MousePos = mousePos;
			//ability2.LookDir = lookDir;
			//ability2.Angle = angle;
			//ability2.Ability.SetScriptable();
		}

		if (ability3 != null)
		{
			//ability3.Rb2d = rb2d;
			//ability3.CastFromPoint = castFromPoint;
			//ability3.MousePos = mousePos;
			//ability3.LookDir = lookDir;
			//ability3.Angle = angle;
			//ability3.Ability.SetScriptable(  );
		}
		abilityController.UpdateCoolDown(meleeAttack, rangedAttack, ability1, ability2, ability3);
	}

	public void TakeDamage(float damage)
	{
		currentHealthPoints -= damage;
		healthBar.SetHP(currentHealthPoints);
		if (currentHealthPoints <= 0 && !isDying) Die();
	}

	public void TakeDamage(float damage, int damageType)
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

	void Die()
	{
		isDying = true;
		StartCoroutine(DeathSequence());
		Debug.Log("I HAVE DIED OH NO");
	}

	void Respawn()
	{
		HubSceneManager.sceneManagerInstance.ChangeScene("Hub Prototype", SceneManager.GetActiveScene().name);
	}

	IEnumerator DeathSequence()
	{
		Time.timeScale = 0f;    //Hitstop
		yield return new WaitForSecondsRealtime(1f);
		Sprite.gameObject.SetActive(false);
		Time.timeScale = 1f;
		yield return new WaitForSecondsRealtime(1.5f);

		//Deathscreen test
		deathScreenTest.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(3f);

		Respawn();
		isDying = false;
		yield return null;
	}
}
