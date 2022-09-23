using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void TakeDamage( float damage );

	void ApplyStatusEffect(IStatusEffect statusEffect);

	void RemoveStatusEffect(IStatusEffect statusEffect);
}
