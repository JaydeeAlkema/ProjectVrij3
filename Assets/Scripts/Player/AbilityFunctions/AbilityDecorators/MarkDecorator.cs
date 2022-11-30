using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkDecorator : AbilityDecorator
{
	public MarkDecorator( IAbility _ability ) : base( _ability )
	{
		_ability.ability.statusEffects.Add( new StatusEffect_Marked( _ability.ability.MarkType ) );
		if( _ability.ability != null )
		{
			//_ability.statusEffects.Add( new StatusEffect_Marked( _ability.MarkType ) );
		}
		//_ability.CallAbility(Player);
	}
}
