using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDecorator : AbilityDecorator
{
	private IAbility ability;
	private string attackTrigger;
	private string playerTrigger;
	public AnimationDecorator( IAbility _ability, string attackAnim, string playerAnim) : base( _ability )
	{
		ability = _ability;
		attackTrigger = attackAnim;
		playerTrigger = playerAnim;
	}

	public override void CallAbility( PlayerControler _player )
	{
		//base.CallAbility();
		Player = _player;
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		if( attackTrigger != "" ) {Player.AnimAttack.SetTrigger( attackTrigger ); }
		if(playerTrigger != "") { Player.AnimPlayer.SetTrigger( playerTrigger ); }
	}
}
