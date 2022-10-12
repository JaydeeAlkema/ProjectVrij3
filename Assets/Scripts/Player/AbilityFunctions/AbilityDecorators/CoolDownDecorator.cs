using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownDecorator : AbilityDecorator
{
	private IAbility ability;
	private bool cooledDown = true;
	public IAbility Ability => ability;
	System.Timers.Timer coolDownTimer = new System.Timers.Timer();
	public CoolDownDecorator( IAbility _ability, float _coolDown ) : base( _ability )
	{
		ability = _ability;
		CoolDown = _coolDown;
	}

	public override void CallAbility(PlayerControler _player)
	{
		//base.CallAbility();
		Player = _player;
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		if( cooledDown )
		{
			cooledDown = false;
			Debug.Log( "I got cooled" );
			ability.BaseStats = baseStats;
			ability.SetPlayerValues(Rb2d, MousePos, LookDir, CastFromPoint, Angle);
			ability.CallAbility(Player);
			base.AbilityBehavior();
			//Player.AnimAttack.SetTrigger( "MeleeAttack1" );
			//Player.AnimPlayer.SetTrigger( "isAttacking" );
			Timer();
		}
	}

	private void Timer()
	{
		coolDownTimer.Interval = CoolDown;
		coolDownTimer.Elapsed += OnTimedEvent;
		coolDownTimer.AutoReset = false;
		coolDownTimer.Enabled = true;
	}

	private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
	{
		cooledDown = true;
		coolDownTimer.Enabled = false;
	}
}
