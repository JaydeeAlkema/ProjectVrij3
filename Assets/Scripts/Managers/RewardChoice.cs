using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardChoice : MonoBehaviour
{
	[Header("RankUp Values")]
	[SerializeField] private int burnMultiplier = 2;
	[SerializeField] private float slowMultiplier = 2;
	[SerializeField] private float markMultiplier = 2;

	[Header("Reward Settings")]
	[SerializeField] private GameObject choicePopUp;
	[SerializeField] private GameObject abilityButtonPopUp;
	[SerializeField] private IAbility abilityToGive;
	[SerializeField] private AbilityScriptable abilityStats;
	[SerializeField] private AbilityReward reward;
	[SerializeField] private int abilityButton;
	[SerializeField] private UpgradeScriptable upgradeToGive;
	[SerializeField] private AbilityScriptable meleeAttack;
	[SerializeField] private AbilityScriptable rangedAttack;
	[SerializeField] private int roll;
	[SerializeField] private Image abilityImg;
	[SerializeField] private Image meleeUpgradeImg;
	[SerializeField] private Image rangedUpgradeImg;
	[SerializeField] private TMP_Text AbilityTitle;
	[SerializeField] private TMP_Text MeleeTitle;
	[SerializeField] private TMP_Text RangedTitle;
	//chances are based on a roll of 1-1000000
	[SerializeField] private int t1Chance = 1; //1-700000
	[SerializeField] private int t2Chance = 700001; //700001-995000
	[SerializeField] private int t3Chance = 995001; //995001-1000000
	private Ability[] abilitiesToGive;
	private UpgradeScriptable[] tier1Upgrades;
	private UpgradeScriptable[] tier2Upgrades;
	private UpgradeScriptable[] tier3Upgrades;
	private PlayerControler player;
	public int AbilityButton { get => abilityButton; set => abilityButton = value; }
	public AbilityReward Reward { get => reward; set => reward = value; }
	public IAbility AbilityToGive { get => abilityToGive; set => abilityToGive = value; }
	public AbilityScriptable AbilityStats { get => abilityStats; set => abilityStats = value; }
	public UpgradeScriptable UpgradeToGive { get => upgradeToGive; set => upgradeToGive = value; }

	private void Start()
	{
		player = FindObjectOfType<PlayerControler>();
		//load abilities
		//      int maxRangeAbilities = Resources.LoadAll<Ability>( "Abilities/PowerAbilities" ).Length;
		//      abilitiesToGive = new Ability[maxRangeAbilities];
		//      abilitiesToGive = Resources.LoadAll<Ability>( "Abilities/PowerAbilities" );
		//foreach( Ability ability in abilitiesToGive )
		//{
		//          if(ability.abilityID == rewardID)
		//          {
		//              abilityToGive = ability;
		//	}
		//}
		//load tier 1 upgrades
		int maxRangeT1 = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier1").Length;
		tier1Upgrades = new UpgradeScriptable[maxRangeT1];
		tier1Upgrades = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier1");
		Debug.Log("tier 1 upgrades loaded: " + tier1Upgrades.Length);
		//load tier 2 upgrades
		int maxRangeT2 = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier2").Length;
		tier2Upgrades = new UpgradeScriptable[maxRangeT2];
		tier2Upgrades = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier2");
		Debug.Log("tier 2 upgrades loaded: " + tier2Upgrades.Length);
		//load tier 3 upgrades
		int maxRangeT3 = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier3").Length;
		tier3Upgrades = new UpgradeScriptable[maxRangeT3];
		tier3Upgrades = Resources.LoadAll<UpgradeScriptable>("ScriptableObjects/Upgrades/Tier3");
		Debug.Log("tier 3 upgrades loaded: " + tier3Upgrades.Length);

		//Rolling for upgrade all folders must contain atleast 1 upgrade
		roll = Random.Range(1, 1000001);
		if (roll >= t3Chance)
		{
			if (tier3Upgrades.Length > 0)
			{
				upgradeToGive = tier3Upgrades[Random.Range(0, tier3Upgrades.Length)];
				Debug.Log(upgradeToGive.name);
			}
		}
		else if (roll >= t2Chance && roll < t3Chance)
		{
			if (tier2Upgrades.Length > 0)
			{
				upgradeToGive = tier2Upgrades[Random.Range(0, tier2Upgrades.Length)];
				Debug.Log(upgradeToGive.name);
			}
		}
		else
		{
			if (tier1Upgrades.Length > 0)
			{
				upgradeToGive = tier1Upgrades[Random.Range(0, tier1Upgrades.Length)];
				Debug.Log(upgradeToGive.name);
			}
		}

		meleeUpgradeImg.sprite = upgradeToGive.UpgradeImageMelee;
		rangedUpgradeImg.sprite = upgradeToGive.UpgradeImageRanged;
		if (abilityStats.AbilityIcon != null)
		{
			abilityImg.sprite = abilityStats.AbilityIcon;
		}
		//AbilityTitle.text = "" + abilityToGive.GetType().Name;
		MeleeTitle.text = upgradeToGive.name + " Melee";
		RangedTitle.text = upgradeToGive.name + " Ranged";
	}

	public void ChooseAbility()
	{
		if (GameManager.Instance.ExpManager.PlayerPoints >= 1)
		{
			// give AbilityScriptable
			//player.Ability1 = abilityToGive;
			//IAbility giveAbility = abilityToGive;

			//player.CurrentAbility1 = abilityToGive;
			switch (reward)
			{
				case AbilityReward.LineUp:
					if (player.CurrentAbility1 == null || abilityButton == 1)
					{
						player.CurrentAbility1 = new LineUpAbility();
						player.Ability1 = abilityStats;
						player.Ability1.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(0, player.Ability1.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility2 == null || abilityButton == 2)
					{
						player.CurrentAbility2 = new LineUpAbility();
						player.Ability2 = abilityStats;
						player.Ability2.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(1, player.Ability2.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility3 == null || abilityButton == 3)
					{
						player.CurrentAbility3 = new LineUpAbility();
						player.Ability3 = abilityStats;
						player.Ability3.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(2, player.Ability3.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility4 == null || abilityButton == 4)
					{
						player.CurrentAbility4 = new LineUpAbility();
						player.Ability4 = abilityStats;
						player.Ability4.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(3, player.Ability4.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else
					{
						//choose what ability to override
						abilityButtonPopUp.SetActive(true);
						choicePopUp.SetActive(false);
					}
					break;
				case AbilityReward.BlackHole:
					if (player.CurrentAbility1 == null || abilityButton == 1)
					{
						player.CurrentAbility1 = new BlackHoleAbility();
						player.Ability1 = abilityStats;
						player.Ability1.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(0, player.Ability1.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility2 == null || abilityButton == 2)
					{
						player.CurrentAbility2 = new BlackHoleAbility();
						player.Ability2 = abilityStats;
						player.Ability2.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(1, player.Ability2.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility3 == null || abilityButton == 3)
					{
						player.CurrentAbility3 = new BlackHoleAbility();
						player.Ability3 = abilityStats;
						player.Ability3.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(2, player.Ability3.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else if (player.CurrentAbility4 == null || abilityButton == 4)
					{
						player.CurrentAbility4 = new BlackHoleAbility();
						player.Ability4 = abilityStats;
						player.Ability4.SetBaseStats();
						GameManager.Instance.ExpManager.PlayerPoints -= 1;
						abilityButton = 0;
						GameManager.Instance.SetPauseState(false);
						player.initAbilities();
						GameManager.Instance.UiManager.SetAbilityUIValues(3, player.Ability4.AbilityIcon);
						Destroy(this.gameObject);
						break;
					}
					else
					{
						//choose what ability to override
						abilityButtonPopUp.SetActive(true);
						choicePopUp.SetActive(false);
					}
					break;
				default:
					break;
			}

		}
	}

	public void ChooseMeleeUpgrade()
	{
		if (GameManager.Instance.ExpManager.PlayerPoints >= 1)
		{
			// give one of possible upgrades
			GameManager.Instance.SetPauseState(false);
			GameManager.Instance.ExpManager.PlayerPoints -= 1;
			player.MeleeAttackScr.CoolDown -= upgradeToGive.AttackSpeedUpgrade;
			player.MeleeAttackScr.Damage += upgradeToGive.DamageUpgrade;
			player.MeleeAttackScr.BoxSize += new Vector2(upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade);
			player.MeleeAttackScr.Distance += upgradeToGive.DistanceUpgrade;
			player.MeleeAttackScr.CritChance += upgradeToGive.CritChanceUpgrade;
			//check for existing upgrades to rank up
			foreach (KeyValuePair<StatusEffectType, bool> pair in player.CurrentMeleeAttack.AbilityUpgrades)
			{
				if (pair.Key == upgradeToGive.StatusEffect)
				{
					if (player.CurrentMeleeAttack.AbilityUpgrades.ContainsKey(upgradeToGive.StatusEffect))
					{
						UpgradeMelee(upgradeToGive.StatusEffect);
						Debug.Log("upgraded melee");
					}
				}
			}
			if (!player.CurrentMeleeAttack.AbilityUpgrades.ContainsKey(upgradeToGive.StatusEffect))
			{
				player.CurrentMeleeAttack.AbilityUpgrades.Add(upgradeToGive.StatusEffect, true);
				Debug.Log("added " + upgradeToGive.name + " to player as melee");
			}
			//Dev UI text, remove later
			GameManager.Instance.UiManager.AddDevText(0, upgradeToGive.name);

			Destroy(this.gameObject);
		}
	}

	public void UpgradeMelee(StatusEffectType status)
	{
		switch (status)
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				player.MeleeAttackScr.BurnDamage *= burnMultiplier;
				//AbilityController.AbilityControllerInstance.CurrentMeleeAttack.BurnDamage *= 2;
				//player.CurrentMeleeAttack.BaseStats.UpdateStatusEffects();
				Debug.Log("Upgraded burn");
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				player.MeleeAttackScr.SlowAmount = (player.MeleeAttackScr.SlowAmount / slowMultiplier);
				//player.AbilityController.CurrentMeleeAttack.BaseStats.UpdateStatusEffects();
				Debug.Log("Upgraded slow");
				break;
			case StatusEffectType.Marked:
				player.MeleeAttackScr.MarkHits *= markMultiplier;
				break;
			default:
				break;
		}
	}

	public void ChooseRangedUpgrade()
	{
		if (GameManager.Instance.ExpManager.PlayerPoints >= 1)
		{
			// give one of possible upgrades
			GameManager.Instance.SetPauseState(false);
			GameManager.Instance.ExpManager.PlayerPoints -= 1;
			player.RangedAttackScr.CoolDown -= upgradeToGive.AttackSpeedUpgrade;
			player.RangedAttackScr.Damage += upgradeToGive.DamageUpgrade;
			player.RangedAttackScr.BoxSize += new Vector2(upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade);
			player.RangedAttackScr.CircleSize += upgradeToGive.CircleSizeUpgrade;
			player.RangedAttackScr.CritChance += upgradeToGive.CritChanceUpgrade;
			//check for existing upgrades to rank up
			foreach (KeyValuePair<StatusEffectType, bool> pair in player.CurrentRangedAttack.AbilityUpgrades)
			{
				if (pair.Key == upgradeToGive.StatusEffect)
				{
					if (player.CurrentRangedAttack.AbilityUpgrades.ContainsKey(upgradeToGive.StatusEffect))
					{
						UpgradeRanged(upgradeToGive.StatusEffect);
						Debug.Log("upgraded ranged");
					}
				}
			}
			if (!player.CurrentRangedAttack.AbilityUpgrades.ContainsKey(upgradeToGive.StatusEffect))
			{
				player.CurrentRangedAttack.AbilityUpgrades.Add(upgradeToGive.StatusEffect, true);
				Debug.Log("added " + upgradeToGive.name + " to player as ranged");
			}

			//Dev UI text, remove later
			GameManager.Instance.UiManager.AddDevText(1, upgradeToGive.name);

			Destroy(this.gameObject);
		}
	}

	public void UpgradeRanged(StatusEffectType status)
	{
		switch (status)
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				player.RangedAttackScr.BurnDamage *= 2;
				player.RangedAttackScr.UpdateStatusEffects();
				Debug.Log("Upgraded burn");
				Debug.Log("burn damage is now: " + player.PlayerAbilityController.CurrentRangedAttack.BurnDamage);
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				player.RangedAttackScr.SlowAmount = (player.RangedAttackScr.SlowAmount / 2);
				player.RangedAttackScr.UpdateStatusEffects();
				Debug.Log("Upgraded slow");
				break;
			case StatusEffectType.Marked:
				player.RangedAttackScr.MarkHits *= 2;
				break;
			default:
				break;
		}
	}
}

public enum AbilityReward
{
	none = 0,
	LineUp = 1 << 0,
	BlackHole = 1 << 1,
}
