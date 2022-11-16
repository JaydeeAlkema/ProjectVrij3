using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Ability
{
	private bool init = true;
	private AK.Wwise.Event abilitySound;
	private PlayerControler player;
	System.Timers.Timer attackTimer = new System.Timers.Timer();

	public override void CallAbility(PlayerControler _player)
	{
		player = _player;
		if (init)
		{
			SetAbilityStats();
			init = false;
		}
		AbilityBehavior();
	}
	public override void AbilityBehavior()
	{
		AudioManager.Instance.PostEventLocal(abilitySound, player.gameObject);
		CastedObject = Object.Instantiate(castObject, CastFromPoint.transform.position + (Vector3)LookDir.normalized, CastFromPoint.rotation, CastFromPoint.transform);
		Projectile proj = CastedObject.GetComponent<Projectile>();
		//TrailUpgrade = BaseStats.TrailUpgrade;
		proj.TrailUpgrade = TrailUpgrade;
		proj.Damage = damage;
		proj.LifeSpan = lifeSpan;
		proj.Force = force;
		proj.CastedFrom = this;
		CastedObject.transform.SetParent(null);
	}

	void SetAbilityStats()
	{
		castObject = BaseStats.CastObject;
		lifeSpan = BaseStats.LifeSpan;
		force = BaseStats.Force;
		damage = BaseStats.Damage;
		AttackTime = BaseStats.AttackTime;
		abilitySound = BaseStats.AbilitySound1;
	}
}
