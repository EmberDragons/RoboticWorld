using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proceduralmovement : MonoBehaviour
{
    //raycasts
    public Transform RayCastPoint;
    public GameObject feet;
    public LayerMask Ground;
    public Transform target1;
    public float travelDistance = 8f;
    RaycastHit point_Collision;

    //overlapsphere
    public float legOffset = 0.5f;
    public LayerMask legMask;
    public Transform legPos;
    void Start()
    {
        //tracing Raycasts
        Vector2 direction1 = ((Vector2)(RayCastPoint.position - target1.position)).normalized;
        RaycastHit2D hit1 = Physics2D.Raycast(RayCastPoint.position, -direction1, travelDistance, Ground);
        Debug.DrawRay(RayCastPoint.position, -direction1 * travelDistance, Color.blue);
        //Debug.Log(hit.collider);

        //setting legs pos
        feet.transform.position = hit1.point;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tracing Raycasts
        Vector2 direction = ((Vector2)(RayCastPoint.position - target1.position)).normalized;
        RaycastHit2D hit = Physics2D.Raycast(RayCastPoint.position, -direction, travelDistance, Ground);
        Debug.DrawRay(RayCastPoint.position, -direction * travelDistance, Color.red);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hit.point, legOffset, legMask);
        legPos.position = hit.point;
        int i = hitColliders.Length;
        if (i == 0)
        {
            //setting legs pos
            feet.transform.position = hit.point;
        }
        if(i !=0)
        {
            int e = i-1;
            int nbcolliders = i;
            while(e>=0){

                if (hitColliders[e].name != feet.name)
                {
                    nbcolliders -= 1;
                }
                if(nbcolliders == 0)
                { 
                    //setting legs pos
                    feet.transform.position = hit.point;
                }
                e -= 1;
            }
            
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(legPos.position, legOffset);
    }
}
