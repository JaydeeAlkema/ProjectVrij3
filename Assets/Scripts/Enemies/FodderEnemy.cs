using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderEnemy : EnemyBase
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControler>().gameObject;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Vector3 targetDir = player.transform.position - this.transform.position;
        rb2d.velocity = targetDir.normalized * speed * Time.deltaTime;
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
