using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownDecorator : AbilityDecorator
{
	private IAbility ability;
	public IAbility Ability => ability;
	public CoolDownDecorator( IAbility _ability ) : base( _ability )
	{
	}
}
