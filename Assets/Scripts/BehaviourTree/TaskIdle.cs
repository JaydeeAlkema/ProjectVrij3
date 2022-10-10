using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
public class TaskIdle : BTNode
{
	private Rigidbody2D _rb2d;

	private float _waitTime = 1f;
	private float _moveTime = 1f;
	private float _waitCounter = 0f;
	private float _moveCounter = 0f;
	private bool _waiting = false;
	private bool _moving = false;
	public Vector2 moveDir;

	public TaskIdle(Rigidbody2D rb2d)
	{
		_rb2d = rb2d;
	}

	public override BTNodeState Evaluate()
	{
		Debug.Log("Waiting: " + _waiting + ", Moving: " + _moving);
		if (_waiting)
		{
			_rb2d.velocity = new Vector2(0, 0);
			_waitCounter += Time.deltaTime;
			if (_waitCounter >= _waitTime)
			{
				_waiting = false;
				_moveCounter = 0f;
				_moving = true;
				moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			}
		}
		else
		{
			if (_moving)
			{
				_rb2d.velocity = moveDir.normalized * FodderBT.speed * Time.deltaTime;

				_moveCounter += Time.deltaTime;
				if (_moveCounter >= _moveTime)
				{
					_moving = false;
					_waitCounter = 0f;
					_waiting = true;
				}
			}
			else
			{
				_waiting = true;
			}

		}

		state = BTNodeState.RUNNING;
		return state;
	}
}
