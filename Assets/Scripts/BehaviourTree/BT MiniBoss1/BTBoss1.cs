using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class BTBoss1 : BTTree
{
	[SerializeField] private int enemyType = 4;
	[SerializeField] private int amountOfAttacks = 2;
	public BossBase bossScript;

	protected override BTNode SetupTree()
	{
		BTNode root = new Selector(new List<BTNode>
		{
			new Sequence(new List<BTNode> //Attack sequence 1: Minion attack
			{
				new BossCheckCurrentAttack(1), //Check if Minion attack
				new Selector(new List<BTNode> //Behaviour depends on whether or not we have minions
				{
					new Sequence(new List<BTNode> //If we have minions, walk around.
					{
						new BossCheckMobs(bossScript),
						new BossWaitWithAnimation(0, bossScript, 5f, "MiniBoss1Walking"),
						new BossClearAttackSequence(new List<string>{ "currentAttackStep", "currentAttackType" })
					}),
					new Sequence(new List<BTNode> //If we don't have minions, spawn minions.
					{
						new BossWaitWithAnimation(0, bossScript, 1f, "MiniBoss1Charging"),
						new BossSpawnMinions(1, bossScript),
						new BossWaitWithAnimation(2, bossScript, 1f, "MiniBoss1Charging"),
						new BossClearAttackSequence(new List<string>{"currentAttackStep"})
					})
				})
			}),
			new Sequence(new List<BTNode> //Attack sequence 2: Smash attack
			{
				new BossCheckCurrentAttack(2), //Check if Smash attack
				new Sequence(new List<BTNode> //Smash attack sequence
				{
					new BossWaitWithAnimation(0, bossScript, 1f, "MiniBoss1Windup"),
					new BossWaitWithAnimation(1, bossScript, 0.1f, "MiniBoss1LeapUp"), // Replace with BossPlayAnimationUntilCompletion when animations are implemented
					//new BossPlayAnimationUntilCompletion(1, bossScript, "MiniBoss1LeapUp"),
					new BossWaitWithAnimation(2, bossScript, 0.75f, "MiniBoss1WaitToFallDown"),
					new BossWaitWithAnimation(3, bossScript, 0.1f, "MiniBoss1FallDown"), // Replace with BossPlayAnimationUntilCompletion when animations are implemented
					//new BossPlayAnimationUntilCompletion(3, bossScript, "MiniBoss1FallDown"),
					new BossStompAttack(4, (MiniBoss1)bossScript),
					new BossWaitWithAnimation(5, bossScript, 3f, "MiniBoss1Endlag"),
					new BossClearAttackSequence(new List<string>{ "currentAttackStep", "currentAttackType" })
				})
			}),
			new Sequence(new List<BTNode> //Idle and roll new attack
			{
				new BossWaitWithAnimation(0, bossScript, 1.5f, "MiniBoss1Idle"), //Add walk instead of idle later
				new BossClearAttackSequence(new List<string>{"currentAttackStep"}),
				new BossRollNewAttack(amountOfAttacks),
			}),
		});

		return root;
	}

}
