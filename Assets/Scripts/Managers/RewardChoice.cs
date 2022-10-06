using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private AbilityScriptable abilityToGive;
    public AbilityScriptable AbilityToGive { get => abilityToGive; set => abilityToGive = value; }

    [SerializeField] private UpgradeScriptable upgradeToGive;
    public UpgradeScriptable UpgradeToGive { get => upgradeToGive; set => upgradeToGive = value; }

    [SerializeField] private AbilityScriptable meleeAttack;
    [SerializeField] private AbilityScriptable rangedAttack;
    public void ChooseAbility()
    {
        // give AbilityScriptable
        FindObjectOfType<PlayerControler>().Ability1 = abilityToGive;
        Destroy( this.gameObject );
    }

    public void ChooseMeleeUpgrade()
    {
        // give one of possible upgrades
        meleeAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        meleeAttack.Damage += upgradeToGive.DamageUpgrade;
        meleeAttack.BoxSize += new Vector2(upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade);
        meleeAttack.CritChance += upgradeToGive.CritChanceUpgrade;
		for( int i = 0; i < meleeAttack.Effects.Length; i++ )
		{
            if( meleeAttack.Effects[i] == upgradeToGive.Effect ) break;
            if( meleeAttack.Effects[i] == EffectType.Null ) meleeAttack.Effects[i] = upgradeToGive.Effect;
		}
        meleeAttack.statusEffectType = upgradeToGive.StatusEffect;
        Destroy( this.gameObject );
	}

    public void ChooseRangedUpgrade()
    {
        // give one of possible upgrades
        rangedAttack.CoolDown += upgradeToGive.AttackSpeedUpgrade;
        rangedAttack.Damage += upgradeToGive.DamageUpgrade;
        rangedAttack.BoxSize += new Vector2( upgradeToGive.HitBoxUpgrade, upgradeToGive.HitBoxUpgrade );
        rangedAttack.CritChance += upgradeToGive.CritChanceUpgrade;
        for( int i = 0; i < rangedAttack.Effects.Length; i++ )
        {
            if( rangedAttack.Effects[i] == upgradeToGive.Effect ) break;
            if( rangedAttack.Effects[i] == EffectType.Null ) rangedAttack.Effects[i] = upgradeToGive.Effect;
        }
        Destroy( this.gameObject );
    }
}
