using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedGround : MonoBehaviour
{
	public int damage;
	public bool isErupting = false;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private SpriteRenderer sprite;
	private Animator animator;

	private void Start()
	{
		animator = sprite.gameObject.GetComponent<Animator>();
		GetComponent<CircleCollider2D>().radius = 0.125f * transform.localScale.x;
	}

	public void StartEruption()
	{
		if (!isErupting)
		{
			StartCoroutine(Eruption());
		}
	}

	public void EruptionHitbox()
	{
		Collider2D playerInArea = Physics2D.OverlapCircle(transform.position, 0.5f * transform.localScale.x, layerMask); //Add boss layer later maybe to damage orb? Test!
		if (playerInArea != null)
		{
			int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
			playerInArea.GetComponent<IDamageable>()?.TakeDamage(damageToDeal);
		}
	}

	public void EruptionDone()
	{
		Destroy(this.gameObject);
	}

	IEnumerator Eruption()
	{
		isErupting = true;

		animator.SetTrigger("StartBuildup");

		yield return new WaitForSeconds(0.5f);

		animator.SetTrigger("StartEruption");

		yield return new WaitForEndOfFrame();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
	}

}
