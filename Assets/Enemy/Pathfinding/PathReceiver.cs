using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathReceiver : MonoBehaviour
{
    //private
    public LayerMask _tile;
    public LayerMask _environnement;
    public GameObject currentTile;
    public float JumpDistance;
    public bool e = false;
    // Start is called before the first frame update
    void Start()
    {
        _tile = LayerMask.GetMask("tile");
        _environnement = LayerMask.GetMask("environnement");
        if (this.GetComponent<Brain>() != null)
            JumpDistance = this.GetComponent<Brain>().jumpLength;
        else JumpDistance = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        if (e == false)
            StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        e = true;
        Collider2D[] _tiles = Physics2D.OverlapCircleAll(transform.position, JumpDistance, _tile);
        float closest = 999f;
        if (_tiles.Length > 0)
        {
            int i = _tiles.Length - 1;
            while (i >= 0)
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
                i--;
            }
        }
        yield return new WaitForSeconds(0.05f);
        e = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, JumpDistance);
    }
}
