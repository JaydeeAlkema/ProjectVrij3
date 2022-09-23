using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
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
    private Vector2 boxSize = new Vector2( 4, 6 );
    [SerializeField]
    private float circleSize = 3f;
    [SerializeField]
    private Rigidbody2D rb2d = default;

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
        horizontal = ( int )Input.GetAxisRaw( "Horizontal" );
        vertical = ( int )Input.GetAxisRaw( "Vertical" );

        rb2d.velocity = new Vector3( horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime ).normalized * moveSpeed;
        vel = rb2d.velocity.magnitude;

        MouseLook();
        Debug.DrawRay( rb2d.position, lookDir, Color.magenta );

        if(Input.GetMouseButtonDown(0)) MeleeAttack();
        if( Input.GetMouseButtonDown( 1 ) ) RangedAttack();
        if( Input.GetKeyDown(KeyCode.Q)) AbilityOneAttack();
        if( Input.GetKeyDown( KeyCode.E ) ) AbilityTwoAttack();
        if( Input.GetKeyDown( KeyCode.R ) ) AbilityThreeAttack();
    }

	void MouseLook()
	{
		mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		lookDir = mousePos - rb2d.transform.position;
		angle = Mathf.Atan2( lookDir.y, lookDir.x ) * Mathf.Rad2Deg - 90f;
		castFromPoint.transform.rotation = Quaternion.Euler( 0f, 0f, angle );
	}

	void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS( rb2d.transform.position + castFromPoint.transform.up * 3, castFromPoint.transform.rotation, boxSize );
        Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS( rb2d.transform.position + castFromPoint.transform.up * 5, castFromPoint.transform.rotation, new Vector3( circleSize, circleSize, 0 ) );
        Gizmos.DrawWireSphere( Vector3.zero, 1 );
    }

    void MeleeAttack()
    {
        meleeAttack.Ability.SetScriptable( meleeAttack );
        meleeAttack.Ability.AbilityBehavior();
	}

    void RangedAttack()
    {
        rangedAttack.Ability.SetScriptable( rangedAttack );
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
        meleeAttack.Rb2d = rb2d;
        meleeAttack.CastFromPoint = castFromPoint;
        meleeAttack.MousePos = mousePos;
        meleeAttack.LookDir = lookDir;
        meleeAttack.Angle = angle;
        meleeAttack.Ability.SetScriptable(meleeAttack);
    }
}
