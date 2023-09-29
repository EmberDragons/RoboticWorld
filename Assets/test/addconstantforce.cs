using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addconstantforce : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rb2d;
    public float speed = 2f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(0f, speed);
    }
}
