using UnityEngine;

public class BreakableProp : MonoBehaviour, IDamageable
{
	[SerializeField] private Animator animator;

	public void ApplyStatusEffect(IStatusEffect statusEffect)
	{
		animator.SetTrigger("Break");
	}

	public void GetMarked(int markType, float markHits)
	{
		animator.SetTrigger("Break");
	}

	public void GetSlowed(float slowAmount)
	{
		animator.SetTrigger("Break");
	}

	public void RemoveStatusEffect(IStatusEffect statusEffect)
	{
		animator.SetTrigger("Break");
	}

	public void TakeDamage(int damage)
	{
		animator.SetTrigger("Break");
	}

	public void TakeDamage( int damage, float critChance, float critModifier )
	{

	}
	public void TakeDamage(int damage, int damageType, float critChance, float critModifier )
	{
		animator.SetTrigger("Break");
	}
}
