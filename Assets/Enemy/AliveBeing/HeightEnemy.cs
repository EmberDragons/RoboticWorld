using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightEnemy : MonoBehaviour
{
    public float height;
    public LayerMask envir;
    Vector2 velocity = Vector2.zero;
    void Start()
    {
        height = (this.GetComponent<Brain>().Size / 2f);
        envir = this.GetComponent<Sens>()._environnement;
    }
    void Update()
    {
        RaycastHit2D hitObj = Physics2D.Raycast(transform.position, -Vector2.up, envir);
        if (hitObj.collider != null)
        {
            float dist = Vector2.Distance(hitObj.collider.transform.position, transform.position);
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(transform.position.x, (transform.position.y - (dist - height))), ref velocity, Mathf.Abs(dist - height)/6f, Mathf.Infinity, Time.deltaTime);
        }
        Debug.DrawRay(transform.position, -Vector2.up, Color.red, height);
    }
}
