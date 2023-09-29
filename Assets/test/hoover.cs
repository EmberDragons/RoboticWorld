using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoover : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform frontDown;
    public Transform frontUp;

    public Transform backDown;
    public Transform backUp;

    public float travelDistance = 1f;
    public LayerMask mask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //front
        if(Physics2D.Raycast(frontDown.position, Vector2.right, travelDistance, mask) && Physics2D.Raycast(frontUp.position, Vector2.right, travelDistance, mask)) {
            frontDown.Rotate(0f,0f,19f);
        }
        Debug.DrawRay(frontDown.position, Vector2.right * travelDistance, Color.red);
    }
}
