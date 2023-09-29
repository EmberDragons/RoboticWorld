using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    private float activateDistance = 1000f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    RaycastHit2D isGrounded;
    Seeker seeker;
    Brain brain;
    Rigidbody2D rb;

    //private stuff
    private bool timeToSwitch = true;

    //jump variable
    public bool canJump = true;

    public void Start()
    {
        brain = GetComponent<Brain>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (target == null) target = brain.destination;
        speed = brain.Speed * 250f;
        if (brain != null)
            if (brain.Target != null)
                target = brain.Target;
            else target = brain.transform;
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
        UpdateFall();
    }
    void UpdateFall()
    {
        rb.AddForce(new Vector2(0f, -20f));
    }
    private void UpdatePath()
    {
        if (target!= null && followEnabled && TargetInDistance() && seeker.IsDone() && rb!=null)
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (rb != null)
        {
            if (path == null)
            {
                return;
            }

            // Reached end of path
            if (currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }

            // See if colliding with anything
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            Vector3 startOffset = transform.position - new Vector3(0f, boxCollider2D.bounds.extents.y + jumpCheckOffset);
            isGrounded = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector3(boxCollider2D.bounds.size.x + 0.3f, boxCollider2D.bounds.size.y+0.3f), 0f, Vector2.down, 0.05f, GetComponent<Sens>()._environnement);


            // Direction Calculation
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime * 2f;

            // Jump
            if (jumpEnabled && isGrounded)
            {
                if (direction.y > jumpNodeHeightRequirement)
                {
                    CheckTheJump();
                }
            }

            // Movement
            //flying enemies
            if(brain.canFly)
            {
                float X = Mathf.Lerp(rb.velocity.x, force.x, Time.deltaTime);
                float Y = Mathf.Lerp(rb.velocity.y, force.y, Time.deltaTime);
                rb.velocity = new Vector2(X, Y);
            }
            //else
            else
            {
                float X = Mathf.Lerp(rb.velocity.x, force.x, Time.deltaTime);
                rb.velocity = new Vector2(X, rb.velocity.y);
                rb.AddForce(new Vector2(0f, force.y));
            }

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Direction Graphics Handling
            if (directionLookEnabled && timeToSwitch)
            {
                if (rb.velocity.x > 0.77f)
                {
                    transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    StartCoroutine(SwitchTime());
                }
                else if (rb.velocity.x < -0.77f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    StartCoroutine(SwitchTime());
                }
            }
        }
    }
    IEnumerator JumpTime()
    {
        yield return new WaitForSeconds(.5f);
        canJump = true;
    }
    IEnumerator SwitchTime()
    {
        timeToSwitch = false;
        yield return new WaitForSeconds(0.07f);
        timeToSwitch = true;
    }
    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }
    private void CheckTheJump()
    {
        bool findNumber = false;
        int WayPoint = currentWaypoint;
        while (findNumber == false && (path.vectorPath.Count-1) >= WayPoint && canJump && rb != null && GetComponent<Brain>().canFly == false)
        {
            if (path.vectorPath[WayPoint].y != path.vectorPath[currentWaypoint].y && canJump)
            {
                canJump = false;
                float jumpForce = ((WayPoint - currentWaypoint) / 5f) * jumpModifier * 25000f;
                if (jumpForce > (1000f + 15 * (WayPoint - currentWaypoint))) jumpForce = 1000f + 15 * (WayPoint - currentWaypoint);
                rb.AddForce(Vector2.up * jumpForce);
                StartCoroutine(JumpTime());
                findNumber = true;
            }
            else
                WayPoint++;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
