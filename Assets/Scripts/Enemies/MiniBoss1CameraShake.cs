using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1CameraShake : MonoBehaviour
{

	public void CameraShakeOnWalk()
	{
		CameraShake.Instance.ShakeCamera(3f, 0.2f);
	}	

	public void CameraShakeOnStompAttack()
	{
		CameraShake.Instance.ShakeCamera(3f, 1f);
	}

}
