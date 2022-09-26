using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private float counter = 0f;
	public float lifeSpan;
	public TrailRenderer trail = null;
	private float force;
	public float Force { get => force; set => force = value; }

	private void Awake()
	{
		if( GetComponentInChildren<TrailRenderer>() != null )
		{
			trail = GetComponentInChildren<TrailRenderer>();
		}
	}

	private void FixedUpdate()
	{
		LifeTime( lifeSpan );
		transform.Translate( transform.up * force * Time.fixedDeltaTime );
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.fixedDeltaTime;
		if (counter >= lifeSpan)
		{
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.layer == 7 || collision.gameObject.layer == 6)
		{
			collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(20f);
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if( trail != null )
		{
			trail.transform.parent = null;
			trail.autodestruct = true;
			trail.GetComponent<TrailWithTrigger>().enabled = true;
		}
	}
}
