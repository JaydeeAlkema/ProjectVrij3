using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderEnemy : EnemyBase
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControler>().gameObject;
    }

    // Update is called once per frame
    new void Update()
    {
        Vector3 targetDir = player.transform.position - this.transform.position;
        float angle = Mathf.Atan2( targetDir.y, targetDir.x ) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        transform.Translate( Vector3.right * speed * Time.fixedDeltaTime ); 
    }

    void AttackPlayer(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerControler>().TakeDamage(damage);
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
        if( collision.gameObject.layer == 8 )
        {
            AttackPlayer( collision.gameObject );
        }
    }
}
