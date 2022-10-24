using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1 : EnemyBase
{
	[SerializeField] private GameObject mobPrefab;
	[SerializeField] PlayerHealthBar healthBar;
	[SerializeField] private float maxHealthPoints = 1000;
	[SerializeField] private float agitateTime = 5f;
	[SerializeField] private float respawnTime = 15f;
	[SerializeField] private SpriteRenderer bodyCharged;
	[SerializeField] private SpriteRenderer coreCharged;

	private Color baseColor;
	public float agitateCounter = 0f;
	public float respawnCounter = 0f;
	public float spawnTimeMultiplier = 1f;

	public List<GameObject> mobs = new List<GameObject>();
	public Transform[] innerSpawnPoints;
	public Transform[] outerSpawnPoints;
	public bool respawning = false;

	private void Start()
	{
		HealthPoints = maxHealthPoints;
		if (healthBar != null)
		{
			healthBar.SetMaxHP(maxHealthPoints);
		}

		baseColor = this.GetComponent<SpriteRenderer>().color;

		SpawnMobs();
	}

	private void FixedUpdate()
	{
		base.Update();
		healthBar.SetHP(HealthPoints);
		AgitateTimer();
		MobListUpdate();
		if (mobs.Count < (innerSpawnPoints.Length + outerSpawnPoints.Length) && !respawning)
		{
			SpawnTimer();
		}
	}

	public void SpawnMobs()
	{
		foreach (Transform spawnPoint in innerSpawnPoints)
		{
			GameObject spawnedMob = Instantiate(mobPrefab, spawnPoint.position, Quaternion.identity);
			spawnedMob.GetComponent<MBCirclingEnemy>().center = transform.position;
			mobs.Add(spawnedMob);
		}

		foreach (Transform spawnPoint in outerSpawnPoints)
		{
			GameObject spawnedMob = Instantiate(mobPrefab, spawnPoint.position, Quaternion.identity);
			MBCirclingEnemy spawnedScript = spawnedMob.GetComponent<MBCirclingEnemy>();
			spawnedScript.center = transform.position;
			spawnedScript.orbitRadius = 5f;
			spawnedScript.rotationSpeed = spawnedScript.rotationSpeed * -1f;
			mobs.Add(spawnedMob);
		}
	}

	public void MobListUpdate()
	{
		foreach(GameObject mob in mobs)
		{
			if(mob == null)
			{
				spawnTimeMultiplier += 0.2f;
				mobs.Remove(mob);
			}
		}
	}

	public void SpawnTimer()
	{
		respawnCounter += spawnTimeMultiplier * Time.fixedDeltaTime;

		Color coreColor = coreCharged.color;
		coreColor.a = respawnCounter / respawnTime;
		coreCharged.color = coreColor;

		if (respawnCounter >= respawnTime)
		{
			respawnCounter = 0f;
			coreColor.a = 0f;
			coreCharged.color = coreColor;
			StartCoroutine(SpawnSequence());
		}
	}

	public void AgitateTimer()
	{
		agitateCounter += Time.fixedDeltaTime;

		Color chargeColor = bodyCharged.color;
		chargeColor.a = agitateCounter / agitateTime;
		bodyCharged.color = chargeColor;

		if (agitateCounter >= agitateTime)
		{
			foreach (GameObject mob in mobs)
			{
				if (mob != null)
				{
					MBCirclingEnemy mobScript = mob.GetComponent<MBCirclingEnemy>();
					if (mobScript.agitated)
					{
						mobScript.aggro = true;
						mobScript.agitated = false;
					}
				}
				else
				{
					mobs.Remove(mob);
				}
			}
			agitateCounter = 0f;
			Debug.Log("ATTACK!!!");
		}
	}

	IEnumerator SpawnSequence()
	{
		respawning = true;
		foreach (GameObject mob in mobs)
		{
			if (mob != null)
			{
				Destroy(mob);
			}
		}
		mobs.Clear();
		spawnTimeMultiplier = 1f;
		yield return new WaitForSeconds(1f);
		SpawnMobs();
		respawning = false;
		yield return null;
	}

	public override IEnumerator FlashColor()
	{
		yield return new WaitForSeconds(0.08f);
		this.GetComponent<SpriteRenderer>().color = baseColor;
	}
}
