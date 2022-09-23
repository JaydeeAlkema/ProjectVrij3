using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private float counter = 0f;
	public float lifeSpan;
	public TrailRenderer trail = null;

	private void Awake()
	{
		if (GetComponentInChildren<TrailRenderer>() == null) return;
		trail = GetComponentInChildren<TrailRenderer>();
	}

	void Update()
	{
		LifeTime(lifeSpan);
	}

	void LifeTime(float lifeSpan)
	{
		counter += Time.deltaTime;
		if (counter >= lifeSpan)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		trail.transform.parent = null;
		trail.autodestruct = true;
		trail.GetComponent<TrailWithTrigger>().enabled = true;
	}
}
