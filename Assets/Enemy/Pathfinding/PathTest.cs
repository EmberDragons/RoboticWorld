using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    float Vitesse;
    public LayerMask _tile;
    public LayerMask _environnement;
    public bool canJump;
    public float JumpDistance;

    public Transform pos;
    public bool canFly;
    public bool canWalkOnWall;
    public List<GameObject> finalList = new List<GameObject>();
    bool continu = false;
    //private
    public GameObject currentTile;
    private int i = 0;
    private bool end = true;
    Brain brain;
    Transform target;
    bool onece = true;
    bool Stopwalking = false;
    // Start is called before the first frame update
    void Start()
    {
        brain = this.GetComponent<Brain>();
        JumpDistance = brain.jumpLength;
        Vitesse = 10f * brain.Speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (brain.Target != null)
        { 
            if (pos != brain.Target)
            {
                pos = brain.Target;
                if (finalList != null){
                    finalList.Clear();
                }
                StartCoroutine(Wait());
                
            }
        }
        //launch Wait()
        

        if (finalList.Count!=0)
            if (finalList[i].gameObject != null)
                GoTo();
    }
    void GoTo()
    {
        //BodyRotation
        float movementDirection = (finalList[i].transform.position.x - transform.position.x); 
        
        if (brain.underAttack != true)
        {
            if (transform.rotation.y > -5f && transform.rotation.y < 5f)
                transform.Rotate(new Vector3(0f, 0f, transform.rotation.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, movementDirection)), Time.deltaTime * 40f);
        }
        if (transform.position.x <= finalList[i].transform.position.x + (GetComponent<Brain>().Size / 2f) && transform.position.x >= finalList[i].transform.position.x - (GetComponent<Brain>().Size / 2f) && end)
        {
            if (transform.position.y <= finalList[i].transform.position.y + (GetComponent<Brain>().Size / 2f) && transform.position.y >= finalList[i].transform.position.y - (GetComponent<Brain>().Size / 2f))
            {
                if (i == (finalList.Count - 1))
                {
                    if (brain.FollowingSmell == null && brain.fighting == false && brain.eating == false && brain.alive == false && brain.underAttack == false) brain.Roam();
                    //if (brain.destination.activeInHierarchy == true) brain.destination.SetActive(false);
                    end = false;
                    i = 0;
                }
                else
                    i++;
            }
        }
        else
        {
            //walk or jump?
            if (i == 0)
            {
                //jump
                if (finalList[i].transform.position.y != currentTile.transform.position.y && onece)
                {
                    onece = false;
                    target = finalList[i].transform;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2((finalList[i].transform.position.x - transform.position.x), JumpDistance), ForceMode2D.Impulse);
                    StartCoroutine(WaitForOnece());
                    RaycastHit2D hitObj = Physics2D.Raycast(transform.position, -Vector2.up, _environnement);
                    if (hitObj.collider != null && hitObj.distance<=0.1f)
                    {
                        onece = true;
                    }
                }
                //walk
                else if (Stopwalking == false)
                {
                    Vector2 pos = Vector2.MoveTowards(transform.position, finalList[i].transform.position, Vitesse * Time.deltaTime);
                    GetComponent<Rigidbody2D>().MovePosition(pos);
                }
            }
            else
            {
                //jump
                if (finalList[i].transform.position.y != finalList[i - 1].transform.position.y && onece)
                {
                    onece = false;
                    target = finalList[i].transform;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2((finalList[i].transform.position.x - transform.position.x), JumpDistance), ForceMode2D.Impulse);
                    StartCoroutine(WaitForOnece());
                }
                //walk
                else if(Stopwalking == false)
                {
                    Vector2 pos = Vector2.MoveTowards(transform.position, finalList[i].transform.position, Vitesse * Time.deltaTime);
                    GetComponent<Rigidbody2D>().MovePosition(pos);
                }
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        currentTile = null;
        Collider2D[] _tiles = Physics2D.OverlapCircleAll(transform.position, JumpDistance, _tile);
        float closest = 999f;
        if (_tiles.Length > 0)
        {
            int i = _tiles.Length - 1;
            while (i >= 0)
            {
                if (canFly == false && canWalkOnWall == false)
                {
                    if (_tiles[i].GetComponent<Tile>().top == true)
                    {
                        Vector2 Direction = (_tiles[i].transform.position - transform.position).normalized;
                        if (!Physics2D.Raycast(transform.position, Direction, Vector2.Distance(_tiles[i].transform.position, transform.position), _environnement) && closest > Vector2.Distance(_tiles[i].transform.position, transform.position))
                        {
                            closest = Vector2.Distance(_tiles[i].transform.position, transform.position);
                            currentTile = _tiles[i].gameObject;
                        }
                    }
                }
                else if (canWalkOnWall == true)
                {

                }
                i--;
            }
            if (currentTile != null)
            {
                currentTile.gameObject.GetComponent<Tile>().sender = false;
                currentTile.gameObject.GetComponent<Tile>().Pathfind(transform, pos.transform, true, JumpDistance, new List<GameObject>());
            }
            continu = true;
        }
    }
    IEnumerator WaitForOnece()
    {
        Stopwalking = true;
        yield return new WaitForSeconds(1.5f);
        Stopwalking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, JumpDistance);

        Gizmos.color = Color.green;
        if (brain != null)
             Gizmos.DrawCube(new Vector2(transform.position.x - (brain.Size / 2f), transform.position.y - (brain.Size / 2f)), new Vector2(0.6f, 1.2f));
    }
}
