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
	private int burnDamage;
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
	public int BurnDamage { get => burnDamage; set => burnDamage = value; }

	private void Awake()
	{
		
	}

	private void Update()
	{
		projectileAnimation.GetComponent<SpriteRenderer>().flipY = transform.rotation.eulerAngles.z > 180 ? true : false;
		LifeTime(lifeSpan);
		transform.Translate(Vector3.up * force * Time.deltaTime);

		if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerProjectile1_fizzle"))
		{
			Destroy(this.gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length * GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).speed);
		}
	}

	public void TurnOnTrail()
	{
		if( trailUpgrade )
		{
			addedTrail = Instantiate( trail.gameObject, this.transform.position + transform.up * 0.15f, this.transform.rotation, this.transform );
			TrailWithTrigger trailTrigger = addedTrail.GetComponent<TrailWithTrigger>();
			trailTrigger.BurnDamage = burnDamage;
			trailUpgrade = false;
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

	void SpawnProjectileExplosion()
	{
		GameObject aoe = Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
		aoe.GetComponent<PlayerProjectileExplosionDamage>().CastedFrom = castedFrom;
		aoe.GetComponent<PlayerProjectileExplosionDamage>().Damage = damage;
		//aoe.GetComponent<PlayerProjectileExplosionDamage>().Radius = aoeRadius;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Destroy projectile when overlapping with enemies
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == typeOfLayer)
		{
			//collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, 1);
			if (collision.gameObject.GetComponent<IDamageable>() != null)
			{
				//GetComponent<OnHitStatusEffectApply>().OnHitApplyStatusEffects(collision.gameObject.GetComponent<IDamageable>());
				castedFrom.OnHitApplyStatusEffects( collision.gameObject.GetComponent<IDamageable>());
			}
			//Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
			Destroy(this.gameObject);
		}

		//Destroy projectile when overlapping with object on 'Unwalkable' layer
		if (collision.gameObject.layer == 10)
		{
			//Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
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
		SpawnProjectileExplosion();
	}
}
