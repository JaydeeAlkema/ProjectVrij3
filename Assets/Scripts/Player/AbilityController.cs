using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Vector3 mousePos;
    private Vector2 lookDir;
    private Transform castFromPoint;
    private float angle;
    private IAbility currentMeleeAttack;
    private IAbility currentRangedAttack;
    private IAbility currentAbility1;
    private IAbility currentAbility2;
    private IAbility currentAbility3;
    public IAbility CurrentMeleeAttack { get => currentMeleeAttack; set => currentMeleeAttack = value; }
    public IAbility CurrentRangedAttack { get => currentRangedAttack; set => currentRangedAttack = value; }
    public IAbility CurrentAbility1 { get => currentAbility1; set => currentAbility1 = value; }
    public IAbility CurrentAbility2 { get => currentAbility2; set => currentAbility2 = value; }
    public IAbility CurrentAbility3 { get => currentAbility3; set => currentAbility3 = value; }

	public void SetAttacks()
	{
        currentMeleeAttack = new CoolDownDecorator( currentMeleeAttack, currentMeleeAttack.BaseStats.CoolDown );
        currentRangedAttack = new CoolDownDecorator( currentRangedAttack, currentRangedAttack.BaseStats.CoolDown );
        if(currentAbility1 != null)
        {
            currentAbility1 = new CoolDownDecorator( currentAbility1, currentAbility1.BaseStats.CoolDown );
		}
        if( currentAbility2 != null )
        {
            currentAbility2 = new CoolDownDecorator( currentAbility2, currentAbility2.BaseStats.CoolDown );
        }
        if( currentAbility3 != null )
        {
            currentAbility3 = new CoolDownDecorator( currentAbility3, currentAbility3.BaseStats.CoolDown );
        }
    }

    public void UpdateCoolDown(AbilityScriptable melee, AbilityScriptable ranged, AbilityScriptable ab1, AbilityScriptable ab2, AbilityScriptable ab3)
    {
        if( currentMeleeAttack != null ) { currentMeleeAttack.CoolDown = melee.CoolDown; }
        if( currentRangedAttack != null ) { currentRangedAttack.CoolDown = ranged.CoolDown; }
        if( currentAbility1 != null ) { currentAbility1.CoolDown = ab1.CoolDown; }
        if( currentAbility2 != null ) { currentAbility2.CoolDown = ab2.CoolDown; }
        if( currentAbility3 != null ) { currentAbility3.CoolDown = ab3.CoolDown; }
	}

	public IAbility MeleeAttacked(IAbility melee)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in melee.AbilityUpgrades )
        {
            if( effect.Value )
            {
                switch( effect.Key )
                {
                    case StatusEffectType.none:
                        break;
                    case StatusEffectType.Burn:
                        //currentMeleeAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
                        IAbility burn = new BurningMeleeDecorator( currentMeleeAttack, currentMeleeAttack.BaseStats );
                        burn.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
                        burn.AbilityBehavior();
                        break;
                    case StatusEffectType.Stun:
                        break;
                    case StatusEffectType.Slow:
                        break;
                    case StatusEffectType.Marked:
                        break;
                    default:
                        break;
                }
            }
            currentMeleeAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            return currentMeleeAttack;
        }
        IAbility anim = new AnimationDecorator( currentMeleeAttack, "MeleeAttack1", "isAttacking" );
        anim.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        anim.CallAbility( currentMeleeAttack.Player );
        currentMeleeAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        return currentMeleeAttack;
	}

    public IAbility RangeAttacked(IAbility ranged)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in ranged.AbilityUpgrades )
        {
            if( effect.Value )
            {
                switch( effect.Key )
                {
                    case StatusEffectType.none:
                        break;
                    case StatusEffectType.Burn:
                        break;
                    case StatusEffectType.Stun:
                        break;
                    case StatusEffectType.Slow:
                        break;
                    case StatusEffectType.Marked:
                        break;
                    default:
                        break;
                }
            }
            currentRangedAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            return currentRangedAttack;
        }
        IAbility anim = new AnimationDecorator( currentMeleeAttack, "", "isAttacking" );
        anim.CallAbility( currentRangedAttack.Player );
        currentRangedAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        return currentRangedAttack;
	}

    public IAbility AbilityOneAttacked(IAbility ability1)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in ability1.AbilityUpgrades )
        {
            if( effect.Value )
            {
                switch( effect.Key )
                {
                    case StatusEffectType.none:
                        break;
                    case StatusEffectType.Burn:
                        break;
                    case StatusEffectType.Stun:
                        break;
                    case StatusEffectType.Slow:
                        break;
                    case StatusEffectType.Marked:
                        break;
                    default:
                        break;
                }
            }
            currentAbility1.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            return currentAbility1;
        }
        currentAbility1.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        return currentAbility1;
	}

    public IAbility AbilityTwoAttacked(IAbility ability2)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in currentAbility2.AbilityUpgrades )
        {
            if( effect.Value )
            {
                switch( effect.Key )
                {
                    case StatusEffectType.none:
                        break;
                    case StatusEffectType.Burn:
                        break;
                    case StatusEffectType.Stun:
                        break;
                    case StatusEffectType.Slow:
                        break;
                    case StatusEffectType.Marked:
                        break;
                    default:
                        break;
                }
            }
            currentAbility2.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            return currentAbility2;
        }
        currentAbility2.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        return currentAbility2;
	}

    public IAbility AbilityThreeAttacked(IAbility ability3)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in ability3.AbilityUpgrades )
        {
            if( effect.Value )
            {
                switch( effect.Key )
                {
                    case StatusEffectType.none:
                        break;
                    case StatusEffectType.Burn:
                        break;
                    case StatusEffectType.Stun:
                        break;
                    case StatusEffectType.Slow:
                        break;
                    case StatusEffectType.Marked:
                        break;
                    default:
                        break;
                }
            }
            currentAbility3.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            return currentAbility3;
        }
        currentAbility3.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
        return currentAbility3;
	}

    public void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle )
    {
        rb2d = _rb2d;
        mousePos = _mousePos;
        lookDir = _lookDir;
        castFromPoint = _castFromPoint;
        angle = _angle;
    }
}
