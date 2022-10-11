using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownDecorator : AbilityDecorator
{
	private IAbility ability;
	private float coolDown;
	private bool cooledDown = true;
	public IAbility Ability => ability;
	System.Timers.Timer coolDownTimer = new System.Timers.Timer();
	public CoolDownDecorator( IAbility _ability, float _coolDown ) : base( _ability )
	{
		ability = _ability;
		coolDown = _coolDown;
	}

	public override void CallAbility()
	{
		//base.CallAbility();
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		if( cooledDown )
		{
			cooledDown = false;
			Debug.Log( "I got cooled" );
			ability.BaseStats = baseStats;
			ability.SetPlayerValues(rb2d, mousePos, lookDir, castFromPoint, angle);
			ability.CallAbility();
			base.AbilityBehavior();
			Timer();
		}
	}

	private void Timer()
	{
		coolDownTimer.Interval = coolDown;
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
