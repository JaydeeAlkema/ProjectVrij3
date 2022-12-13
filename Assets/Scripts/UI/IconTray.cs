using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTray : MonoBehaviour
{
    private List<Sprite> icons = new List<Sprite>();
    [SerializeField] private Sprite[] sprite;
    private List<GameObject> iconObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
		for( int i = 0; i < sprite.Length; i++ )
		{
            AddNewSpriteToIcons( sprite[i], i );  
		}
        GetComponent<GridLayoutGroup>().cellSize /= ( icons.Count / 8 ) + 1;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void AddNewSpriteToIcons(Sprite icon, int index)
    {
        icons.Add( icon );
        GameObject spriteObject = new GameObject();
        spriteObject.AddComponent<Image>();
        spriteObject.GetComponent<Image>().sprite = icon;
        spriteObject.transform.parent = this.transform;
        spriteObject.transform.localPosition = new Vector3( (this.GetComponent<RectTransform>().sizeDelta.y / sprite.Length) * index, 0, 0 );
        spriteObject.transform.localScale = new Vector3( 1, 1, 1 );
        iconObjects.Add( spriteObject );
	}
}
