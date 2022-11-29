using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BossCheckMobs : BTNode
{

	private BossBase bossScript;

	public BossCheckMobs(BossBase bossScript)
	{
		this.bossScript = bossScript;
	}

	public override BTNodeState Evaluate()
	{
		if (bossScript.mobs != null)
		{
			state = BTNodeState.SUCCESS;
			return state;
		}
		state = BTNodeState.FAILURE;
		return state;
	}

}
