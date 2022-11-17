using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class GetCrowdControlled : BTNode
{
	private EnemyBase enemyScript;
	public GetCrowdControlled( EnemyBase _enemyScript )
	{
		enemyScript = _enemyScript;
	}

	public override BTNodeState Evaluate()
	{
		if( enemyScript.beingCrowdControlled )
		{
			Debug.Log( "i got pulled" );
			ClearData("dashDestination");
			ClearData( "dashDir" );
			ClearData( "target" );
			ClearData( "ready" );
			enemyScript.beingDisplaced();
			state = BTNodeState.RUNNING;
			return state;
		}
		enemyScript.beingCrowdControlled = false;
		state = BTNodeState.FAILURE;
		return state;
	}
}
