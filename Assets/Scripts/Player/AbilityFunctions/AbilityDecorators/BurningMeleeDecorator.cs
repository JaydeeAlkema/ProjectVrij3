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
		Debug.Log( "i got decorated with fire" );
		ability = _ability;
		baseStats = _baseStats;
		burnObject = baseStats.BurnObject;
		//rb2d = baseStats.Rb2d;
		//castFromPoint = baseStats.CastFromPoint;
		distance = baseStats.Distance;
		//AbilityBehavior();
	}


	public override void AbilityBehavior()
	{
		Debug.Log( "i want to burn" );
		//base.AbilityBehavior();
		//ability.AbilityBehavior();
		for( int i = 0; i < 3; i++ )
		{
			Object.Instantiate( burnObject, rb2d.transform.position + castFromPoint.transform.right * ( i - 1 ) + castFromPoint.transform.up * distance, Quaternion.identity );
			Debug.Log( "burn instantiated" );
		}
	}
}
