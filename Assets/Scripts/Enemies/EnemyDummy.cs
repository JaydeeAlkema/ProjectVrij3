using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : EnemyBase
{
	[SerializeField] private GameObject rewardInstance;
	[SerializeField] private int enemyID;
	[SerializeField] private Ability ability;
	[SerializeField] private AbilityScriptable abilityStats;

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
		GameObject reward = Instantiate(rewardInstance, this.transform.position, Quaternion.identity);
		reward.GetComponent<RewardChoice>().AbilityStats = abilityStats;
		reward.GetComponent<RewardChoice>().AbilityToGive = ability;
		Debug.Log( reward.GetComponent<RewardChoice>().AbilityToGive );
		Destroy(this.gameObject);
	}
}
