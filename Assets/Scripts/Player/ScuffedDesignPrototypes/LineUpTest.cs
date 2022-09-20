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
	public GameObject CastFromPoint;
	public float angle;
	public Vector2 boxSize = new Vector2(4, 6);
	public float circleSize = 3f;
	public Vector2 lookDir;
	public LayerMask layerMask;

	void Update()
	{

		MouseLook();
		Debug.DrawRay(player.position, lookDir, Color.magenta);

		if (Input.GetKeyDown(KeyCode.E))
		{
			LineUp();
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			BlackHole();
		}

	}
	public void MouseLook()
	{
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		lookDir = mousePos - player.transform.position;
		angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
		CastFromPoint.transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void LineUp()
	{
		Collider2D[] enemiesInBox = Physics2D.OverlapBoxAll(player.transform.position + CastFromPoint.transform.up * 3, boxSize, angle, layerMask);
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

	public void BlackHole()
	{
		Vector2 circlePos = player.transform.position + CastFromPoint.transform.up * 5;
		Collider2D[] enemiesInCircle = Physics2D.OverlapCircleAll(circlePos, circleSize, layerMask);
		Debug.Log("Enemies: " + enemiesInCircle.Length);

		foreach (Collider2D enemy in enemiesInCircle)
		{
			newPoint = circlePos;
			enemy.GetComponent<ICrowdControllable>()?.Pull(newPoint);
		}

	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = Matrix4x4.TRS(player.transform.position + CastFromPoint.transform.up * 3, CastFromPoint.transform.rotation, boxSize);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.TRS(player.transform.position + CastFromPoint.transform.up * 5, CastFromPoint.transform.rotation, new Vector3(circleSize, circleSize, 0));
		Gizmos.DrawWireSphere(Vector3.zero, 1);
	}
}
