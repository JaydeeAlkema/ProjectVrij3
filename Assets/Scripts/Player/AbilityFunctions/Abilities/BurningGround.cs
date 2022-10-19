using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGround : MonoBehaviour
{
	private float counter = 0f;
	public float lifeSpan = 4f;
	void Update()
    {
		Lifetime();
    }

    void Lifetime()
	{
		counter += Time.fixedDeltaTime;
		if (counter >= lifeSpan)
		{
			Destroy(this.gameObject);
		}
	}
}
