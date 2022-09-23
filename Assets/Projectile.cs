using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float counter = 0f;
    public float lifeSpan;

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
}
