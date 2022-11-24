using UnityEngine;

[CreateAssetMenu(fileName = "New Map Pieces", menuName = "ScriptableObjects/New Map Pieces")]
public class MapPiecesList : ScriptableObject
{
	[SerializeField] private WeightedRandomList<MapPiece> mapPieces = new WeightedRandomList<MapPiece>();

	public WeightedRandomList<MapPiece> MapPieces { get => mapPieces; private set => mapPieces = value; }
}
