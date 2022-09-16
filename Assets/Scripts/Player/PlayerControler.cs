using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    private int horizontal = 0;
    private int vertical = 0;
    [SerializeField]
    private float moveSpeed = 1;
    [SerializeField]
    private float vel = 0;

    private Rigidbody2D rb2d = default;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = ( int )Input.GetAxisRaw( "Horizontal" );
        vertical = ( int )Input.GetAxisRaw( "Vertical" );

        rb2d.velocity = new Vector3( horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime ).normalized * moveSpeed;
        vel = rb2d.velocity.magnitude;
    }
}
