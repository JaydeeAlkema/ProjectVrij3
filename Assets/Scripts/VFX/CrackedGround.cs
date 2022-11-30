using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedGround : MonoBehaviour
{

	public bool isErupting = false;

	public void StartEruption()
	{
		if (!isErupting)
		{
			StartCoroutine(Eruption());
		}
	}

	public void EruptionHitbox()
	{
		//AOE damage to player
		Destroy(this.gameObject);
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

}
