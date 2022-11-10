using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSpriteSorter : MonoBehaviour
{
	private enum SortingMode
	{
		Awake,
		Start,
		Update,
		FixedUpdate,
	}

	private enum RoundingMode
	{
		Floor,
		Ceiling
	}

	[SerializeField] private SortingMode sortingMode = SortingMode.Awake;
	[SerializeField] private RoundingMode roundingMode = RoundingMode.Floor;
	[SerializeField] private int sortingOrderOffset = 0;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		if (sortingMode == SortingMode.Awake)
		{
			SortByRoundingMode();
		}
	}

	private void Start()
	{
		if (sortingMode == SortingMode.Start)
		{
			SortByRoundingMode();
		}
	}

	private void Update()
	{
		if (sortingMode == SortingMode.Update)
		{
			SortByRoundingMode();
		}
	}

	private void FixedUpdate()
	{
		if (sortingMode == SortingMode.FixedUpdate)
		{
			SortByRoundingMode();
		}
	}

	private void SortByRoundingMode()
	{
		switch (roundingMode)
		{
			case RoundingMode.Floor:
				spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y + sortingOrderOffset);
				break;
			case RoundingMode.Ceiling:
				spriteRenderer.sortingOrder = Mathf.CeilToInt(transform.position.y + sortingOrderOffset);
				break;
			default:
				break;
		}
	}
}
