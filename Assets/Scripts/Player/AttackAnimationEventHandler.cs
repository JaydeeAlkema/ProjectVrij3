using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventHandler : MonoBehaviour
{

	[SerializeField] private bool hitDetection = false;

	public bool HitDetection { get => hitDetection; set => hitDetection = value; }

	//public void StartMeleeHitDetection()
	//{
	//	hitDetection = true;
	//}

}
