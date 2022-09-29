using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour , IDamageable
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
	private Vector2 lookDir;
	[SerializeField]
	private Transform castFromPoint;
	[SerializeField]
	private Vector2 boxSize = new Vector2(4, 6);
	[SerializeField]
	private float circleSize = 3f;
	[SerializeField]
	private Rigidbody2D rb2d = default;
	[SerializeField] SpriteRenderer Sprite;
	[SerializeField] GameObject Pivot_AttackAnimation;
	[SerializeField] GameObject AttackAnimation;
	[SerializeField] Animator animAttack;
	[SerializeField] Animator animPlayer;
	private bool isAttacking = false;

    [SerializeField] private float healthPoints = 500;

    [Header("Abilities")]
	#region ability fields
	[SerializeField]
	private AbilityScriptable meleeAttack;
	[SerializeField]
	private AbilityScriptable rangedAttack;
	[SerializeField]
	private AbilityScriptable ability1;
	[SerializeField]
	private AbilityScriptable ability2;
	[SerializeField]
	private AbilityScriptable ability3;
	#endregion


	// Start is called before the first frame update
	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{
		CheckAbilityUpdate();
		horizontal = (int)Input.GetAxisRaw("Horizontal");
		vertical = (int)Input.GetAxisRaw("Vertical");

		rb2d.velocity = new Vector3(horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime).normalized * moveSpeed;
		vel = rb2d.velocity.magnitude;

		MouseLook();

		Sprite.flipX = lookDir.x > 0 ? true : false;
		AttackAnimation.GetComponent<SpriteRenderer>().flipX = lookDir.x > 0 ? true : false;

		Debug.DrawRay(rb2d.position, lookDir, Color.magenta);

		if (Input.GetMouseButtonDown(0)) MeleeAttack();
		if (Input.GetMouseButtonDown(1)) RangedAttack();
		if (Input.GetKeyDown(KeyCode.Q)) AbilityOneAttack();
		if (Input.GetKeyDown(KeyCode.E)) AbilityTwoAttack();
		if (Input.GetKeyDown(KeyCode.R)) AbilityThreeAttack();
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

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = Matrix4x4.TRS(rb2d.transform.position + castFromPoint.transform.up * 3, castFromPoint.transform.rotation, boxSize);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.TRS(rb2d.transform.position + castFromPoint.transform.up * 5, castFromPoint.transform.rotation, new Vector3(circleSize, circleSize, 0));
		Gizmos.DrawWireSphere(Vector3.zero, 1);
	}

	void MeleeAttack()
	{
		meleeAttack.Ability.SetScriptable(meleeAttack);
		meleeAttack.Ability.AbilityBehavior();
		animAttack.SetTrigger("MeleeAttack1");
		animPlayer.SetTrigger("isAttacking");
	}

	void RangedAttack()
	{
		rangedAttack.Ability.SetScriptable(rangedAttack);
		rangedAttack.Ability.AbilityBehavior();
	}

	void AbilityOneAttack()
	{

	}

	void AbilityTwoAttack()
	{

	}

	void AbilityThreeAttack()
	{

	}

	void CheckAbilityUpdate()
	{
		if (meleeAttack != null)
		{
			meleeAttack.Rb2d = rb2d;
			meleeAttack.CastFromPoint = castFromPoint;
			meleeAttack.MousePos = mousePos;
			meleeAttack.LookDir = lookDir;
			meleeAttack.Angle = angle;
			meleeAttack.Ability.SetScriptable(meleeAttack);
		}

		if (rangedAttack != null)
		{
			rangedAttack.Rb2d = rb2d;
			rangedAttack.CastFromPoint = castFromPoint;
			rangedAttack.MousePos = mousePos;
			rangedAttack.LookDir = lookDir;
			rangedAttack.Angle = angle;
			rangedAttack.Ability.SetScriptable(rangedAttack);
		}

		if (ability1 != null)
		{
			ability1.Rb2d = rb2d;
			ability1.CastFromPoint = castFromPoint;
			ability1.MousePos = mousePos;
			ability1.LookDir = lookDir;
			ability1.Angle = angle;
			ability1.Ability.SetScriptable(rangedAttack);
		}

		if (ability2 != null)
		{
			ability2.Rb2d = rb2d;
			ability2.CastFromPoint = castFromPoint;
			ability2.MousePos = mousePos;
			ability2.LookDir = lookDir;
			ability2.Angle = angle;
			ability2.Ability.SetScriptable(rangedAttack);
		}

        if( ability3 != null )
        {
            ability3.Rb2d = rb2d;
            ability3.CastFromPoint = castFromPoint;
            ability3.MousePos = mousePos;
            ability3.LookDir = lookDir;
            ability3.Angle = angle;
            ability3.Ability.SetScriptable( rangedAttack );
        }
    }

	public void TakeDamage( float damage )
	{
        healthPoints -= damage;
	}

	public void ApplyStatusEffect( IStatusEffect statusEffect )
	{

	}

	public void RemoveStatusEffect( IStatusEffect statusEffect )
	{

	}
}
