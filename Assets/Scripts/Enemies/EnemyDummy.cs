using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : EnemyBase
{
	[SerializeField] private GameObject rewardInstance;

	private void Start()
	{
		this.GetComponent<SpriteRenderer>().color = Color.green;
	}

	public override IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.08f);
		this.GetComponent<SpriteRenderer>().color = Color.green;
	}

	public override void Die()
	{
		Instantiate(rewardInstance, this.transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}
}
