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
	[SerializeField] private int damage;
	[SerializeField] private float fadeSpeed = 6f;
	[SerializeField] private float flySpeed = 0.1f;
	[SerializeField] private Vector3 sizeSmall = new Vector3(0.4f, 0.4f, 0.4f);
	[SerializeField] private Vector3 sizeBig;
	[SerializeField] private float resizeRate = 5f;
	public bool isDisappearing = false;


	void Awake()
	{
		textMesh = GetComponent<TextMeshPro>();
		textColor = textMesh.color;
		flyDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));

	}

	private void Start()
	{
		textMesh.SetText(damage.ToString());
		//DetermineStartSize(damage);
		StartCoroutine(ResizeText());
	}

	void Update()
	{
		FlyOff();
		Disappear(isDisappearing);
	}

	public void DetermineStartSize(int damage)
	{
		if (damage < 100)
		{
			sizeBig = new Vector3(1f, 1f, 1f);
		}
		else
		{
			sizeBig = new Vector3(damage / 100f, damage / 100f, damage / 100f);
			resizeRate *= (damage / 100);
			if (sizeBig.magnitude > 2f)
			{
				sizeBig = new Vector3(2f, 2f, 2f);
				resizeRate = 10f;
			}
		}
	}

	public void SetDamageText(int damage)
	{
		this.damage = damage;
	}

	void FlyOff()
	{
		transform.position += (Vector3)flyDir.normalized * flySpeed * Time.deltaTime;
	}

	IEnumerator ResizeText()
	{
		transform.localScale = sizeSmall;
		yield return new WaitForEndOfFrame();
		DetermineStartSize(damage);
		transform.localScale = sizeBig;
		yield return new WaitForSeconds(0.15f);
		while (transform.localScale.magnitude > sizeSmall.magnitude && transform.localScale.x > 0f)
		{
			//transform.localScale -= Vector3.Lerp(sizeBig, sizeSmall, Time.deltaTime);
			transform.localScale -= Vector3.one * resizeRate * Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		transform.localScale = sizeSmall;
		yield return new WaitForSeconds(0.15f);
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
