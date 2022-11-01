using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
	[SerializeField] private ScriptableInt playerExp;
	[SerializeField] private ScriptableInt playerPoints;
	[SerializeField] private int expToNextPoint = 0;

	public int PlayerExp { get => playerExp.value; private set => playerExp.value = value; }
	public int PlayerPoints { get => playerPoints.value; set => playerPoints.value = value; }
	public int ExpToNextPoint { get => expToNextPoint; set => expToNextPoint = value; }


	public void Start()
	{
		playerExp.value = 0;
		playerPoints.value = 0;
	}

	public void AddExp(int exp)
	{
		playerExp.value += exp;
		CheckGetPoint();
	}

	public void AddPoint()
	{
		playerPoints.value++;
	}

	private void CheckGetPoint()
	{
		if (playerExp.value >= expToNextPoint)
		{
			AddPoint();
			playerExp.value = 0;
		}
	}
}
