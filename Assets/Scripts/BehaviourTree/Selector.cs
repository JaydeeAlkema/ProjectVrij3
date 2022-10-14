using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
	public class Selector : BTNode
	{
		public Selector() : base() { }
		public Selector(List<BTNode> children) : base(children) { }

		public override BTNodeState Evaluate()
		{ 
			foreach (BTNode btNode in children)
			{
				switch (btNode.Evaluate())
				{
					case BTNodeState.FAILURE:
						continue;
					case BTNodeState.SUCCESS:
						state = BTNodeState.SUCCESS;
						return state;
					case BTNodeState.RUNNING:
						state = BTNodeState.RUNNING;
						return state;
					default:
						continue;
				}
			}

			state = BTNodeState.FAILURE;
			return state;
		}
	}
}
