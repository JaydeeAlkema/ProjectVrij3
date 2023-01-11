using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void TakeDamage( int damage );
	void TakeDamage(int damage, float critC, float critM);

	void TakeDamage(int damage, int damageType, float critC, float critM);

	void GetSlowed(float slowAmount);

	void GetMarked(int markType, float markHits);

	void ApplyStatusEffect(IStatusEffect statusEffect);

	void RemoveStatusEffect(IStatusEffect statusEffect);
}
