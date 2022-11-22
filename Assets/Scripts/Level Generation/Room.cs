using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private RoomType roomType = RoomType.Generic;
	[SerializeField] private Vector2Int roomSize = Vector2Int.one;
	[Space]
	[SerializeField] private Transform wallsParent = null;
	[SerializeField] private Transform floorsParent = null;
	[SerializeField] private Transform propsParent = null;
	[SerializeField] private List<GameObject> pathwayOpenings = new List<GameObject>();
	[SerializeField] private List<GameObject> statuePoints = new List<GameObject>();
	[Space]
	[SerializeField, ReadOnly] private List<Room> connectedRooms = new List<Room>();
	[SerializeField, ReadOnly] private Dictionary<Vector2Int, Transform> collideableTiles = new Dictionary<Vector2Int, Transform>();
	[SerializeField, ReadOnly] private Dictionary<Vector2Int, Transform> noncollideableTiles = new Dictionary<Vector2Int, Transform>();

	public RoomType RoomType { get => roomType; set => roomType = value; }
	public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
	public Transform PropsParent { get => propsParent; set => propsParent = value; }
	public List<GameObject> PathwayOpenings { get => pathwayOpenings; set => pathwayOpenings = value; }
	public List<GameObject> StatuePoints { get => statuePoints; set => statuePoints = value; }
	public List<Room> ConnectedRooms { get => connectedRooms; set => connectedRooms = value; }
	public Dictionary<Vector2Int, Transform> CollideableTiles { get => collideableTiles; set => collideableTiles = value; }
	public Dictionary<Vector2Int, Transform> NoncollideableTiles { get => noncollideableTiles; set => noncollideableTiles = value; }

	public IEnumerator InitRoom()
	{
		yield return StartCoroutine(FetchCollideableTiles());
		yield return StartCoroutine(FetchNonCollideableTiles());
	}

	private IEnumerator FetchCollideableTiles()
	{
		List<Transform> allChildren = wallsParent.GetComponentsInChildren<Transform>().ToList();
		collideableTiles.Clear();

		foreach (GameObject pathwayOpeningTile in pathwayOpenings)
		{
			allChildren.AddRange(pathwayOpeningTile.GetComponentInChildren<Transform>());
		}
		foreach (Transform child in allChildren)
		{
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
			SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
			Vector2Int childCoordinate = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));
			if (boxCollider2D && spriteRenderer)
			{
				bool added = collideableTiles.TryAdd(childCoordinate, child);
				//if (!added)
				//{
				//	Debug.Log(child.name, child);
				//}
			}
		}

		//Debug.Log(collideableTiles.Count);
		yield return null;
	}

	private IEnumerator FetchNonCollideableTiles()
	{
		Transform[] allChildren = floorsParent.GetComponentsInChildren<Transform>();
		noncollideableTiles.Clear();

		foreach (Transform child in allChildren)
		{
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
			SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
			Vector2Int childCoordinate = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));
			if (!boxCollider2D && spriteRenderer)
			{
				bool added = noncollideableTiles.TryAdd(childCoordinate, child);
				//if (!added)
				//{
				//	Debug.Log(child.name, child);
				//}
			}
		}

		//Debug.Log(noncollideableTiles.Count);
		yield return null;
	}

	public void RandomizeFloorTileSprites(List<Sprite> sprites)
	{
		foreach (KeyValuePair<Vector2Int, Transform> floorTile in noncollideableTiles)
		{
			SpriteRenderer spriteRenderer = floorTile.Value.GetComponent<SpriteRenderer>();
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
