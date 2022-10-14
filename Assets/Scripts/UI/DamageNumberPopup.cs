using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumberPopup : MonoBehaviour
{
	public Vector2 flyDir;
	private TextMeshPro textMesh;
	private Color textColor;
	private float damage;
	[SerializeField] private float fadeSpeed = 6f;
	[SerializeField] private float flySpeed = 0.1f;
	[SerializeField] private Vector3 sizeSmall;
	[SerializeField] private float resizeRate = 5f;
	public bool isDisappearing = false;


	void Awake()
	{
		textMesh = GetComponent<TextMeshPro>();
		textColor = textMesh.color;
		flyDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
		sizeSmall = transform.localScale * 0.7f;
		StartCoroutine(ResizeText());
	}


	void Update()
	{
		FlyOff();
		Disappear(isDisappearing);
	}

	public void SetDamageText(float damage)
	{
		textMesh.SetText(damage.ToString());
	}

	void FlyOff()
	{
		transform.position += (Vector3)flyDir.normalized * flySpeed * Time.deltaTime;
	}

	IEnumerator ResizeText()
	{
		yield return new WaitForSeconds(0.1f);
		while (transform.localScale.magnitude > sizeSmall.magnitude)
		{
			transform.localScale -= Vector3.one * resizeRate * Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		transform.localScale = sizeSmall;
		yield return new WaitForSeconds(0.3f);
		isDisappearing = true;
		yield return null;
	}

	void Disappear(bool isDisappearing)
	{
		if (isDisappearing)
		{
			if (textMesh.color.a > 0)
			{
				textColor.a -= fadeSpeed * Time.deltaTime;
				textMesh.color = textColor;
			}
			else
			{
				Destroy(this.gameObject);
			}
		}

	}
}
