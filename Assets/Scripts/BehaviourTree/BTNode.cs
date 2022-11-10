using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
	public enum BTNodeState
	{
		RUNNING,
		SUCCESS,
		FAILURE
	}
	public class BTNode
	{
		public string name;

		protected BTNodeState state;

		public BTNode parent;
		protected List<BTNode> children = new List<BTNode>();

		private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

		public BTNode()
		{
			parent = null;
		}
		public BTNode(List<BTNode> children)
		{
			foreach (BTNode child in children)
			{
				_Attach(child);
			}
				
		}

		private void _Attach(BTNode bTNode)
		{
			bTNode.parent = this;
			children.Add(bTNode);
		}

		public virtual BTNodeState Evaluate() => BTNodeState.FAILURE;

		public void SetData(string key, object value)
		{
			_dataContext[key] = value;
		}

		public object GetData(string key)
		{
			object value = null;
			if (_dataContext.TryGetValue(key, out value))
			{
				return value;
			}
				

			BTNode bTNode = parent;
			while (bTNode != null)
			{
				value = bTNode.GetData(key);
				if (value != null)
				{
					return value;
				}
				bTNode = bTNode.parent;
			}
			return null;
		}

		public bool ClearData(string key)
		{
			object value = null;
			if (_dataContext.ContainsKey(key))
			{
				_dataContext.Remove(key);
				return true;
			}

			BTNode bTNode = parent;
			while (bTNode != null)
			{
				bool cleared = bTNode.ClearData(key);
				if (cleared)
				{
					return true;
				}
				bTNode = bTNode.parent;
			}
			return false;
		}

	}

}