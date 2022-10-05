using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable, ICrowdControllable
{
    [SerializeField] private float healthPoints = 0;

	private Vector2 pullPoint = new Vector2();
	private Vector2 vel = new Vector2();
	public bool beingCrowdControlled = false;
	public LayerMask layerMask = default;

	public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public void Update()
	{
		foreach (IStatusEffect statusEffect in statusEffects.ToArray())
		{
			IDamageable damageable = GetComponent<IDamageable>();
			if(statusEffect != null)
			{
				statusEffect.Process(damageable);
			}
		}

		if( healthPoints <= 0 ) { Die(); }

		if( beingCrowdControlled )
		{
			beingDisplaced();
		}
		else
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2( 0, 0 );
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

	public void TakeDamage(float damage)
	{
		healthPoints -= damage;
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		StartCoroutine( FlashColor() );
		if( healthPoints <= 0 ) Die();
	}
	
	public virtual void GetSlowed(float slowAmount)
	{

	}

	public virtual IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.2f);
		this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public virtual void Die()
	{
		Destroy( this.gameObject );
	}

	private void beingDisplaced()
	{
		if( Vector2.Distance( transform.position, pullPoint ) > 0.1f )
		{
			GetComponent<Rigidbody2D>().MovePosition( Vector2.SmoothDamp( transform.position, pullPoint, ref vel, 8f * Time.deltaTime ) );

		}
		else
		{
			beingCrowdControlled = false;
			Physics.IgnoreLayerCollision( layerMask.value, layerMask.value, true );
		}
	}

	public void Pull( Vector2 pullPoint )
	{
		beingCrowdControlled = true;
		Physics.IgnoreLayerCollision( layerMask.value, layerMask.value, false );
		this.pullPoint = pullPoint;
	}
}
