using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkDecorator : AbilityDecorator
{
	public MarkDecorator( IAbility _ability ) : base( _ability )
	{
		if(_ability.ability != null)
		{
			_ability.ability.statusEffects.Add( new StatusEffect_Marked(_ability.BaseStats.markType) );
			//_ability.statusEffects.Add( new StatusEffect_Marked( _ability.MarkType ) );
		}
		//_ability.CallAbility(Player);
	}
}
