using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningRangedDecorator : AbilityDecorator
{
	private IAbility ability;
	private bool upgrade;

	public override void CallAbility( PlayerControler _player )
	{
		Player = _player;
		AbilityBehavior();
	}

	public BurningRangedDecorator( IAbility _ability, bool upgraded ) : base( _ability )
	{
		ability = _ability;
		upgrade = upgraded;
	}
	public override void AbilityBehavior()
	{
		ability.TrailUpgrade = upgrade;
		Debug.Log( "decorater bool is " + upgrade );
		Debug.Log( "ability bool should be " + ability.TrailUpgrade );
		//ability.CallAbility( ability.Player );
	}

}
