using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void TakeDamage(int damage);

	void TakeDamage(int damage, int damageType);

	void GetSlowed(float slowAmount);

	void GetMarked(int markType);

	void ApplyStatusEffect(IStatusEffect statusEffect);

	void RemoveStatusEffect(IStatusEffect statusEffect);
}
