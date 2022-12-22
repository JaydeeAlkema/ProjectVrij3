using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : EnemyBase
{

	[SerializeField] private SpriteRenderer weakSpotSprite;
	[SerializeField] protected bool invulnerable = false;
	protected Animator weakspotAnimator;
	public List<GameObject> mobs = new List<GameObject>();

	public SpriteRenderer WeakSpotSprite { get => weakSpotSprite; set => weakSpotSprite = value; }
	public Animator WeakspotAnimator { get => weakspotAnimator; set => weakspotAnimator = value; }

	protected void Start()
	{
		base.Start();
		weakspotAnimator = weakSpotSprite.gameObject.GetComponent<Animator>();
	}

	protected void Update()
	{
		base.Update();
	}

	protected override void ListenToEvents()
	{
		//do not listen
	}

	public virtual void SpawnMobs()
	{

	}

	public override void TakeDamage(int damage, int damageType)
	{
		int damageToTake = damage;
		if (!invulnerable)
		{
			if (damageType == 0 && meleeTarget)
			{
				HealthPoints -= ((int)(damage * markHits));
				meleeTarget = false;
				meleeMarkDecal.SetActive(false);
				damageToTake = ((int)(damageToTake * markHits));
				damageToTake += damage;
			}

			if (damageType == 1 && castTarget)
			{
				HealthPoints -= ((int)(damage * markHits));
				castTarget = false;
				castMarkDecal.SetActive(false);
				damageToTake = ((int)(damageToTake * markHits));
				damageToTake += damage;
			}

			if (damageType == 0)
			{
				AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
				if (Time.timeScale == 1f)
				{
					StartCoroutine(HitStop(0.02f));
				}
			}

			if (damageType == 1)
			{
				AkSoundEngine.PostEvent("npc_dmg_cast", this.gameObject);
			}

			OnHitVFX();
			Debug.Log("i took " + damage + " damage");
			DamagePopup(damageToTake);
			HealthPoints -= damage;
			StopCoroutine(FlashColor());
			StartCoroutine(FlashColor());
			if (HealthPoints <= 0) Die();
		}
		else
		{
			if (damageType == 0 && meleeTarget)
			{
				meleeTarget = false;
				meleeMarkDecal.SetActive(false);
			}

			if (damageType == 1 && castTarget)
			{
				castTarget = false;
				castMarkDecal.SetActive(false);
			}

			if (damageType == 0)
			{
				AkSoundEngine.PostEvent("npc_dmg_melee", this.gameObject);
			}

			if (damageType == 1)
			{
				AkSoundEngine.PostEvent("npc_dmg_cast", this.gameObject);
			}

			DamagePopup(0);
		}
	}

	public override IEnumerator FlashColor()
	{
		Color orbColor = weakSpotSprite.color;
		orbColor.a = 1f;
		weakSpotSprite.color = orbColor;
		yield return new WaitForSeconds(0.2f);
		while (orbColor.a > 0 && !invulnerable)
		{
			orbColor = weakSpotSprite.color;
			orbColor.a -= 10f * Time.deltaTime;
			weakSpotSprite.color = orbColor;
			yield return new WaitForEndOfFrame();
		}
		orbColor.a = 0f;
		weakSpotSprite.color = orbColor;
	}

}
