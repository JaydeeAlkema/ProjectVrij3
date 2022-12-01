using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkDecorator : AbilityDecorator
{
	public MarkDecorator( IAbility _ability ) : base( _ability )
	{
		_ability.BaseStats.statusEffects.Add( new StatusEffect_Marked( _ability.BaseStats.markType ) );
	}
}
