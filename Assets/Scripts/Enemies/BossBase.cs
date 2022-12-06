using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : EnemyBase
{

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

}
