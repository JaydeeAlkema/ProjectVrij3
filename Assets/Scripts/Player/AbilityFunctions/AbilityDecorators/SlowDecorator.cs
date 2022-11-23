using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDecorator : AbilityDecorator
{
	public SlowDecorator( IAbility _ability ) : base( _ability )
	{
		_ability.statusEffects.Add( new StatusEffect_Slow( _ability.SlowAmount, _ability.SlowDuration ) );
		//_ability.CallAbility( Player );
	}
}
