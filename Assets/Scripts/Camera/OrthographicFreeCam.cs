using NaughtyAttributes;
using UnityEngine;

public class OrthographicFreeCam : MonoBehaviour
{
	[SerializeField, BoxGroup("This is now a BoxGroup"), Label("This now has a label")] private float moveSpeed = 1f;

	public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
