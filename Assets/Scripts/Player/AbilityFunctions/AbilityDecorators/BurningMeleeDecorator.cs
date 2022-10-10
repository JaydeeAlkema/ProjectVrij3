using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningMeleeDecorator : AbilityDecorator
{
	private IAbility m_ability;
	public IAbility ability => m_ability;

	[SerializeField] private GameObject burnObject;
	private Rigidbody2D rb2d;
	private Transform castFromPoint;
	private float distance;
	public BurningMeleeDecorator( IAbility _ability, GameObject _burnObject, Rigidbody2D _rb2d, Transform _castFromPoint, float _distance ) : base( _ability ) 
	{
		Debug.Log( "i got decorated with fire" );
		m_ability = _ability;
		burnObject = _burnObject;
		rb2d = _rb2d;
		castFromPoint = _castFromPoint;
		distance = _distance;
		AbilityBehavior();
	}

	public override void AbilityBehavior()
	{
		m_ability.AbilityBehavior();
		for( int i = 0; i < 3; i++ )
		{
			Object.Instantiate( burnObject, rb2d.transform.position + castFromPoint.transform.right * ( i - 1 ) + castFromPoint.transform.up * distance, Quaternion.identity );
			Debug.Log( "burn instantiated" );
		}
	}
}
