using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedGround : MonoBehaviour
{
	public int damage;
	public bool isErupting = false;
	[SerializeField] private LayerMask layerMask;

	public void StartEruption()
	{
		if (!isErupting)
		{
			StartCoroutine(Eruption());
		}
	}

	public void EruptionHitbox()
	{
		Collider2D playerInArea = Physics2D.OverlapCircle(transform.position, transform.localScale.x * 0.5f, layerMask); //Add boss layer later maybe to damage orb? Test!
		if (playerInArea != null)
		{
			int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
			playerInArea.GetComponent<IDamageable>()?.TakeDamage(damageToDeal);
		}
		Destroy(this.gameObject, 1f);
	}

	IEnumerator Eruption()
	{
		isErupting = true;
		SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
		sprite.color = Color.green;

		yield return new WaitForSeconds(0.5f);

		EruptionHitbox();

		yield return new WaitForEndOfFrame();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
	}

}
