using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Marked : IStatusEffect
{

	public int markType;    //0 = MeleeTarget, 1 = CastTarget
	private int markHits;

	public StatusEffect_Marked(int getMarkType, int getMarkHits)
	{
		markType = getMarkType;
		markHits = getMarkHits;
	}


	public void Process(IDamageable damageable)
	{
		switch (markType)
		{
			case 0:
				meleeMark(damageable);
				break;
			case 1:
				castMark(damageable);
				break;
		}
	}

	public void meleeMark(IDamageable damageable)
	{
		damageable.GetMarked(0, markHits);
		damageable.RemoveStatusEffect(this);
	}

	public void castMark(IDamageable damageable)
	{
		damageable.GetMarked(1, markHits);
		damageable.RemoveStatusEffect(this);
	}
}