using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private RoomType roomType = RoomType.Generic;
	[SerializeField] private Vector2Int roomSize = Vector2Int.one;
	[Space]
	[SerializeField] private List<GameObject> pathwayEntries = new List<GameObject>();
	[SerializeField] private Transform wallTilesParent = default;
	[Space]
	[SerializeField, ReadOnly] private List<Room> connectedRooms = new List<Room>();
	[SerializeField, ReadOnly] private List<Vector2Int> wallTileCoordinates = new List<Vector2Int>();

	public List<GameObject> PathwayEntries { get => pathwayEntries; set => pathwayEntries = value; }
	public List<Room> ConnectedRooms { get => connectedRooms; set => connectedRooms = value; }
	public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
	public List<Vector2Int> WallTileCoordinates { get => wallTileCoordinates; set => wallTileCoordinates = value; }

	private void Start()
	{
		foreach (Transform child in wallTilesParent.GetComponentsInChildren<Transform>())
		{
			if (child != wallTilesParent)
			{
				Vector2Int coordinates = new Vector2Int(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.y));
				WallTileCoordinates.Add(coordinates);
			}
		}
	}

	public void TogglePathwayEntry(int index, bool toggle)
	{
		pathwayEntries[index].gameObject.SetActive(toggle);
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
