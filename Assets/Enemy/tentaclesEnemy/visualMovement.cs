using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualMovement : MonoBehaviour
{
    public Transform[] tentacles;
    public Transform[] tentaclesEnd;
    public Transform[] tentaclesAim;
    public List<bool> move;
    public List<bool> isMoving;
    public float lineWidth;
    public float ropeCurve;
    public int LengthOfTheRayacast;
    public int LengthOfTheTentacles;
    public float legOffset = 0.5f;
    LayerMask environement;
    public float speed = 300f;
    RaycastHit2D hit;
    Vector2 Target;
    private float Normalspeed;


    //linerenderer part (not so usefull)
    public List<LineRenderer> line; //the lineRenderer we will use to show the tentacles
    public Vector3[] segmentPoses; //all The vector3 pos for the line Renderer


    //gravity interaction
    public float gravity;
    public int nbLegGrounded;
    private int maxnbLegGrounded;
    private float multiplicaterLeg;
    private void Start()
    {
        Normalspeed = speed;
        segmentPoses = new Vector3[LengthOfTheTentacles];
        environement = GetComponent<Sens>()._environnement;
        for(int i = 0; i < tentacles.Length; i++)
        {
            move.Add(false);
            isMoving.Add(false);
        }
        for (int i = 0; i < tentacles.Length; i++)
        {
            line.Add(tentacles[i].GetComponent<LineRenderer>());
            line[i].positionCount = LengthOfTheTentacles;
        }
        maxnbLegGrounded = tentacles.Length;
    }
    private void Update()
    {
        //gravity Interactions
        if (nbLegGrounded != 0)
            multiplicaterLeg = (maxnbLegGrounded / nbLegGrounded*0.75f) -1f;
        else
            multiplicaterLeg = maxnbLegGrounded-2.75f;
        gravity = multiplicaterLeg;
        if (gravity < 0f) gravity = 0f;
        GetComponent<Rigidbody2D>().gravityScale = gravity;

        //for each vector find a point
        SetTheGroundedLegs();
        if (tentacles != null)
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                findTentacleEnd(i);
                PhysicsBackground(i);
                line[i].gameObject.GetComponent<RopeBridge>().StartPoint = tentacles[i];
                line[i].gameObject.GetComponent<RopeBridge>().EndPoint = tentaclesEnd[i];
                line[i].gameObject.GetComponent<RopeBridge>().lineWidth = lineWidth;
            }
        }
    }
    void SetTheGroundedLegs()
    {
        nbLegGrounded = 0;
        for (int y = 0; y < tentacles.Length; y++)
        {
            if(isMoving[y] == false)
            {
                nbLegGrounded++;
            } 
        }
    }
    void findTentacleEnd(int i)
    {
        Vector2 direction = ((Vector2)(tentacles[i].position - tentaclesAim[i].position)).normalized;
        hit = Physics2D.Raycast(tentacles[i].position, -direction, LengthOfTheRayacast, environement);
        Debug.DrawRay(tentacles[i].position, -direction * LengthOfTheRayacast, Color.blue);
        
        
        if (!hit)
        {
            Target = new Vector2(tentaclesAim[i].position.x, tentaclesAim[i].position.y);
        }
        else Target = hit.point;

        float dist = Vector2.Distance(tentaclesEnd[i].position, Target);

        if (dist > legOffset)
        {
            //setting back legs pos
            move[i] = true;
        }
        if (dist < 0.2f && hit)
        {
            move[i] = false;
            isMoving[i] = false;
        }
        if (move[i])
        {
            moveToPoint(i);
        }
    }
    void moveToPoint(int i)
    {
        //get back to the middle distance between the body and ourTarget going to the point
        Vector2 middleDist = new Vector2((transform.position.x + Target.x + 2) / 2, (transform.position.y + Target.y + 2) / 2);
        if (isMoving[i])
        {
            tentaclesEnd[i].position = Vector2.Lerp(tentaclesEnd[i].position, Target, speed * Time.deltaTime);
            StartCoroutine(SpeedUp(tentacles[i].position, Target));
        }
        else
        {
            tentaclesEnd[i].position = Vector2.Lerp(tentaclesEnd[i].position, middleDist, 1.5f * speed * Time.deltaTime);
            StartCoroutine(Wait(i));
        }
        //set the rope length

        float y = Vector2.Distance(tentacles[i].position, tentaclesEnd[i].position);
        float x = y/40f;
        line[i].gameObject.GetComponent<RopeBridge>().ropeSegLen = x;
    }
    void PhysicsBackground(int i)
    {
        //1.select the position of the points with a sinus function how ????? idk
        Vector3[] segmentEndPoints = new Vector3[LengthOfTheTentacles];
        segmentPoses[0] = tentacles[i].position;

        //here we find the values for segmentEndPoints

        //2.set the points positions in the line renderer to the segmentEndPoints
        for (int y = 1; y < segmentPoses.Length; y++)
        {
            float dist = Vector3.Distance(segmentEndPoints[y], segmentPoses[y]);
            segmentPoses[y] = Vector2.Lerp(segmentPoses[y], segmentEndPoints[y], dist * speed * Time.deltaTime);
        }
    }
   
    
    IEnumerator Wait(int i)
    {
        yield return new WaitForSeconds(0.3f);
        isMoving[i] = true;
    }
    IEnumerator SpeedUp(Vector2 firstPos,Vector2 SecondPos)
    {
        yield return new WaitForSeconds(1f);
        if(Vector2.Distance(firstPos, SecondPos) > 0.2f) StartCoroutine(SpeedCramp(firstPos,SecondPos)); 
        speed = Normalspeed;
    }
    IEnumerator SpeedCramp(Vector2 firstPos, Vector2 SecondPos)
    {
        while (Vector2.Distance(firstPos, SecondPos) > 0.2f)
        {
            yield return new WaitForSeconds(0.1f);
            speed = 15f;
        }
    }

}
