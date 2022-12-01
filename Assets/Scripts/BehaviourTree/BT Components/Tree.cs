using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BTTree : MonoBehaviour
    {

        private BTNode _root = null;

		protected void Start()
		{
			_root = SetupTree();
		}

		private void Update()
		{
			if (_root != null)
			{
				_root.Evaluate();
			}
		}

		protected abstract BTNode SetupTree();
	}
}

