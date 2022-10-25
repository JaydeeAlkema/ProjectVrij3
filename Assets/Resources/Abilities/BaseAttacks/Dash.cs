using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
	private bool init = true;
	System.Timers.Timer coolDownTimer = new System.Timers.Timer();

	public override void CallAbility( PlayerControler _player )
	{
		Player = FindObjectOfType<PlayerControler>();
		Player.IsDashing = true;
		Player.Trail.emitting = true;
		if( init )
		{
			SetAbilityStats();
			init = false;
		}
	}

	public override void AbilityBehavior()
	{
		Timer();
		Vector2 dashDir = new Vector3( Player.Horizontal, Player.Vertical ).normalized;
		Rb2d.velocity = dashDir.normalized * baseStats.DashSpeed;
	}

	private void Timer()
	{
		coolDownTimer.Elapsed += OnTimedEvent;
		coolDownTimer.Interval = baseStats.DashDuration * 1000;
		coolDownTimer.AutoReset = false;
		coolDownTimer.Enabled = true;
	}

	private void OnTimedEvent( object source, System.Timers.ElapsedEventArgs e )
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
