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

	[Header( "Reward Settings" )]
	[SerializeField] private int healOnPointSpend;
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

		System.Random rand = new System.Random();

		if (roll >= t3Chance)
		{
			if (tier3Upgrades.Length > 0)
			{
				upgradeToGive = tier3Upgrades[rand.Next(0, tier3Upgrades.Length -1)];
				Debug.Log(upgradeToGive.name);
			}
		}
		else if (roll >= t2Chance && roll < t3Chance)
		{
			if (tier2Upgrades.Length > 0)
			{
				upgradeToGive = tier2Upgrades[rand.Next( 0, tier2Upgrades.Length -1)];
				Debug.Log(upgradeToGive.name);
			}
		}
		else
		{
			if (tier1Upgrades.Length > 0)
			{
				upgradeToGive = tier1Upgrades[rand.Next( 0, tier1Upgrades.Length -1)];
				Debug.Log(upgradeToGive.name);
			}
		}

		meleeUpgradeImg.sprite = upgradeToGive.UpgradeImageMelee;
		rangedUpgradeImg.sprite = upgradeToGive.UpgradeImageRanged;
		if (abilityStats.AbilityIcon != null)
		{
			abilityImg.sprite = abilityStats.AbilityIcon;
		}
		MeleeTitle.text = upgradeToGive.name + " Melee";
		RangedTitle.text = upgradeToGive.name + " Ranged";
	}

	public void ChooseAbility()
	{
		if( GameManager.Instance.ExpManager.PlayerPoints >= 1 )
		{
			IAbility abilityToGive = null;
			switch( reward )
			{
				case AbilityReward.LineUp:
					abilityToGive = new LineUpAbility();
					break;
				case AbilityReward.BlackHole:
					abilityToGive = new BlackHoleAbility();
					break;
			}

			if( abilityToGive != null )
			{
				bool hasChosen = false;
				if( player.CurrentAbility1 == null || abilityButton == 1 )
				{
					player.CurrentAbility1 = abilityToGive;
					player.Ability1 = abilityStats;
					player.Ability1.SetBaseStats();
					GameManager.Instance.UiManager.SetAbilityUIValues( 0, player.Ability1.AbilityIcon );
					hasChosen = true;
				}
				else if( player.CurrentAbility2 == null || abilityButton == 2 )
				{
					player.CurrentAbility2 = abilityToGive;
					player.Ability2 = abilityStats;
					player.Ability2.SetBaseStats();
					GameManager.Instance.UiManager.SetAbilityUIValues( 1, player.Ability2.AbilityIcon );
					hasChosen = true;
				}
				else if( player.CurrentAbility3 == null || abilityButton == 3 )
				{
					player.CurrentAbility3 = abilityToGive;
					player.Ability3 = abilityStats;
					player.Ability3.SetBaseStats();
					GameManager.Instance.UiManager.SetAbilityUIValues( 2, player.Ability3.AbilityIcon );
					hasChosen = true;
				}
				else if( player.CurrentAbility4 == null || abilityButton == 4 )
				{
					player.CurrentAbility4 = abilityToGive;
					player.Ability4 = abilityStats;
					player.Ability4.SetBaseStats();
					GameManager.Instance.UiManager.SetAbilityUIValues( 3, player.Ability4.AbilityIcon );
					hasChosen = true;
				}
				else
				{
					//choose what ability to override
					abilityButtonPopUp.SetActive( true );
					choicePopUp.SetActive( false );
				}

				if( hasChosen )
				{
					GameManager.Instance.ExpManager.PlayerPoints -= 1;
					GameManager.Instance.PlayerHP.value += healOnPointSpend;
					abilityButton = 0;
					GameManager.Instance.SetPauseState( false );
					player.initAbilities();
					Destroy( this.gameObject );
				}
			}
		}
	}

	public void ChooseMeleeUpgrade()
	{
		// Check if the player has enough points to upgrade
		if( GameManager.Instance.ExpManager.PlayerPoints >= 1 )
		{
			meleeAttack = player.MeleeAttackScr;

			GameManager.Instance.SetPauseState( false );
			GameManager.Instance.ExpManager.PlayerPoints -= 1;
			GameManager.Instance.PlayerHP.value += healOnPointSpend;

			// Check if the upgrade already exists and apply it if necessary
			if( player.CurrentMeleeAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
			{
				UpgradeMelee( upgradeToGive.StatusEffect );
				Debug.Log( "upgraded melee" );
			}
			else
			{
				player.CurrentMeleeAttack.AbilityUpgrades.Add( upgradeToGive.StatusEffect, true );
				Debug.Log( "added " + upgradeToGive.name + " to player as melee" );
			}

			// Apply the upgrade
			if( meleeAttack.CoolDown > upgradeToGive.AttackSpeedUpgrade )
			{
				meleeAttack.CoolDown -= upgradeToGive.AttackSpeedUpgrade;
			}
			meleeAttack.Damage += upgradeToGive.DamageUpgrade;
			meleeAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
			meleeAttack.Distance += upgradeToGive.DistanceUpgrade;
			meleeAttack.CritChance += upgradeToGive.CritChanceUpgrade;

			// Remove the upgrade object
			Destroy( this.gameObject );
		}
	}

	public void UpgradeMelee(StatusEffectType status)
	{
		switch (status)
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				meleeAttack.BurnDamage *= burnMultiplier;
				Debug.Log("Upgraded burn");
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				meleeAttack.SlowAmount /= slowMultiplier;
				Debug.Log("Upgraded slow");
				break;
			case StatusEffectType.Marked:
				meleeAttack.MarkHits *= markMultiplier;
				break;
			default:
				break;
		}
	}

	public void ChooseRangedUpgrade()
	{
		if( GameManager.Instance.ExpManager.PlayerPoints >= 1 )
		{
			rangedAttack = player.RangedAttackScr;
			GameManager.Instance.SetPauseState( false );
			GameManager.Instance.ExpManager.PlayerPoints -= 1;
			GameManager.Instance.PlayerHP.value += healOnPointSpend;

			// Check if upgradeToGive.StatusEffect exists in player.CurrentRangedAttack.AbilityUpgrades
			if( player.CurrentRangedAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
			{
				UpgradeRanged( upgradeToGive.StatusEffect );
				Debug.Log( "upgraded ranged" );
			}
			else
			{
				// Add upgradeToGive.StatusEffect to player.CurrentRangedAttack.AbilityUpgrades
				player.CurrentRangedAttack.AbilityUpgrades.Add( upgradeToGive.StatusEffect, true );
				Debug.Log( "added " + upgradeToGive.name + " to player as ranged" );
			}

			// Apply upgrades to rangedAttack
			if( rangedAttack.CoolDown > upgradeToGive.AttackSpeedUpgrade )
			{
				rangedAttack.CoolDown -= upgradeToGive.AttackSpeedUpgrade;
			}
			rangedAttack.Damage += upgradeToGive.DamageUpgrade;
			rangedAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
			rangedAttack.CircleSize += upgradeToGive.CircleSizeUpgrade;
			rangedAttack.CritChance += upgradeToGive.CritChanceUpgrade;

			//Dev UI text, remove later
			GameManager.Instance.UiManager.AddDevText( 1, upgradeToGive.name );

			Destroy( this.gameObject );
		}
	}

	public void UpgradeRanged(StatusEffectType status)
	{
		switch (status)
		{
			case StatusEffectType.none:
				break;
			case StatusEffectType.Burn:
				rangedAttack.BurnDamage *= 2;
				rangedAttack.UpdateStatusEffects();
				Debug.Log("Upgraded burn");
				Debug.Log("burn damage is now: " + player.PlayerAbilityController.CurrentRangedAttack.BurnDamage);
				break;
			case StatusEffectType.Stun:
				break;
			case StatusEffectType.Slow:
				rangedAttack.SlowAmount = (player.RangedAttackScr.SlowAmount / 2);
				rangedAttack.UpdateStatusEffects();
				Debug.Log("Upgraded slow");
				break;
			case StatusEffectType.Marked:
				rangedAttack.MarkHits *= 2;
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
