using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followplayer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public float strength;
    public int i = 0;
    Rigidbody2D rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.position - transform.position;
        rb.velocity = new Vector2(direction.x * strength, direction.y * strength);
        if (this.gameObject.transform.GetChild(i) != null)
        {
            Vector2 newdirection = player.gameObject.transform.GetChild(i).transform.position - transform.GetChild(i).transform.position;
            Rigidbody2D rbChild = this.gameObject.transform.GetChild(i).GetComponent<Rigidbody2D>();
            rbChild.velocity = new Vector2(newdirection.x * strength, newdirection.y * strength);
            i++;
        }
        if (i>11)
        {
            i = 0;
        }

    }
}
