using UnityEngine;

public class OrthographicFreeCam : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private float moveSpeedMultiplier = 2f;

	private float inputHorizontal = 0f;
	private float inputVertical = 0f;

	private Camera mainCam;

	private void Awake()
	{
		mainCam = Camera.main;
	}

	private void Update()
	{
		float finalMoveSpeed = moveSpeed;
		inputHorizontal = Input.GetAxisRaw("Horizontal");
		inputVertical = Input.GetAxisRaw("Vertical");
		Vector3 input = new(inputHorizontal, inputVertical, 0f);

		if (Input.GetKey(KeyCode.LeftShift)) finalMoveSpeed *= moveSpeedMultiplier;

		mainCam.orthographicSize -= Input.mouseScrollDelta.y;

		transform.position += finalMoveSpeed * Time.deltaTime * input.normalized;
	}
}
