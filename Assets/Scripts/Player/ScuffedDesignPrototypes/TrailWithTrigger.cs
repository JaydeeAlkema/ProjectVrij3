using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailWithTrigger : MonoBehaviour
{

	TrailRenderer fireTrail;
	EdgeCollider2D trailCollider;

	[SerializeField] private float timer = 0f;
	[SerializeField] private float timeSpawnNew;
	[SerializeField] private GameObject burningGround;

	private int burnDamage;
	public int BurnDamage { get => burnDamage; set => burnDamage = value; }

	void Start()
	{
		//fireTrail = this.GetComponent<TrailRenderer>();

		//GameObject colliderGameObject = new GameObject("TrailCollider", typeof(EdgeCollider2D), typeof(OnTriggerStatusEffectApply));
		//trailCollider = colliderGameObject.GetComponent<EdgeCollider2D>();
		//trailCollider.isTrigger = true;
		//trailCollider.edgeRadius = 0.25f;
		//colliderGameObject.GetComponent<OnTriggerStatusEffectApply>().statusEffectType = StatusEffectType.Burn;
		//colliderGameObject.GetComponent<OnTriggerStatusEffectApply>().SetBurnValue( burnDamage );
	}

	void FixedUpdate()
	{
		//SetTriggerPointsFromTrail(fireTrail, trailCollider);

		if (timer < timeSpawnNew)
		{
			timer += Time.deltaTime;
		}
		else
		{
			SpawnBurningGround();
			timer = 0f;
		}

	}

	void SpawnBurningGround()
	{
		GameObject ground = Instantiate(burningGround, transform.position, Quaternion.identity);
		ground.GetComponent<OnTriggerStatusEffectApply>().BurnDamage = burnDamage;
	}

	//void SetTriggerPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
	//{
	//	List<Vector2> points = new List<Vector2>();
	//	for(int position = 0; position<trail.positionCount; position++)
	//	{
	//		points.Add(trail.GetPosition(position));
	//	}
	//	collider.SetPoints(points);
	//}

	void OnDestroy()
	{
		SpawnBurningGround();
		//if (trailCollider == null) return;
		//Destroy(trailCollider.gameObject);
	}
}
