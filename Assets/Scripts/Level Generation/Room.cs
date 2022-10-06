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

	private void Start()
	{
		Transform[] allChildren = GetComponentsInChildren<Transform>();

		foreach (Transform child in allChildren)
		{
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
			SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
			if (boxCollider2D && spriteRenderer)
			{
				CollideableTiles.Add(child);
			}
			else if (!boxCollider2D && spriteRenderer)
			{
				NoncollideableTiles.Add(child);
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
