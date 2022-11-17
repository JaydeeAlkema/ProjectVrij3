using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OneShotVFX : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private Light2D light2D = null;

	private void Awake()
	{
		Destroy(this.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
	}

	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitSpark1") && light2D != null)
		{
			while (light2D.intensity > 0f)
			{
				light2D.intensity -= 0.5f;
				break;
			}
		}
	}
}
