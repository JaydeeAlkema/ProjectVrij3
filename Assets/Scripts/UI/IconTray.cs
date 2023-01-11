using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTray : MonoBehaviour
{
	[SerializeField] private int rows = 1;
	private List<Sprite> icons = new List<Sprite>();
	[SerializeField] private Sprite[] sprite;
	private List<GameObject> iconObjects = new List<GameObject>();
	[SerializeField] private GridLayoutGroup glg;
	private int index = 0;

	// Start is called before the first frame update
	void Start()
	{
		index = 0;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void AddNewSpriteToIcons( Sprite icon)
	{
		icons.Add( icon );
		GameObject spriteObject = new GameObject();
		spriteObject.AddComponent<Image>();
		spriteObject.GetComponent<Image>().sprite = icon;
		spriteObject.transform.SetParent(transform);
		spriteObject.transform.localScale = new Vector3( 1, 1, 1 );
		if( index > 1 )
		{
			spriteObject.transform.localPosition = new Vector3( ( GetComponent<RectTransform>().sizeDelta.y / sprite.Length ) * index, 0, 0 );
		}
		else
		{
			spriteObject.transform.localPosition = new Vector3( ( GetComponent<RectTransform>().sizeDelta.y / sprite.Length ) * 1, 0, 0 );
		}
		iconObjects.Add( spriteObject );
		if( iconObjects.Count > rows * 10 )
		{
			glg.cellSize = new Vector2( 50 / ( iconObjects.Count / ( rows * 10 ) ), 50 / ( iconObjects.Count / ( rows * 10 ) ) );
		}
		index++;
	}
}
