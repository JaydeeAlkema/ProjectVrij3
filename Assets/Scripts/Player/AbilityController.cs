using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private static AbilityController abilityControllerInstance;
    private PlayerControler player;
    private Rigidbody2D rb2d;
    private Vector3 mousePos;
    private Vector2 lookDir;
    private Transform castFromPoint;
    [SerializeField] private bool isAttacking;
    private float angle;
    private IAbility currentMeleeAttack;
    private IAbility currentRangedAttack;
    private IAbility currentDash;
    private IAbility currentAbility1;
    private IAbility currentAbility2;
    private IAbility currentAbility3;
    public static AbilityController AbilityControllerInstance{ get => abilityControllerInstance; set => abilityControllerInstance = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public IAbility CurrentMeleeAttack { get => currentMeleeAttack; set => currentMeleeAttack = value; }
    public IAbility CurrentRangedAttack { get => currentRangedAttack; set => currentRangedAttack = value; }
    public IAbility CurrentDash { get => currentDash; set => currentDash = value; }
    public IAbility CurrentAbility1 { get => currentAbility1; set => currentAbility1 = value; }
    public IAbility CurrentAbility2 { get => currentAbility2; set => currentAbility2 = value; }
    public IAbility CurrentAbility3 { get => currentAbility3; set => currentAbility3 = value; }
    public PlayerControler Player { get => player; set => player = value; }


	private void Awake()
	{
        abilityControllerInstance = this;
	}
	//gives all attacks/abilities cooldowns
	public void SetAttacks()
	{
        currentMeleeAttack = new CoolDownDecorator( currentMeleeAttack, currentMeleeAttack.BaseStats.CoolDown );
        currentRangedAttack = new CoolDownDecorator( currentRangedAttack, currentRangedAttack.BaseStats.CoolDown );
        currentDash = new CoolDownDecorator( currentDash, currentDash.BaseStats.CoolDown );
    }

    public void SetAbility()
    {
        if( currentAbility1 != null )
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

    //makes it so the cooldowns actually update when getting upgrades
    public void UpdateCoolDown(AbilityScriptable melee, AbilityScriptable ranged, AbilityScriptable ab1, AbilityScriptable ab2, AbilityScriptable ab3, AbilityScriptable cd)
    {
        if( currentMeleeAttack != null ) { currentMeleeAttack.CoolDown = melee.CoolDown; currentMeleeAttack.BurnDamage = melee.BurnDamage; }
        if( currentRangedAttack != null ) { currentRangedAttack.CoolDown = ranged.CoolDown; }
        if(currentDash != null) { currentDash.CoolDown = cd.CoolDown; }
        if( currentAbility1 != null ) { currentAbility1.CoolDown = ab1.CoolDown; }
        if( currentAbility2 != null ) { currentAbility2.CoolDown = ab2.CoolDown; }
        if( currentAbility3 != null ) { currentAbility3.CoolDown = ab3.CoolDown; }
	}


    //Shit show of switches for all the effects with decorators
	public IAbility MeleeAttacked(IAbility melee)
    {
        if( !isAttacking )
        {
            isAttacking = true;
            foreach( KeyValuePair<StatusEffectType, bool> effect in melee.AbilityUpgrades )
            {
                Debug.Log( "effect in effects is: " + effect );
                if( effect.Value )
                {
                    switch( effect.Key )
                    {
                        case StatusEffectType.none:
                            break;
                        case StatusEffectType.Burn:
                            IAbility burn = new BurningMeleeDecorator( currentMeleeAttack, currentMeleeAttack.BaseStats );
                            burn.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
                            burn.AbilityBehavior();
                            Debug.Log( "should be burning" );
                            break;
                        case StatusEffectType.Stun:
                            break;
                        case StatusEffectType.Slow:
                            IAbility slow = new SlowDecorator( currentMeleeAttack );
                            break;
                        case StatusEffectType.Marked:
                            IAbility mark = new MarkDecorator( currentMeleeAttack );
                            break;
                        default:
                            break;
                    }
                }
            }
            IAbility anim = new AnimationDecorator( currentMeleeAttack, "MeleeAttack1", "isAttacking" );
            anim.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
            anim.CallAbility( this.GetComponent<PlayerControler>() );
            currentMeleeAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
            currentMeleeAttack.CallAbility( player );

            //Camera shake
            CameraShake.Instance.ShakeCamera(4f, 0.1f);

            //Reset buffer counter
            player.BufferCounterMelee = 0f;

            //Melee sound
            AkSoundEngine.PostEvent("plr_attck_melee", this.gameObject);
        }
        return currentMeleeAttack;
	}

    public IAbility RangeAttacked(IAbility ranged)
    {
        if( !isAttacking )
        {
            isAttacking = true;
            foreach( KeyValuePair<StatusEffectType, bool> effect in ranged.AbilityUpgrades )
            {
                if( effect.Value )
                {
                    switch( effect.Key )
                    {
                        case StatusEffectType.none:
                            break;
                        case StatusEffectType.Burn:
                            Debug.Log( "burning" );
                            IAbility burn = new BurningRangedDecorator( currentRangedAttack, true );
                            burn.AbilityBehavior();
                            break;
                        case StatusEffectType.Stun:
                            break;
                        case StatusEffectType.Slow:
                            IAbility slow = new SlowDecorator( currentRangedAttack );
                            break;
                        case StatusEffectType.Marked:
                            IAbility mark = new MarkDecorator( currentRangedAttack );
                            break;
                        default:
                            break;
                    }
                }
                //currentRangedAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
                //return currentRangedAttack;
            }
            IAbility anim = new AnimationDecorator( currentMeleeAttack, "", "isAttacking" );
            anim.CallAbility( this.GetComponent<PlayerControler>() );
            currentRangedAttack.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
            currentRangedAttack.CallAbility( player );

            //Camera shake
            CameraShake.Instance.ShakeCamera(4f, 0.1f);

            //Reset buffer counter
            player.BufferCounterCast = 0f;

            //Cast sound
            AkSoundEngine.PostEvent("plr_attck_cast", this.gameObject);

        }
        return currentRangedAttack;
	}

    public IAbility Dashing(IAbility dash)
    {
        foreach( KeyValuePair<StatusEffectType, bool> effect in dash.AbilityUpgrades )
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
        }
        currentDash.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
        currentDash.CallAbility( player );

        //Reset buffer counter
        player.BufferCounterDash = 0f;

        return currentDash;
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
            //currentAbility1.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle );
            //return currentAbility1;
        }
        currentAbility1.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
        currentAbility1.CallAbility( player );
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
            currentAbility2.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
            return currentAbility2;
        }
        currentAbility2.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
        currentAbility2.CallAbility( player );
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
            currentAbility3.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
            return currentAbility3;
        }
        currentAbility3.SetPlayerValues( rb2d, mousePos, lookDir, castFromPoint, angle, false );
        currentAbility3.CallAbility( player );
        return currentAbility3;
	}


    //always get set player values when the player controller updates their rotation etc, must always be called on mouse movement
    public void SetPlayerValues( Rigidbody2D _rb2d, Vector3 _mousePos, Vector2 _lookDir, Transform _castFromPoint, float _angle )
    {
        rb2d = _rb2d;
        mousePos = _mousePos;
        lookDir = _lookDir;
        castFromPoint = _castFromPoint;
        angle = _angle;
    }
}
