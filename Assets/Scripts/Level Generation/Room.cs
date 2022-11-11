using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private RoomType roomType = RoomType.Generic;
	[SerializeField] private Vector2Int roomSize = Vector2Int.one;
	[Space]
	[SerializeField] private List<GameObject> pathwayOpenings = new List<GameObject>();
	[Space]
	[SerializeField, ReadOnly] private List<Room> connectedRooms = new List<Room>();
	[SerializeField, ReadOnly] private List<Transform> collideableTiles = new List<Transform>();
	[SerializeField, ReadOnly] private List<Transform> noncollideableTiles = new List<Transform>();

	public List<GameObject> PathwayOpenings { get => pathwayOpenings; set => pathwayOpenings = value; }
	public List<Room> ConnectedRooms { get => connectedRooms; set => connectedRooms = value; }
	public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
	public List<Transform> CollideableTiles { get => collideableTiles; set => collideableTiles = value; }
	public List<Transform> NoncollideableTiles { get => noncollideableTiles; set => noncollideableTiles = value; }
	public RoomType RoomType { get => roomType; set => roomType = value; }

	private void Start()
	{
		foreach (Transform collideableTile in collideableTiles)
		{
			collideableTile.GetComponent<SpriteRenderer>().sortingOrder = Mathf.CeilToInt(collideableTile.transform.position.y) + 1;
		}
		foreach (Transform nonCollideableTile in noncollideableTiles)
		{
			nonCollideableTile.GetComponent<SpriteRenderer>().sortingOrder = Mathf.CeilToInt(nonCollideableTile.transform.position.y) - 10;
		}
	}

	[Button]
	private void FetchCollideableTiles()
	{
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		collideableTiles.Clear();

		foreach (Transform child in allChildren)
		{
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
			SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
			if (boxCollider2D && spriteRenderer)
			{
				CollideableTiles.Add(child);
			}
		}
	}

	[Button]
	private void FetchNonCollideableTiles()
	{
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		noncollideableTiles.Clear();

		foreach (Transform child in allChildren)
		{
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
			SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
			if (!boxCollider2D && spriteRenderer)
			{
				NoncollideableTiles.Add(child);
			}
		}
	}

	public void RandomizeFloorTileSprites(List<Sprite> sprites)
	{
		foreach (Transform floorTile in noncollideableTiles)
		{
			SpriteRenderer spriteRenderer = floorTile.GetComponent<SpriteRenderer>();
			if (spriteRenderer)
			{
				spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
			}
		}
	}

	public void AddConnectedRoom(Room connectedRoom)
	{
		if (connectedRooms.Contains(connectedRoom)) return;

		connectedRooms.Add(connectedRoom);
	}

	private void OnDrawGizmosSelected()
	{
		if (roomSize != Vector2.zero)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(Vector3.zero, new Vector2(roomSize.x, roomSize.y));
		}
	}
}
