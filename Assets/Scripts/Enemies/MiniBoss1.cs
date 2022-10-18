using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1 : EnemyBase
{

	private Color baseColor;
	[SerializeField] PlayerHealthBar healthBar;
	[SerializeField] private float maxHealthPoints = 1000;

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
	}

	public override IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.08f);
		this.GetComponent<SpriteRenderer>().color = baseColor;
	}
}
