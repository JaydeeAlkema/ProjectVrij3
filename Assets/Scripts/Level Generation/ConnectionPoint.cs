using UnityEngine;

public enum ConnectionPointDirection
{
	North,
	East,
	South,
	West
}

public class ConnectionPoint : MonoBehaviour
{
	[SerializeField] private ConnectionPointDirection direction = ConnectionPointDirection.North;
	[SerializeField] private bool occupied = false;

	public ConnectionPointDirection Direction { get => direction; private set => direction = value; }
	public bool Occupied { get => occupied; set => occupied = value; }

	private void OnDrawGizmos()
	{
		Color occupiedColor = new Color(0, 1, 0, 0.5f);
		Color unoccupiedColor = new Color(1, 0, 0, 0.5f);
		Gizmos.color = occupied ? occupiedColor : unoccupiedColor;

		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
