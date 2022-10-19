using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private AbilityScriptable abilityToGive;
    [SerializeField] private UpgradeScriptable upgradeToGive;
    [SerializeField] private AbilityScriptable meleeAttack;
    [SerializeField] private AbilityScriptable rangedAttack;
    [SerializeField] private int roll;
    //chances are based on a roll of 1-1000000
    [SerializeField] private int t1Chance = 1; //1-700000
    [SerializeField] private int t2Chance = 700001; //700001-995000
    [SerializeField] private int t3Chance = 995001; //995001-1000000
    private UpgradeScriptable[] tier1Upgrades;
    private UpgradeScriptable[] tier2Upgrades;
    private UpgradeScriptable[] tier3Upgrades;
    private PlayerControler player;
    public AbilityScriptable AbilityToGive { get => abilityToGive; set => abilityToGive = value; }
    public UpgradeScriptable UpgradeToGive { get => upgradeToGive; set => upgradeToGive = value; }

	private void Start()
	{
        player = FindObjectOfType<PlayerControler>();
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
    }

	public void ChooseAbility()
    {
        // give AbilityScriptable
        player.Ability1 = abilityToGive;
        IAbility giveAbility = abilityToGive.Ability;
        player.CurrentAbility1 = giveAbility;
        player.initAbilities();
        Destroy( this.gameObject );
    }

    public void ChooseMeleeUpgrade()
    {
        // give one of possible upgrades
        player.CurrentMeleeAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        player.CurrentMeleeAttack.Damage += upgradeToGive.DamageUpgrade;
        player.CurrentMeleeAttack.BoxSize += new Vector2(upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade);
        player.CurrentMeleeAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        player.CurrentMeleeAttack.AbilityUpgrades.Add( upgradeToGive.StatusEffect, true );
        Debug.Log( "added " + upgradeToGive.name + " to player as melee" );
        Destroy( this.gameObject );
	}

    public void ChooseRangedUpgrade()
    {
        // give one of possible upgrades
        player.CurrentRangedAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        player.CurrentRangedAttack.Damage += upgradeToGive.DamageUpgrade;
        player.CurrentRangedAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
        player.CurrentRangedAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        player.CurrentRangedAttack.AbilityUpgrades.Add(upgradeToGive.StatusEffect, true);
        Debug.Log( "added " + upgradeToGive.name + " to player as ranged" );
        Destroy( this.gameObject );
    }
}
