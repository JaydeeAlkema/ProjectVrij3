using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
	public class Sequence : BTNode
	{
		public Sequence() : base() { }
		public Sequence(List<BTNode> children) : base(children) { }

		public override BTNodeState Evaluate()
		{
			bool anyChildIsRunning = false;

			foreach (BTNode btNode in children)
			{
				switch (btNode.Evaluate())
				{
					case BTNodeState.FAILURE:
						state = BTNodeState.FAILURE;
						return state;
					case BTNodeState.SUCCESS:
						continue;
					case BTNodeState.RUNNING:
						anyChildIsRunning = true;
						continue;
					default:
						state = BTNodeState.SUCCESS;
						return state;
				}
			}

			state = anyChildIsRunning ? BTNodeState.RUNNING : BTNodeState.SUCCESS;
			return state;
		}
	}
}
