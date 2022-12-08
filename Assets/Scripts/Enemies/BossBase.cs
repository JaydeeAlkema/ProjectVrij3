using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : EnemyBase
{

	[SerializeField] private SpriteRenderer weakSpotSprite;
	public List<GameObject> mobs = new List<GameObject>();

	private void Start()
	{
		base.Start();
	}

	private void Update()
	{
		base.Update();
	}


	public virtual void SpawnMobs()
	{

	}


	public override IEnumerator FlashColor()
	{
		weakSpotSprite.material = MaterialHit;
		yield return new WaitForSeconds(0.09f);
		weakSpotSprite.material = MaterialDefault;
	}

}
