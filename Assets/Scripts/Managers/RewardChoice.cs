using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private IAbility abilityToGive;
    [SerializeField] private AbilityScriptable abilityStats;
    [SerializeField] private int rewardID;
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
    public int RewardID { get => rewardID; set => rewardID = value; }
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
        int maxRangeT1 = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier1" ).Length;
        tier1Upgrades = new UpgradeScriptable[maxRangeT1];
        tier1Upgrades = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier1");
        Debug.Log( "tier 1 upgrades loaded: " + tier1Upgrades.Length );
        //load tier 2 upgrades
        int maxRangeT2 = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier2" ).Length;
        tier2Upgrades = new UpgradeScriptable[maxRangeT2];
        tier2Upgrades = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier2" );
        Debug.Log( "tier 2 upgrades loaded: " + tier2Upgrades.Length );
        //load tier 3 upgrades
        int maxRangeT3 = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier3" ).Length;
        tier3Upgrades = new UpgradeScriptable[maxRangeT3];
        tier3Upgrades = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades/Tier3" );
        Debug.Log( "tier 3 upgrades loaded: " + tier3Upgrades.Length );

        //Rolling for upgrade all folders must contain atleast 1 upgrade
        roll = Random.Range( 1, 1000001 );
        if(roll >= t3Chance)
        {
            if( tier3Upgrades.Length > 0 )
            {
                upgradeToGive = tier3Upgrades[Random.Range( 0, tier3Upgrades.Length )];
                Debug.Log( upgradeToGive.name );
            }
        }
        else if( roll >= t2Chance && roll < t3Chance)
        {
            if( tier2Upgrades.Length > 0 )
            {
                upgradeToGive = tier2Upgrades[Random.Range( 0, tier2Upgrades.Length )];
                Debug.Log( upgradeToGive.name );
            }
        }
        else
        {
            if( tier1Upgrades.Length > 0 )
            {
                upgradeToGive = tier1Upgrades[Random.Range( 0, tier1Upgrades.Length )];
                Debug.Log( upgradeToGive.name );
            }
        }

        meleeUpgradeImg.sprite = upgradeToGive.UpgradeImage;
        rangedUpgradeImg.sprite = upgradeToGive.UpgradeImage;
        abilityImg.sprite = abilityToGive.BaseStats.AbilityImage;
        AbilityTitle.text = "" + abilityToGive.GetType().Name;
        MeleeTitle.text = upgradeToGive.name + " Melee";
        RangedTitle.text = upgradeToGive.name + " Ranged";
    }

	public void ChooseAbility()
    {
        // give AbilityScriptable
        //player.Ability1 = abilityToGive;
        //IAbility giveAbility = abilityToGive;
        GameManager.Instance.TogglePauseGame();
        player.Ability1 = abilityStats;
        player.CurrentAbility1 = abilityToGive;
        player.initAbilities();
        Destroy( this.gameObject );
    }

    public void ChooseMeleeUpgrade()
    {
        // give one of possible upgrades
        GameManager.Instance.TogglePauseGame();
        player.CurrentMeleeAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        player.CurrentMeleeAttack.Damage += upgradeToGive.DamageUpgrade;
        player.CurrentMeleeAttack.BoxSize += new Vector2(upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade);
        player.CurrentMeleeAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        //check for existing upgrades to rank up
		foreach( KeyValuePair<StatusEffectType, bool> pair in player.CurrentMeleeAttack.AbilityUpgrades )
		{
            if( pair.Key == upgradeToGive.StatusEffect )
            {
                if( player.CurrentMeleeAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
                {
                    UpgradeMelee( upgradeToGive.StatusEffect );
                    Debug.Log( "upgraded melee" );
                }
            }
        }
        if( !player.CurrentMeleeAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
        {
            player.CurrentMeleeAttack.AbilityUpgrades.Add( upgradeToGive.StatusEffect, true );
            Debug.Log( "added " + upgradeToGive.name + " to player as melee" );
        }
        //Dev UI text, remove later
        GameManager.Instance.UiManager.AddDevText(0, upgradeToGive.name);

        Destroy( this.gameObject );
	}

    public void UpgradeMelee(StatusEffectType status)
    {
		switch( status )
        {
            case StatusEffectType.none:
                break;
            case StatusEffectType.Burn:
                player.AbilityController.CurrentMeleeAttack.BaseStats.BurnDamage *= 2;
                player.AbilityController.CurrentMeleeAttack.BaseStats.UpdateStatusEffects();
                //player.MeleeAttackScr.BurnDamage *= 2;
                //player.MeleeAttackScr.UpdateStatusEffects();
                Debug.Log( "Upgraded burn" );
                break;
            case StatusEffectType.Stun:
                break;
            case StatusEffectType.Slow:
                player.MeleeAttackScr.SlowAmount = ( player.CurrentMeleeAttack.SlowAmount / 2 );
                player.MeleeAttackScr.UpdateStatusEffects();
                Debug.Log( "Upgraded slow" );
                break;
            case StatusEffectType.Marked:
                break;
                default:
		    break;
		}
    }

    public void ChooseRangedUpgrade()
    {
        // give one of possible upgrades
        GameManager.Instance.TogglePauseGame();
        player.CurrentRangedAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        player.CurrentRangedAttack.Damage += upgradeToGive.DamageUpgrade;
        player.CurrentRangedAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
        player.CurrentRangedAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        //check for existing upgrades to rank up
        foreach( KeyValuePair<StatusEffectType, bool> pair in player.CurrentRangedAttack.AbilityUpgrades )
        {
            if( pair.Key == upgradeToGive.StatusEffect )
            {
                if( player.CurrentRangedAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
                {
                    UpgradeRanged( upgradeToGive.StatusEffect );
                    Debug.Log( "upgraded ranged" );
                }
            }
        }
        if( !player.CurrentRangedAttack.AbilityUpgrades.ContainsKey( upgradeToGive.StatusEffect ) )
        {
            player.CurrentRangedAttack.AbilityUpgrades.Add( upgradeToGive.StatusEffect, true );
            Debug.Log( "added " + upgradeToGive.name + " to player as ranged" );
        }

        //Dev UI text, remove later
        GameManager.Instance.UiManager.AddDevText(1, upgradeToGive.name);

        Destroy( this.gameObject );
    }

    public void UpgradeRanged( StatusEffectType status )
    {
        switch( status )
        {
            case StatusEffectType.none:
                break;
            case StatusEffectType.Burn:
                //player.AbilityController.CurrentMeleeAttack.BurnDamage *= 2;
                player.RangedAttackScr.BurnDamage *= 2;
                player.RangedAttackScr.UpdateStatusEffects();
                Debug.Log( "Upgraded burn" );
                break;
            case StatusEffectType.Stun:
                break;
            case StatusEffectType.Slow:
                player.RangedAttackScr.SlowAmount = ( player.CurrentRangedAttack.SlowAmount / 2 );
                player.RangedAttackScr.UpdateStatusEffects();
                Debug.Log( "Upgraded slow" );
                break;
            case StatusEffectType.Marked:
                break;
            default:
                break;
        }
    }
}
