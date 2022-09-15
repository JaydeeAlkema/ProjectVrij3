using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class AbilityScriptable : ScriptableObject
{
	[SerializeField]
	private bool damaging = false;
	[SerializeField]
	private bool moving = false;

	[SerializeField]
	private float coolDown = 0f;
	[SerializeField]
	private float damage = 0f;
	[SerializeField]
	private EdgeCollider2D shape;
	[SerializeField]
	private float distance = 0f;

	private void DealDamage(GameObject target)
	{
		//target.hp -- damage;
	}

	private void Move(float dis, Vector3 dir, float spd)
	{
		//this.rb2d.velocity += movement;
	}
	
}
