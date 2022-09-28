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
	private float damage;
	public float Damage { get => damage; set => damage = value; }
	[SerializeField] private int typeOfLayer = 6;

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
		transform.Translate( Vector3.up * force * Time.fixedDeltaTime, Space.Self );
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.fixedDeltaTime;
		if (counter >= lifeSpan)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( collision.gameObject.layer == 7 || collision.gameObject.layer == typeOfLayer )
		{
			collision.gameObject.GetComponent<IDamageable>()?.TakeDamage( damage );
			Destroy( this.gameObject );
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
