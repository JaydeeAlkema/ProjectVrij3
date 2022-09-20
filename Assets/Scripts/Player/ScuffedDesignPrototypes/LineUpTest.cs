using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LineUpTest : MonoBehaviour
{
	public Transform maxRangePoint;
	public Transform enemy;
	public Rigidbody2D player;
	public Vector3 newPoint;
	public Vector3 mousePos;
	public Camera cam;
	public GameObject hands;
	public float angle;
	public Vector2 boxSize = new Vector2(4, 6);
	public Vector2 lookDir;
	public LayerMask layerMask;
	void Start()
	{

	}


	void Update()
	{

		MouseLook();
		Debug.DrawRay(player.position, lookDir, Color.magenta);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll(player.transform.position + hands.transform.up * 3, boxSize, angle, layerMask);
			Debug.Log("Enemies: " + enemiesInBox.Length);

			foreach (Collider2D enemy in enemiesInBox)
			{
				Vector3 abNormal = lookDir.normalized;
				Vector3 enemyVec = enemy.transform.position - player.transform.position;

				float dotP = Vector2.Dot(enemyVec, abNormal);

				newPoint = player.transform.position + (abNormal * dotP);
				enemy.GetComponent<ICrowdControllable>()?.Pull(newPoint);
			}

		}

	}

	public void MouseLook()
	{
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		lookDir = mousePos - player.transform.position;
		angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
		hands.transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = Matrix4x4.TRS(player.transform.position + hands.transform.up * 3, hands.transform.rotation, boxSize);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}
}
