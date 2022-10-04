using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private GameObject projectileAnimation;
	private float counter = 0f;
	[SerializeField] private float lifeSpan;
	public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
	[SerializeField] private TrailRenderer trail = null;
	[SerializeField] private bool trailUpgrade = false;
	public bool TrailUpgrade { get => trailUpgrade; set => trailUpgrade = value; }
	[SerializeField] private float force;
	public float Force { get => force; set => force = value; }
	private float damage;
	public float Damage { get => damage; set => damage = value; }
	[SerializeField] private int typeOfLayer = 6;
	private GameObject addedTrail = null;

	private void Awake()
	{
		if (trailUpgrade)
		{
			addedTrail = Instantiate(trail.gameObject, this.transform.position + transform.up * 0.15f, this.transform.rotation, this.transform);
		}
	}

	private void FixedUpdate()
	{
		projectileAnimation.GetComponent<SpriteRenderer>().flipY = transform.rotation.eulerAngles.z > 180 ? true : false;
		LifeTime(lifeSpan);
		transform.Translate(Vector3.up * force * Time.deltaTime);
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.fixedDeltaTime;
		if (counter >= lifeSpan && GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerProjectile1"))
		{
			GetComponentInChildren<Animator>().Play("PlayerProjectile1_fizzle");
			Destroy(this.gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == typeOfLayer)
		{
			collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (trailUpgrade)
		{
			addedTrail.transform.parent = null;
			addedTrail.GetComponent<TrailRenderer>().autodestruct = true;
			addedTrail.GetComponent<TrailWithTrigger>().enabled = true;
		}
	}
}
