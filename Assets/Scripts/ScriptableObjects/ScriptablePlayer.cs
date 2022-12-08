using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptablePlayer : ScriptableObject
{
	private PlayerControler player;
	public PlayerControler Player { get => player; set => player = value; }

	private AbilityController abilityController;

	public AbilityController AbilityController { get => abilityController; set => abilityController = value; }
}
