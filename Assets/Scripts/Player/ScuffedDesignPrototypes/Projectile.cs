using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private float counter = 0f;
	[SerializeField] private float lifeSpan;
	public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
	[SerializeField] private TrailRenderer trail = null;
	[SerializeField] private bool trailUpgrade = false;
	public bool TrailUpgrade { get => trailUpgrade; set => trailUpgrade = value; }
	private float force;
	public float Force { get => force; set => force = value; }
	private float damage;
	public float Damage { get => damage; set => damage = value; }
	[SerializeField] private int typeOfLayer = 6;

	private void Awake()
	{
		if(trailUpgrade)
		{
			Instantiate( trail.gameObject, this.transform.position, this.transform.rotation, this.transform );
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
		if( trailUpgrade )
		{
			trail.transform.parent = null;
			trail.autodestruct = true;
			trail.GetComponent<TrailWithTrigger>().enabled = true;
		}
	}
}
