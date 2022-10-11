using UnityEngine;

public class Node : IHeapItem<Node>
{

	[SerializeField] private bool walkable;
	[SerializeField] private Vector2 worldPosition;
	[SerializeField] private int gridX;
	[SerializeField] private int gridY;

	[SerializeField] private int gCost;
	[SerializeField] private int hCost;
	[SerializeField] private Node parent;

	public Node(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int FCost => gCost + hCost;
	public int HeapIndex { get; set; }
	public bool Walkable { get => walkable; set => walkable = value; }
	public Vector2 WorldPosition { get => worldPosition; set => worldPosition = value; }
	public int GridX { get => gridX; set => gridX = value; }
	public int GridY { get => gridY; set => gridY = value; }
	public int GCost { get => gCost; set => gCost = value; }
	public int HCost { get => hCost; set => hCost = value; }
	public Node Parent { get => parent; set => parent = value; }

	public int CompareTo(Node nodeToCompare)
	{
		int compare = FCost.CompareTo(nodeToCompare.FCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}