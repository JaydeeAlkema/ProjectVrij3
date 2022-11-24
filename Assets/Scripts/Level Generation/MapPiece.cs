using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
	[SerializeField] private List<Transform> connectionPoints = new List<Transform>();
	[SerializeField] private WeightedRandomList<GameObject> adjecentMapPieces = new WeightedRandomList<GameObject>();

	public List<Transform> ConnectionPoints { get => connectionPoints; private set => connectionPoints = value; }
	public WeightedRandomList<GameObject> AdjecentMapPieces { get => adjecentMapPieces; private set => adjecentMapPieces = value; }
}
