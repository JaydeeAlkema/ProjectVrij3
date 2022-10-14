using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private AbilityScriptable abilityToGive;
    [SerializeField] private UpgradeScriptable upgradeToGive;
    [SerializeField] private AbilityScriptable meleeAttack;
    [SerializeField] private AbilityScriptable rangedAttack;
    private UpgradeScriptable[] upgrades;
    private PlayerControler player;
    public AbilityScriptable AbilityToGive { get => abilityToGive; set => abilityToGive = value; }
    public UpgradeScriptable UpgradeToGive { get => upgradeToGive; set => upgradeToGive = value; }

	private void Start()
	{
        player = FindObjectOfType<PlayerControler>();
        //load all upgrade assets
        int maxRange = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrade" ).Length;
        upgrades = new UpgradeScriptable[maxRange];
        upgrades = Resources.LoadAll<UpgradeScriptable>( "ScriptableObjects/Upgrades");
        Debug.Log( "upgrades loaded: " + upgrades.Length );
        upgradeToGive = upgrades[Random.Range( 0, upgrades.Length -1 )];
        Debug.Log(upgradeToGive.name);
    }

	public void ChooseAbility()
    {
        // give AbilityScriptable
        FindObjectOfType<PlayerControler>().Ability1 = abilityToGive;
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
        Destroy( this.gameObject );
    }
}
