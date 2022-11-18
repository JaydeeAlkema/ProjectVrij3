using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
	private bool init = true;
	public bool burnAreaUpgrade = false;
	public GameObject burningGround;
	System.Timers.Timer attackTimer = new System.Timers.Timer();
	private List<Collider2D> enemyList = new List<Collider2D>();
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
		player = _player;
		if (init)
		{
			SetAbilityStats();
			init = false;
		}
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		AudioManager.Instance.PostEventLocal(abilitySound, player.gameObject);
		//player.IsAttackPositionLocked = true;
		caller.CallCoroutine(TestCoroutine());

	}

	public void DamageDetectedEnemies(Collider2D enemy)
	{
		if (enemyList != null)
		{
			//Debug.Log("Starting enemy damaging");
			enemy.GetComponent<IDamageable>()?.TakeDamage(damage + (20 * comboCounter), 0);
			OnHitApplyStatusEffects(enemy.GetComponent<IDamageable>());
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
	}

	public void ResetComboTimer()
	{
		if (comboTimerCoroutine != null)
		{
			caller.CancelCoroutine(comboTimerCoroutine);
		}
		comboTimerCoroutine = ComboTimer();
		caller.CallCoroutine(comboTimerCoroutine);
		Debug.Log("Combo timer has been reset");
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
			Debug.Log("Combo: " + comboCounter);
			player.AttackAnimation.GetComponent<SpriteRenderer>().material = player.materialDefault;
		}
		else
		{
			thirdHit = true;
			Debug.Log("Full combo!");
			IAbility anim = new AnimationDecorator(AbilityController.AbilityControllerInstance.CurrentMeleeAttack, "MeleeAttack2", "isAttacking");
			anim.SetPlayerValues(Rb2d, MousePos, LookDir, CastFromPoint, Angle);
			anim.CallAbility(player);
			AbilityController.AbilityControllerInstance.CurrentDash.CallAbility(true);
			player.AttackAnimation.GetComponent<SpriteRenderer>().material = player.materialHit;
		}

		hitDetecting = true;
		yield return new WaitForFixedUpdate();

		while (player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack") || player.AnimAttack.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttackTwirl"))
		{
			player.IsAttackPositionLocked = true;

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
				enemiesInBox = Physics2D.OverlapBoxAll(Rb2d.transform.position + player.Pivot_AttackAnimation.transform.up * distance, boxSize, player.Pivot_AttackAnimation.transform.rotation.z, layerMask);
			}
			else
			{
				enemiesInBox = Physics2D.OverlapBoxAll(Rb2d.transform.position + CastFromPoint.transform.up * distance, boxSize, Angle, layerMask);
			}


			foreach (Collider2D enemy in enemiesInBox)
			{
				if (!enemyList.Contains(enemy))
				{
					enemyList.Add(enemy);
					DamageDetectedEnemies(enemy);
				}
			}
			//Debug.Log("Detecting Enemies");
			//Debug.Log("Enemies: " + enemyList.Count);

			yield return new WaitForEndOfFrame();
		}
		thirdHit = false;
		player.IsAttackPositionLocked = false;
		enemyList.Clear();
		yield return null;
	}

	public IEnumerator ComboTimer()
	{
		yield return new WaitForSeconds(3f);

		comboCounter = 0;
		Debug.Log("Combo timer ends, combo counter is: " + comboCounter);
	}
}
