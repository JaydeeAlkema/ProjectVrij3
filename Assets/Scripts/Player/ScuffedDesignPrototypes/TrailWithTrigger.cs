using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailWithTrigger : MonoBehaviour
{

	TrailRenderer fireTrail;
	EdgeCollider2D trailCollider;

	void Awake()
	{
		fireTrail = this.GetComponent<TrailRenderer>();

		GameObject colliderGameObject = new GameObject("TrailCollider", typeof(EdgeCollider2D));
		trailCollider = colliderGameObject.GetComponent<EdgeCollider2D>();
		trailCollider.isTrigger = true;
		trailCollider.edgeRadius = 0.25f;
		trailCollider.gameObject.AddComponent<OnTriggerStatusEffectApply>();
		trailCollider.GetComponent<OnTriggerStatusEffectApply>().statusEffectType = StatusEffectType.Burn;
	}

	void Update()
	{
		SetTriggerPointsFromTrail(fireTrail, trailCollider);
	}

	void SetTriggerPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
	{
		List<Vector2> points = new List<Vector2>();
		for(int position = 0; position<trail.positionCount; position++)
		{
			points.Add(trail.GetPosition(position));
		}
		collider.SetPoints(points);
	}

	void OnDestroy()
	{
		if (trailCollider == null) return;
		Destroy(trailCollider.gameObject);
	}
}
