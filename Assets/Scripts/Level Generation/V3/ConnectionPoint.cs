using UnityEngine;

public enum ConnectionPointDirection
{
	North,
	East,
	South,
	West
}

public enum ConnectionPointStatus
{
	Connected,
	Disconnected,
	Blocked
}

public class ConnectionPoint : MonoBehaviour
{
	[SerializeField] private ConnectionPointDirection direction = ConnectionPointDirection.North;
	[SerializeField] private ConnectionPointStatus status = ConnectionPointStatus.Disconnected;
	[SerializeField] private ConnectionPoint connectedTo = null;
	[SerializeField] private DeadEnd deadEnd = default;

	public ConnectionPointDirection Direction { get => direction; private set => direction = value; }
	public ConnectionPointStatus Status { get => status; set => status = value; }
	public ConnectionPoint ConnectedTo { get => connectedTo; set => connectedTo = value; }
	public DeadEnd DeadEnd { get => deadEnd; private set => deadEnd = value; }

	private void OnDrawGizmos()
	{
		Color connectedColor = new Color(0, 1, 0, 0.5f);
		Color disconnectedColor = new Color(1, 0, 0, 0.5f);
		Color blockedColor = new Color(1, 1, 0, 0.5f);

		switch (status)
		{
			case ConnectionPointStatus.Connected:
				Gizmos.color = connectedColor;
				break;
			case ConnectionPointStatus.Disconnected:
				Gizmos.color = disconnectedColor;
				break;
			case ConnectionPointStatus.Blocked:
				Gizmos.color = blockedColor;
				break;
			default:
				break;
		}
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
