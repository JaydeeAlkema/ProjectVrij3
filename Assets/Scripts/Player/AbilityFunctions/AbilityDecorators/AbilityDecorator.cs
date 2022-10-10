using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDecorator : IAbility
{
	private IAbility ability;

	public AbilityDecorator(IAbility _ability)
	{
		ability = _ability;
	}

	public virtual void AbilityBehavior()
	{
		ability.AbilityBehavior();
	}
}
