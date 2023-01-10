using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDecorator : AbilityDecorator
{
	private IAbility ability;
	private string attackTrigger;
	private string playerTrigger;
	private float scaling;
	public AnimationDecorator( IAbility _ability, string attackAnim, string playerAnim, float sizeScale) : base( _ability )
	{
		ability = _ability;
		attackTrigger = attackAnim;
		playerTrigger = playerAnim;
		scaling = sizeScale;
	}

	public override void CallAbility( PlayerControler _player )
	{
		//base.CallAbility();
		Player = _player;
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		if( attackTrigger != "" ) { Player.AnimAttack.transform.localScale = Vector3.Scale( Player.AttackAnimBaseScale , new Vector3(scaling,scaling,1)); Player.AnimAttack.SetTrigger( attackTrigger ); }
		if(playerTrigger != "") { Player.AnimPlayer.SetTrigger( playerTrigger ); }
	}
}
