using UnityEngine;

public class MapPieceSizeHelper : MonoBehaviour
{
	[SerializeField] private int mapPieceSize = 41;
	[SerializeField] private bool drawGizmo = true;

	private void OnDrawGizmos()
	{
		if (!drawGizmo) return;

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, new Vector3(mapPieceSize, mapPieceSize));
	}
}
