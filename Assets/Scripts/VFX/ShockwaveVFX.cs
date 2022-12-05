using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveVFX : MonoBehaviour
{
	[SerializeField] private int damage;
	[SerializeField] private Vector3 maxRadius;
	[SerializeField] private float expansionSpeed;
	private GameObject shockwave = null;
	private GameObject shockwaveIndicator = null;
	private float waitTimer = 0f;
	public Material setMaterial = null;

	public int Damage { get => damage; set => damage = value; }
	public float ExpansionSpeed { get => expansionSpeed; set => expansionSpeed = value; }
	public Vector3 MaxRadius { get => maxRadius; set => maxRadius = value; }


	private void Start()
	{
		DrawIndicator();
	}

	private void Update()
	{
		if (ExpandCircle(shockwaveIndicator, maxRadius, 50f) && shockwave != null)
		{
			if (ExpandCircle(shockwave, maxRadius, expansionSpeed))
			{
				Destroy(shockwave);
				Destroy(shockwaveIndicator);
				Destroy(this.gameObject);
			}
		}
	}

	//Always draw indicator BEFORE shockwave!
	public void DrawIndicator()
	{
		//Draw indicator
		var drawnCircle2 = new GameObject { name = "ShockWaveIndicator" };
		drawnCircle2.DrawCircle(1f, 0.04f);
		drawnCircle2.GetComponent<LineRenderer>().material = setMaterial;
		drawnCircle2.GetComponent<LineRenderer>().startColor = Color.white;
		drawnCircle2.GetComponent<LineRenderer>().endColor = Color.white;
		drawnCircle2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		drawnCircle2.transform.position = transform.position;
		shockwaveIndicator = drawnCircle2;
	}

	//Only after indicator has been drawn!
	public void DrawShockwave(Color color)
	{
		//Draw shockwave
		var drawnCircle = new GameObject { name = "ShockWave" };
		drawnCircle.DrawCircle(1f, 0.1f);
		drawnCircle.GetComponent<LineRenderer>().material = setMaterial;
		drawnCircle.GetComponent<LineRenderer>().startColor = color;
		drawnCircle.GetComponent<LineRenderer>().endColor = color;
		drawnCircle.AddComponent<LineRendererCollision>();
		drawnCircle.GetComponent<EdgeCollider2D>().isTrigger = true;
		drawnCircle.AddComponent<ShockwaveCollision>();
		drawnCircle.GetComponent<ShockwaveCollision>().Damage = damage;
		drawnCircle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		drawnCircle.transform.position = transform.position;
		shockwave = drawnCircle;
	}


	bool ExpandCircle(GameObject circle, Vector3 maxRadius, float expansionSpeed)
	{
		circle.transform.localScale += expansionSpeed * Time.deltaTime * Vector3.one;
		if (circle.transform.localScale.magnitude < maxRadius.magnitude)
		{
			return false;
		}
		else
		{
			circle.transform.localScale = maxRadius;
			return true;
		}
	}
}
