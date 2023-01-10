using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
	private bool init = true;
	System.Timers.Timer coolDownTimer = new System.Timers.Timer();

	public override void CallAbility(PlayerControler _player)
	{
		Player = Object.FindObjectOfType<PlayerControler>();
		Player.IsDashing = true;
		if (init)
		{
			SetAbilityStats();
			init = false;
		}
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		Timer();
		Player.DashParticlesVFX.Play();
		Vector2 dashDir = new Vector3(Player.Horizontal, Player.Vertical).normalized;
		float dashAngle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg;
		Debug.Log("Dash angle: " + dashAngle);
		string dashAnimation = null;
		if (dashAngle <= 45 && dashAngle >= -45)
		{
			dashAnimation = "isDashSide";
			Player.PlayerSprite.flipX = true;
		}
		if (dashAngle >= 135 || dashAngle <= -135)
		{
			dashAnimation = "isDashSide";
			Player.PlayerSprite.flipX = false;
		}
		if (dashAngle > -135 && dashAngle < -45)
		{
			dashAnimation = "isDashDown";
			//player rotation here
		}
		if (dashAngle > 45 && dashAngle < 135)
		{
			dashAnimation = "isDashUp";
			//player rotation here
		}
		if (dashAnimation != null)
		{
			IAbility anim = new AnimationDecorator(AbilityController.AbilityControllerInstance.CurrentMeleeAttack, "", dashAnimation);
			anim.SetPlayerValues(Rb2d, MousePos, LookDir, CastFromPoint, Angle);
			anim.CallAbility(Player);
		}
		
		Player.Pivot_DashAnimation.transform.rotation = Quaternion.Euler(0f, 0f, dashAngle + 270);
		Player.DashVFX.GetComponent<Animator>().SetTrigger("doDashTrail");
		Rb2d.velocity = dashDir.normalized * baseStats.DashSpeed;
	}

	private void Timer()
	{
		coolDownTimer.Elapsed += OnTimedEvent;
		coolDownTimer.Interval = baseStats.DashDuration * 1000;
		coolDownTimer.AutoReset = false;
		coolDownTimer.Enabled = true;
		if(coolDownTimer.Enabled == true) { Player.IsDashing = true; }
	}

	private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
	{
		coolDownTimer.Enabled = false;
		Player.IsDashing = false;
	}

	public void SetAbilityStats()
	{
		boxSize = BaseStats.BoxSize;
		layerMask = BaseStats.Layer;
		damage = BaseStats.Damage;
		distance = BaseStats.Distance;
		coolDown = BaseStats.CoolDown;
	}
}
