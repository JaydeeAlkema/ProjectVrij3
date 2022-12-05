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
				new BossCheckCurrentAttack(1, bossScript), //Check if Minion attack
				new Sequence(new List<BTNode> //Minion attack
				{
					new BossPlayAnimationUntilCompletion(0, bossScript, "MiniBoss1StartCharging"),
					new BossWaitWithAnimation(1, bossScript, 1f, "MiniBoss1Charging"),
					new BossSpawnMinions(2, bossScript),
					new BossWaitWithAnimation(3, bossScript, 1f, "MiniBoss1Charging"),
					new BossWalkAround(4, bossScript, 8f, new List<string>{ "MiniBoss1Walking", "MiniBoss1Idle" }),
					new BossLaunchMobs(5, bossScript),
					new BossPlayAnimationUntilCompletion(6, bossScript, "MiniBoss1StartCharging"),
					new BossWaitWithAnimation(7, bossScript, 4f, "MiniBoss1Charging"),
					new BossPlayAnimationUntilCompletion(8, bossScript, "MiniBoss1StartEndlag"),
					new BossWaitWithAnimation(9, bossScript, 2f, "MiniBoss1Endlag"),
					new BossClearAttackSequence(new List<string>{ "currentAttackStep", "currentAttackType" })
				})
			}),
			new Sequence(new List<BTNode> //Attack sequence 2: Smash attack
			{
				new BossCheckCurrentAttack(2, bossScript), //Check if Smash attack
				new Sequence(new List<BTNode> //Smash attack sequence
				{
					new BossWaitWithAnimation(0, bossScript, 1f, "MiniBoss1Windup"),
					new BossWaitWithAnimation(1, bossScript, 0.1f, "MiniBoss1LeapUp"), // Replace with BossPlayAnimationUntilCompletion when animations are implemented
					//new BossPlayAnimationUntilCompletion(1, bossScript, "MiniBoss1LeapUp"),
					new BossWaitWithAnimation(2, bossScript, 0.75f, "MiniBoss1WaitToFallDown"),
					new BossWaitWithAnimation(3, bossScript, 0.1f, "MiniBoss1FallDown"), // Replace with BossPlayAnimationUntilCompletion when animations are implemented
					//new BossPlayAnimationUntilCompletion(3, bossScript, "MiniBoss1FallDown"),
					new BossStompAttack(4, (MiniBoss1)bossScript), //Make sure the shockwave object has been spawned BEFORE this!
					new BossPlayAnimationUntilCompletion(5, bossScript, "MiniBoss1StartEndlag"),
					new BossWaitWithAnimation(6, bossScript, 3f, "MiniBoss1Endlag"),
					new BossClearAttackSequence(new List<string>{ "currentAttackStep", "currentAttackType" })
				})
			}),
			new Sequence(new List<BTNode> //Idle and roll new attack
			{
				new BossWalkAround(0, bossScript, 3f, new List<string>{ "MiniBoss1Walking", "MiniBoss1Idle" }),
				new BossClearAttackSequence(new List<string>{"currentAttackStep"}),
				new BossRollNewAttack(amountOfAttacks),
			}),
		});

		return root;
	}

}
