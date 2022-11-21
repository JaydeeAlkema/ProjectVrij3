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
		//object hitWallPoint = GetData("hitWallPoint");
		object wallcheck = GetData("hitWall");
		if (wallcheck == null)
		{
			state = BTNodeState.FAILURE;
			return state;
		}

		bool hitWall = (bool)GetData("hitWall");

		if (hitWall)
		{
			//ClearData("hitWallPoint");
			ClearData("hitWall");
			state = BTNodeState.FAILURE;
			return state;
		}
		state = BTNodeState.SUCCESS;
		return state;
	}

}
