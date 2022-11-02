using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotVFX : MonoBehaviour
{
    [SerializeField] private Animator animator;

	private void Awake()
	{
		Destroy(this.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
	}
}
