using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClassTest : MonoBehaviour, ICrowdControllable
{
    private Vector2 pullPoint = new Vector2();
    private Vector2 vel = new Vector2();
    public bool beingCrowdControlled = false;
    public LayerMask layerMask = default;

	public void Pull(Vector2 pullPoint)
	{
        beingCrowdControlled = true;
        Physics.IgnoreLayerCollision(layerMask.value, layerMask.value, false);
        this.pullPoint = pullPoint;
	}

    void Update()
    {
        if (beingCrowdControlled)
        {
            beingDisplaced();
        }
        
    }

    private void beingDisplaced()
	{
        if (Vector2.Distance(transform.position, pullPoint) > 0.1f)
        {
            GetComponent<Rigidbody2D>().MovePosition(Vector2.SmoothDamp(transform.position, pullPoint, ref vel, 8f * Time.deltaTime));
 
		}
		else
		{
            beingCrowdControlled = false;
            Physics.IgnoreLayerCollision(layerMask.value, layerMask.value, true);
        }
    }
}
