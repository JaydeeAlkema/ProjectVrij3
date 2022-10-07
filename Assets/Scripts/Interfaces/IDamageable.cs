using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void TakeDamage(float damage);

	void TakeDamage(float damage, int damageType);

	void GetSlowed(float slowAmount);

	void GetMarked(int markType);

	void ApplyStatusEffect(IStatusEffect statusEffect);

	void RemoveStatusEffect(IStatusEffect statusEffect);
}
