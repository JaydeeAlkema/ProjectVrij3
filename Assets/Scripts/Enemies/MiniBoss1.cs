using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1 : BossBase
{
	[SerializeField] private GameObject player = null;
	[SerializeField] private GameObject mobPrefab;
	[SerializeField] private float innerRadius;
	[SerializeField] private float innerOrbitRotationSpeed;
	[SerializeField] private float outerRadius;
	[SerializeField] private float outerOrbitRotationSpeed;
	public Vector2[] innerSpawnPoints;
	public Vector2[] outerSpawnPoints;

	[SerializeField] private GameObject shockwaveVFXPrefab;

	[SerializeField] private GameObject rewardInstance;

	private void Start()
	{
		base.Start();
		player = FindObjectOfType<PlayerControler>()?.gameObject;
	}

	private void Update()
	{
		MobListUpdate();
		LookAtTarget();
	}

	public override void SpawnMobs()
	{
		foreach (Vector2 spawnPoint in innerSpawnPoints)
		{
			GameObject spawnedMob = Instantiate(mobPrefab, spawnPoint + (Vector2)transform.position, Quaternion.identity);
			MBCirclingEnemy spawnedScript = spawnedMob.GetComponent<MBCirclingEnemy>();
			spawnedScript.enabled = true;
			spawnedScript.Boss = this.gameObject;
			spawnedScript.center = transform.position;
			spawnedScript.orbitRadius = innerRadius;
			spawnedScript.rotationSpeed = innerOrbitRotationSpeed;
			mobs.Add(spawnedMob);
		}

		foreach (Vector2 spawnPoint in outerSpawnPoints)
		{
			GameObject spawnedMob = Instantiate(mobPrefab, spawnPoint + (Vector2)transform.position, Quaternion.identity);
			MBCirclingEnemy spawnedScript = spawnedMob.GetComponent<MBCirclingEnemy>();
			spawnedScript.enabled = true;
			spawnedScript.Boss = this.gameObject;
			spawnedScript.center = transform.position;
			spawnedScript.orbitRadius = outerRadius;
			spawnedScript.rotationSpeed = outerOrbitRotationSpeed;
			mobs.Add(spawnedMob);
		}
	}

	public void MobListUpdate()
	{
		GameObject[] checkMobs = mobs.ToArray();
		foreach (GameObject mob in checkMobs)
		{
			if (mob == null)
			{
				mobs.Remove(mob);
			}
		}
	}

	public void DoShockWave()
	{
		GameObject shockwave = Instantiate(shockwaveVFXPrefab, transform.position, Quaternion.identity);
		shockwave.GetComponent<ShockwaveVFX>().Damage = damage;
	}

	public IEnumerator LaunchMobs()
	{
		GameObject[] mobsToFire = mobs.ToArray();
		for (int i = 0; i < mobsToFire.Length; i++)
		{
			if (mobsToFire[i] != null)
			{
				MBCirclingEnemy mobScript = mobsToFire[i].GetComponent<MBCirclingEnemy>();
				mobScript.aggro = true;
				if (player != null)
				{
					mobScript.LaunchDestination = player.transform.position;
				}
				yield return new WaitForSeconds(0.2f);
			}
		}
		yield return new WaitForEndOfFrame();
	}

}
