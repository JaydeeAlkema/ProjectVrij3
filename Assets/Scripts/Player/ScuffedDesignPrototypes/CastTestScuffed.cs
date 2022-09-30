using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTestScuffed : MonoBehaviour
{
	public GameObject projectilePrefab;
	public Transform castFromPoint;
	public float projectileLifeSpan = 10f;
	public float projectileForce = 30f;

	void Start()
	{

	}

	void Update()
	{
		if (Input.GetButtonDown("Fire2"))
		{
			Shoot();
		}
	}

	void Shoot()
	{
		GameObject Projectile = Instantiate(projectilePrefab, castFromPoint.transform.position, castFromPoint.transform.rotation);
		Rigidbody2D rbProjectile = Projectile.GetComponent<Rigidbody2D>();
		rbProjectile.GetComponent<Projectile>().LifeSpan = projectileLifeSpan;
		rbProjectile.AddForce(castFromPoint.up * projectileForce, ForceMode2D.Impulse);
	}
}
