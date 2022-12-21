using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{

	public bool burnAreaUpgrade = false;
	public GameObject burningGround;
	System.Timers.Timer attackTimer = new System.Timers.Timer();
	private List<GameObject> enemyList = new List<GameObject>();
	private Collider2D[] enemiesInBox;
	private bool hitDetecting = false;
	private PlayerControler player;
	private AK.Wwise.Event abilitySound;
	private int comboCounter = 0;
	private float comboTimer = 0f;
	private IEnumerator comboTimerCoroutine;
	private bool thirdHit = false;

	public override void CallAbility(PlayerControler _player)
	{
		if (init)
		{
			init = false;
		}
		SetAbilityStats();
		player = _player;
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PostEventLocal(abilitySound, player.gameObject);
		}
		//player.IsAttackPositionLocked = true;
		player.IsDashing = true;
		Vector2 dashDir = new Vector3( player.Horizontal, player.Vertical ).normalized;
		Rb2d.velocity = dashDir.normalized * baseStats.DashSpeed;
		caller.CallCoroutine(TestCoroutine());


	}

	public void DamageDetectedEnemies(GameObject enemy)
	{
		if (enemyList != null)
		{
			//Debug.Log("Starting enemy damaging");
			if (enemy == null)
			{
				enemyList.Remove(enemy);
				return;
			}
			int damageToDeal = (int)(damage * Random.Range(0.8f, 1.2f));
			if (enemy != null)
			{
				enemy.GetComponent<IDamageable>()?.TakeDamage(damageToDeal + (20 * comboCounter), 0);
				OnHitApplyStatusEffects(enemy.GetComponent<IDamageable>());
			}
			//Debug.Log("Enemy damaged: " + enemy + ", damage: " + damage);
		}
	}

	public void SetAbilityStats()
	{
		boxSize = BaseStats.BoxSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
		AttackTime = BaseStats.AttackTime;
		abilitySound = BaseStats.AbilitySound1;
		statusEffects = BaseStats.statusEffects;
	}

	public void ResetComboTimer()
	{
		if (comboTimerCoroutine != null)
		{
			caller.CancelCoroutine(comboTimerCoroutine);
		}
		comboTimerCoroutine = ComboTimer();
		caller.CallCoroutine(comboTimerCoroutine);
		//Debug.Log("Combo timer has been reset");
	}

	public IEnumerator TestCoroutine()
	{
		ResetComboTimer();

		if (comboCounter == 3)
		{
			comboCounter = 0;
		}

		comboCounter++;

		if (comboCounter < 3)
		{
			thirdHit = false;
			//Debug.Log("Combo: " + comboCounter);
			player.AttackAnimation.GetComponent<SpriteRenderer>().material = player.materialDefault;

			if (comboCounter == 2)
			{
				IAbility anim = new AnimationDecorator(AbilityController.AbilityControllerInstance.CurrentMeleeAttack, "MeleeAttack1", "isAttacking2");
				anim.SetPlayerValues(Rb2d, MousePos, LookDir, CastFromPoint, Angle);
				anim.CallAbility(player);
				//AbilityController.AbilityControllerInstance.CurrentDash.CallAbility(true);
			}
		}
		else
		{
			thirdHit = true;
			//Debug.Log("Full combo!");
			IAbility anim = new AnimationDecorator(AbilityController.AbilityControllerInstance.CurrentMeleeAttack, "MeleeAttack2", "isAttacking3");
			anim.SetPlayerValues(Rb2d, MousePos, LookDir, CastFromPoint, Angle);
			anim.CallAbility(player);
			//AbilityController.AbilityControllerInstance.CurrentDash.CallAbility(true);
			player.AttackAnimation.GetComponent<SpriteRenderer>().material = player.materialHit;
		}

		hitDetecting = true;
		yield return new WaitForFixedUpdate();


		//while ((player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack") || player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttackTwirl")) && player.AnimAttack.GetComponent<AttackAnimationEventHandler>().HitDetection)
		while (player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack") || player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttackTwirl"))
		{
			player.IsAttackPositionLocked = true;

			if (comboCounter == 2)
			{
				player.AttackAnimation.GetComponent<SpriteRenderer>().flipX = LookDir.x > 0 ? false : true;
			}

			if (thirdHit)
			{
				int twirlDir;
				if (player.AttackAnimation.GetComponent<SpriteRenderer>().flipX)
				{
					twirlDir = -1;
				}
				else
				{
					twirlDir = 1;
				}
				player.Pivot_AttackAnimation.transform.Rotate(0f, 0f, 20f * twirlDir);
				if (player.AnimAttack.GetComponent<AttackAnimationEventHandler>().HitDetection)
				{
					enemiesInBox = Physics2D.OverlapBoxAll(Rb2d.transform.position + player.Pivot_AttackAnimation.transform.up * distance, boxSize, player.Pivot_AttackAnimation.transform.rotation.z, layerMask);
				}
			}
			else
			{
				if (player.AnimAttack.GetComponent<AttackAnimationEventHandler>().HitDetection)
				{
					enemiesInBox = Physics2D.OverlapBoxAll(Rb2d.transform.position + CastFromPoint.transform.up * distance, boxSize, Angle, layerMask);
				}
			}

			if (enemiesInBox != null)
			{
				foreach (Collider2D enemy in enemiesInBox)
				{
					if (enemy != null)
					{
						if (!enemyList.Contains(enemy.gameObject))
						{
							enemyList.Add(enemy.gameObject);
							DamageDetectedEnemies(enemy.gameObject);
						}
					}
				}
			}

			//Debug.Log("Detecting Enemies");
			//Debug.Log("Enemies: " + enemyList.Count);

			yield return new WaitForEndOfFrame();
		}
		thirdHit = false;
		player.IsAttackPositionLocked = false;
		enemyList.Clear();
		enemiesInBox = null;
		player.IsDashing = false;
		yield return null;
	}

	public IEnumerator ComboTimer()
	{
		yield return new WaitForSeconds(0.7f);

		comboCounter = 0;
		//Debug.Log("Combo timer ends, combo counter is: " + comboCounter);
	}
}
