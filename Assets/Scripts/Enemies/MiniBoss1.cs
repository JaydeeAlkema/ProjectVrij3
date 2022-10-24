using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1 : EnemyBase
{
	[SerializeField] PlayerHealthBar healthBar;
	[SerializeField] private float maxHealthPoints = 1000;
	[SerializeField] private float agitateTime = 5f;
	[SerializeField] private SpriteRenderer bodyCharged;
	private Color baseColor;
	private float counter = 0f;

	public GameObject[] addMobs;

	private void Start()
	{
		HealthPoints = maxHealthPoints;
		if (healthBar != null)
		{
			healthBar.SetMaxHP(maxHealthPoints);
		}

		baseColor = this.GetComponent<SpriteRenderer>().color;
	}

	private void Update()
	{
		base.Update();
		healthBar.SetHP(HealthPoints);
		AgitateTimer();
	}

	public void AgitateTimer()
	{
		counter += Time.fixedDeltaTime;

		Color chargeColor = bodyCharged.color;
		chargeColor.a = counter / agitateTime;
		bodyCharged.color = chargeColor;

		if (counter >= agitateTime)
		{
			foreach (GameObject addMob in addMobs)
			{
				if (addMob != null)
				{
					MBCirclingEnemy mobScript = addMob.GetComponent<MBCirclingEnemy>();
					if (mobScript.agitated)
					{
						mobScript.aggro = true;
						mobScript.agitated = false;
					}
				}
			}
			counter = 0f;
			Debug.Log("ATTACK!!!");
		}
	}

	public override IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.08f);
		this.GetComponent<SpriteRenderer>().color = baseColor;
	}
}
