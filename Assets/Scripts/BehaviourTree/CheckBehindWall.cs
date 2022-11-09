using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CheckBehindWall : BTNode
{

	public CheckBehindWall()
	{

	}

	public override BTNodeState Evaluate()
	{
		object hitWallPoint = GetData("hitWallPoint");
		if (hitWallPoint == null)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		if (hitWallPoint != null || (Vector2)hitWallPoint != Vector2.zero)
		{
			ClearData("hitWallPoint");
			state = BTNodeState.FAILURE;
			return state;
		}
		state = BTNodeState.SUCCESS;
		return state;
	}

}
