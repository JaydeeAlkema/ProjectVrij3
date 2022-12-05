using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniBoss1 : BossBase
{
	[SerializeField] private GameObject player = null;
	[SerializeField] Transform healthBar;
	[SerializeField] private int maxHealthPoints;
	[SerializeField] private GameObject mobPrefab;
	[SerializeField] private float innerRadius;
	[SerializeField] private float innerOrbitRotationSpeed;
	[SerializeField] private float outerRadius;
	[SerializeField] private float outerOrbitRotationSpeed;
	[SerializeField] private BoxCollider2D bodyBlock;
	[SerializeField] private float shockwaveExpansionSpeed;
	private float storedSpeed;
	public Vector2[] innerSpawnPoints;
	public Vector2[] outerSpawnPoints;
	public GameObject spawnedShockwaveObject = null;

	[SerializeField] private GameObject shockwaveVFXPrefab;

	[SerializeField] private GameObject rewardInstance;

	private void Start()
	{
		base.Start();
		maxHealthPoints = HealthPoints;
		storedSpeed = Speed;
		if (healthBar != null)
		{
			healthBar.GetComponent<Slider>().maxValue = maxHealthPoints;
			healthBar.GetComponent<Slider>().value = maxHealthPoints;
		}
		player = FindObjectOfType<PlayerControler>()?.gameObject;
		GameManager.Instance.SetupNonDungeon("Boss Testing");
	}

	private void Update()
	{
		base.Update();
		healthBar.GetComponent<Slider>().value = HealthPoints;
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

	public override void LookAtTarget()
	{
		if (DestinationSetter.target != null && enemySprite != null)
		{
			enemySprite.flipX = (DestinationSetter.GetComponent<Pathfinding.IAstarAI>().destination - transform.position).normalized.x > 0 ? true : false;
		}
		float flippedValue;
		flippedValue = enemySprite.flipX ? -1f : 1f;
		GetComponent<BoxCollider2D>().offset = new Vector2(Mathf.Abs(GetComponent<BoxCollider2D>().offset.x) * flippedValue, GetComponent<BoxCollider2D>().offset.y);
		bodyBlock.offset = new Vector2(Mathf.Abs(bodyBlock.offset.x) * -flippedValue, bodyBlock.offset.y);
	}

	public void SpawnShockWaveObject(float radius)
	{
		spawnedShockwaveObject = Instantiate(shockwaveVFXPrefab, transform.position - new Vector3(0f, 3f, 0f), Quaternion.identity);
		ShockwaveVFX shockwaveScript = spawnedShockwaveObject.GetComponent<ShockwaveVFX>();
		shockwaveScript.Damage = damage;
		shockwaveScript.MaxRadius = new Vector3(radius, radius, radius);
		shockwaveScript.ExpansionSpeed = shockwaveExpansionSpeed;
	}

	public void ShockWave()
	{
		if (spawnedShockwaveObject != null)
		{
			spawnedShockwaveObject.GetComponent<ShockwaveVFX>().DrawShockwave();
		}
	}

	public void SmallShockWave(float radius)
	{
		GameObject shockwave = Instantiate(shockwaveVFXPrefab, transform.position - new Vector3(0f, 3f, 0f), Quaternion.identity);
		ShockwaveVFX shockwaveScript = shockwave.GetComponent<ShockwaveVFX>();
		shockwaveScript.Damage = damage;
		shockwaveScript.MaxRadius = new Vector3(radius, radius, radius);
		shockwaveScript.ExpansionSpeed = shockwaveExpansionSpeed;
		shockwave.GetComponent<ShockwaveVFX>().DrawShockwave();
	}

	public void StandStill(bool isStill)
	{
		if (isStill)
		{
			Speed = 0f;
		}
		else
		{
			Speed = storedSpeed;
		}
	}

	public override void OnHitVFX()
	{
		Instantiate(vfxHitSpark, (Vector2)transform.position + GetComponent<BoxCollider2D>().offset, Quaternion.Euler(0, 0, Random.Range(0, 360)));
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
					mobScript.LaunchDestination = player.transform.position + ((player.transform.position - transform.position).normalized * Random.Range(2f, 5f));
				}
				yield return new WaitForSeconds(0.2f);
			}
		}
		yield return new WaitForEndOfFrame();
	}

}
