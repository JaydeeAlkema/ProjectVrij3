using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveVFX : MonoBehaviour
{
	[SerializeField] private int damage;
	[SerializeField] private Vector3 maxRadius;
	private GameObject shockwave = null;
	private GameObject shockwaveIndicator = null;
	public Material setMaterial = null;

	public int Damage { get => damage; set => damage = value; }

	private void Start()
	{
		DrawShockwave();
	}

	private void Update()
	{
		ExpandShockwave(maxRadius, 4f);
	}

	void DrawShockwave()
	{
		//Draw shockwave
		var drawnCircle = new GameObject { name = "ShockWave" };
		drawnCircle.DrawCircle(1f, 0.04f);
		drawnCircle.GetComponent<LineRenderer>().material = setMaterial;
		drawnCircle.GetComponent<LineRenderer>().startColor = Color.red;
		drawnCircle.GetComponent<LineRenderer>().endColor = Color.red;
		drawnCircle.AddComponent<LineRendererCollision>();
		drawnCircle.GetComponent<EdgeCollider2D>().isTrigger = true;
		drawnCircle.AddComponent<ShockwaveCollision>();
		drawnCircle.GetComponent<ShockwaveCollision>().Damage = damage;
		drawnCircle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		drawnCircle.transform.position = transform.position;
		shockwave = drawnCircle;

		//Draw indicator
		var drawnCircle2 = new GameObject { name = "ShockWaveIndicator" };
		drawnCircle2.DrawCircle(maxRadius.x, 0.04f);
		drawnCircle2.GetComponent<LineRenderer>().material = setMaterial;
		drawnCircle2.GetComponent<LineRenderer>().startColor = Color.white;
		drawnCircle2.GetComponent<LineRenderer>().endColor = Color.white;
		drawnCircle2.transform.position = transform.position;
		shockwaveIndicator = drawnCircle2;
	}

	void ExpandShockwave(Vector3 maxRadius, float expansionSpeed)
	{
		if ((shockwave.transform.localScale.magnitude < maxRadius.magnitude))
		{
			shockwave.transform.localScale += expansionSpeed * Time.deltaTime * Vector3.one;
		}
		else
		{
			Destroy(shockwave);
			Destroy(shockwaveIndicator);
			Destroy(this.gameObject);
		}
	}

}
