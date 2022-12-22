using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	//[SerializeField] private IAbility castedFrom;
	//public IAbility CastedFrom { get => castedFrom; set => castedFrom = value; }
	[SerializeField] private GameObject projectileAnimation;
	private float counter = 0f;
	[SerializeField] private float lifeSpan;
	public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
	//[SerializeField] private TrailRenderer trail = null;
	[SerializeField] private GameObject explosion = null;
	//[SerializeField] private bool trailUpgrade = false;
	//public bool TrailUpgrade { get => trailUpgrade; set => trailUpgrade = value; }
	[SerializeField] private float force;
	public float Force { get => force; set => force = value; }
	[SerializeField] private int damage;
	public int Damage { get => damage; set => damage = value; }
	[SerializeField] private int typeOfLayer = 8;
	//private GameObject addedTrail = null;

	[SerializeField] private AK.Wwise.Event bobProjectileImpact;

	private void Awake()
	{
		
	}

	private void FixedUpdate()
	{
		//if( trailUpgrade )
		//{
		//	addedTrail = Instantiate( trail.gameObject, this.transform.position + transform.up * 0.15f, this.transform.rotation, this.transform );
		//	trailUpgrade = false;
		//}
		projectileAnimation.GetComponent<SpriteRenderer>().flipY = transform.rotation.eulerAngles.z > 180 ? true : false;
		LifeTime(lifeSpan);
		transform.Translate(Vector3.up * force * Time.deltaTime);
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.fixedDeltaTime;
		//if (counter >= lifeSpan && GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerProjectile1"))
		//{
		//	GetComponentInChildren<Animator>().Play("PlayerProjectile1_fizzle");
		//	Destroy(this.gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
		//}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Destroy projectile when overlapping with player
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == typeOfLayer)
		{
			collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
			//if (collision.gameObject.GetComponent<IDamageable>() != null)
			//{
			//	//GetComponent<OnHitStatusEffectApply>().OnHitApplyStatusEffects(collision.gameObject.GetComponent<IDamageable>());
			//	castedFrom.OnHitApplyStatusEffects( collision.gameObject.GetComponent<IDamageable>());
			//}
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
		//if (addedTrail != null)
		//{
		//	addedTrail.transform.parent = null;
		//	addedTrail.GetComponent<TrailRenderer>().autodestruct = true;
		//	addedTrail.GetComponent<TrailWithTrigger>().enabled = true;
		//}
		AudioManager.Instance.PostEventGlobal(bobProjectileImpact);
		Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
	}
}
