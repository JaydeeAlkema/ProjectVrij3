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
			Debug.Log("We have minions, proceed.");
			state = BTNodeState.SUCCESS;
			return state;
		}
		Debug.Log("We DO NOT have minions, going to spawn them.");
		state = BTNodeState.FAILURE;
		return state;
	}

}
