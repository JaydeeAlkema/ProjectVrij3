using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private IAbility castedFrom;
	public IAbility CastedFrom { get => castedFrom; set => castedFrom = value; }
	[SerializeField] private GameObject projectileAnimation;
	private float counter = 0f;
	[SerializeField] private float lifeSpan;
	public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
	[SerializeField] private TrailRenderer trail = null;
	[SerializeField] private GameObject explosion = null;
	[SerializeField] private bool trailUpgrade = false;
	public bool TrailUpgrade { get => trailUpgrade; set => trailUpgrade = value; }
	[SerializeField] private float force;
	public float Force { get => force; set => force = value; }
	private int damage;
	public int Damage { get => damage; set => damage = value; }
	[SerializeField] private int typeOfLayer = 6;
	private GameObject addedTrail = null;

	private void Awake()
	{
		
	}

	private void FixedUpdate()
	{
		if( trailUpgrade )
		{
			addedTrail = Instantiate( trail.gameObject, this.transform.position + transform.up * 0.15f, this.transform.rotation, this.transform );
			trailUpgrade = false;
		}
		projectileAnimation.GetComponent<SpriteRenderer>().flipY = transform.rotation.eulerAngles.z > 180 ? true : false;
		LifeTime(lifeSpan);
		transform.Translate(Vector3.up * force * Time.deltaTime);

		if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerProjectile1_fizzle"))
		{
			Destroy(this.gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length * GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).speed);
		}
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.fixedDeltaTime;
		if (counter >= lifeSpan && GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerProjectile1"))
		{
			GetComponentInChildren<Animator>().Play("PlayerProjectile1_fizzle");
			
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Destroy projectile when overlapping with enemies
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == typeOfLayer)
		{
			collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, 1);
			if (collision.gameObject.GetComponent<IDamageable>() != null)
			{
				//GetComponent<OnHitStatusEffectApply>().OnHitApplyStatusEffects(collision.gameObject.GetComponent<IDamageable>());
				castedFrom.OnHitApplyStatusEffects( collision.gameObject.GetComponent<IDamageable>());
			}
			Destroy(this.gameObject);
		}

		//Destroy projectile when overlapping with object on 'Unwalkable' layer
		if (collision.gameObject.layer == 10)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (addedTrail != null)
		{
			addedTrail.transform.parent = null;
			addedTrail.GetComponent<TrailRenderer>().autodestruct = true;
			addedTrail.GetComponent<TrailWithTrigger>().enabled = true;
		}
		Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
	}
}
