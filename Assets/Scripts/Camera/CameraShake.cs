using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance { get; private set; }

	private CinemachineVirtualCamera cinemachineVirtualCamera;
	private float shakeCounter;
	private float shakeTime;
	private float startAmount;

	private void Awake()
	{
		Instance = this;
		cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
	}

	public void ShakeCamera(float amount, float duration)
	{
		CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amount;
		startAmount = amount;
		shakeTime = duration;
		shakeCounter = duration;
	}

	private void Update()
	{
		//Gradually reduce shake amount to zero
		if (shakeCounter > 0f)
		{
			shakeCounter -= Time.deltaTime;
			CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startAmount, 0f, 1 - (shakeCounter / shakeTime));
		}
	}
}
