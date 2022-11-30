using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class BossClearAttackSequence : BTNode
{
	private List<string> listOfDataToClear;

	public BossClearAttackSequence(List<string> listOfDataToClear)
	{
		this.listOfDataToClear = listOfDataToClear;
	}

	public override BTNodeState Evaluate()
	{
		foreach(string dataToClear in listOfDataToClear)
		{
			Debug.Log("Now clear data: " + dataToClear);
			ClearData(dataToClear);
		}

		state = BTNodeState.SUCCESS;
		return state;
	}

}
