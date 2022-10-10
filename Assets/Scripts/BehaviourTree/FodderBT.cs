using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class FodderBT : BTTree
{
	public Rigidbody2D rb2d;
	public static float speed = 150f;
	protected override BTNode SetupTree()
	{
		BTNode root = new TaskIdle(rb2d);
		return root;
	}


}
