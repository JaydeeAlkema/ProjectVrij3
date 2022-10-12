using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private AbilityScriptable abilityToGive;
    [SerializeField] private UpgradeScriptable upgradeToGive;
    [SerializeField] private AbilityScriptable meleeAttack;
    [SerializeField] private AbilityScriptable rangedAttack;
    private PlayerControler player;
    public AbilityScriptable AbilityToGive { get => abilityToGive; set => abilityToGive = value; }
    public UpgradeScriptable UpgradeToGive { get => upgradeToGive; set => upgradeToGive = value; }

	private void Start()
	{
        player = FindObjectOfType<PlayerControler>();
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
        rangedAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        rangedAttack.Damage += upgradeToGive.DamageUpgrade;
        rangedAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
        rangedAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        rangedAttack.statusEffectType = upgradeToGive.StatusEffect;
        Destroy( this.gameObject );
    }
}
