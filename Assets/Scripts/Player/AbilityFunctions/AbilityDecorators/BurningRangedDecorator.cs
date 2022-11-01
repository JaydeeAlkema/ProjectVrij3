using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningRangedDecorator : AbilityDecorator
{
	private IAbility ability;
	private bool upgrade;
	public BurningRangedDecorator( IAbility _ability, bool upgraded ) : base( _ability )
	{
		ability = _ability;
		upgrade = upgraded;
	}
	public override void AbilityBehavior()
	{
		ability.TrailUpgrade = upgrade;
		//ability.CallAbility( ability.Player );
	}

}
