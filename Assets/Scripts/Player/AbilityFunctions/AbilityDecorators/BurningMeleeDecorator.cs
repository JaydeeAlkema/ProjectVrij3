using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningMeleeDecorator : AbilityDecorator
{
	private IAbility ability;
	public IAbility Ability => ability;

	private GameObject burnObject;
	private float distance;
	public BurningMeleeDecorator( IAbility _ability, AbilityScriptable _baseStats ) : base( _ability ) 
	{
		ability = _ability;
		baseStats = _baseStats;
		burnObject = baseStats.BurnObject;
		distance = baseStats.Distance;
	}


	public override void AbilityBehavior()
	{
		for( int i = 0; i < 3; i++ )
		{
			GameObject burnGround = Object.Instantiate( burnObject, Rb2d.transform.position + CastFromPoint.transform.right * ( i - 1 ) + CastFromPoint.transform.up * distance, Quaternion.identity );
			burnGround.GetComponent<OnTriggerStatusEffectApply>().BurnDamage = ability.BurnDamage;
			burnGround.GetComponent<OnTriggerStatusEffectApply>().UpdateStatusEffects();
			Debug.Log( "burn instantiated" );
		}
	}
}
