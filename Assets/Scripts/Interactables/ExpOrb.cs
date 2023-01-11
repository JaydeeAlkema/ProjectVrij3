using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class ExpOrb : MonoBehaviour
{
	[SerializeField] private GameObject vfxPickupParticles;
	[SerializeField] private TrailRenderer expOrbTrail;
	[SerializeField] private float pickupRange;
	[SerializeField] private float flySpeed;
	[SerializeField] private int expToGive;
	[SerializeField] private float scatterTime;
	private float timer = 0f;
	private Vector2 scatterDestination;
	private bool canBePickedUp = false;
	public LayerMask layerMask;

	private void Start()
	{
		scatterDestination = (Vector2)transform.position + new Vector2(Random.Range(0.5f, 2f), Random.Range(0.5f, 2f));
		expOrbTrail.enabled = false;
	}

	void Update()
	{
		TimeBeforePickup();

		if (!canBePickedUp)
		{
			ScatterOnSpawn();
		}
		else
		{
			CheckIfPlayerIsInOrbRange();
		}
	}

	void TimeBeforePickup()
	{
		if (timer < scatterTime)
		{
			timer += Time.deltaTime;
		}
		else
		{
			canBePickedUp = true;
		}
	}

	void ScatterOnSpawn()
	{
		if (Vector2.Distance(transform.position, scatterDestination) > 0.1f)
		{
			transform.position = Vector2.MoveTowards(transform.position, scatterDestination, flySpeed * Time.deltaTime);
		}
	}

	void CheckIfPlayerIsInOrbRange()
	{
		Collider2D playerInArea = Physics2D.OverlapCircle(transform.position, pickupRange, layerMask); //Add boss layer later maybe to damage orb? Test!
		if (playerInArea != null)
		{
			expOrbTrail.enabled = true;
			OrbFlyTowardsPlayer(playerInArea.gameObject);

			if (Vector2.Distance(transform.position, playerInArea.transform.position) < 0.2f)
			{
				PlayerPickupOrb(playerInArea.gameObject);
			}
		}
	}

	void OrbFlyTowardsPlayer(GameObject player)
	{
		transform.position = Vector2.MoveTowards(transform.position, player.transform.position, flySpeed * Time.deltaTime);
	}

	void PlayerPickupOrb(GameObject player)
	{
		GameManager.Instance.ExpManager.AddExp(expToGive);
		GameObject vfxPickup = Instantiate(vfxPickupParticles, player.transform.position, Quaternion.identity);
		ParticleSystem particles = vfxPickup.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule particlesMain = particles.main;
		particlesMain.startSpeedMultiplier = 25 + 0.5f * expToGive;
		ParticleSystem.Burst particleBurst = particles.emission.GetBurst(0);
		particleBurst.count = 7 + expToGive;
		vfxPickup.GetComponent<ParticleSystem>().emission.SetBurst(0, particleBurst);
		PlayerControler playerScript = player.GetComponent<PlayerControler>();
		playerScript.StopCoroutine(playerScript.GetExpVFX(1f));
		playerScript.StartCoroutine(playerScript.GetExpVFX(1f));
		Destroy(this.gameObject);
	}
}
