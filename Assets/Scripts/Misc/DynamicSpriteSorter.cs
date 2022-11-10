using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSpriteSorter : MonoBehaviour
{
	private enum SortingMode
	{
		Start,
		Update,
		FixedUpdate,
	}
	private enum RoundingMode
	{
		Floor,
		Ceiling
	}
	private enum PositionReference
	{
		Parent,
		Self
	}

	[SerializeField] private SortingMode sortingMode = SortingMode.Start;
	[SerializeField] private RoundingMode roundingMode = RoundingMode.Floor;
	[SerializeField] private PositionReference positionReference = PositionReference.Self;
	[SerializeField] private int sortingOrderOffset = 0;

	private SpriteRenderer spriteRenderer;
	private Transform positionReferenceTransform = null;

	private void Awake()
	{
		switch (positionReference)
		{
			case PositionReference.Parent:
				positionReferenceTransform = transform.parent;
				break;
			case PositionReference.Self:
				positionReferenceTransform = transform;
				break;
			default:
				break;
		}
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
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
				spriteRenderer.sortingOrder = Mathf.FloorToInt(positionReferenceTransform.position.y);
				spriteRenderer.sortingOrder += sortingOrderOffset;
				break;
			case RoundingMode.Ceiling:
				spriteRenderer.sortingOrder = Mathf.CeilToInt(positionReferenceTransform.position.y);
				spriteRenderer.sortingOrder += sortingOrderOffset;
				break;
			default:
				break;
		}
	}
}
