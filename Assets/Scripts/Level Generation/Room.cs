using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private RoomType roomType = RoomType.Generic;
	[Space]
	[SerializeField] private List<GameObject> pathwayEntries = new List<GameObject>();
	[Space]
	[SerializeField, ReadOnly] private List<Room> connectedRooms = new List<Room>();

	public void TogglePathwayEntry(int index, bool toggle)
	{
		pathwayEntries[index].gameObject.SetActive(toggle);
	}

	public void AddConnectedRoom(Room connectedRoom)
	{
		if (connectedRooms.Contains(connectedRoom)) return;

		connectedRooms.Add(connectedRoom);
	}
}
