using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public LayerMask _tile;
    [SerializeField] private Color _baseColor, _offsetColor, _groundedColor, _sideColor;
    [SerializeField] private SpriteRenderer _renderer;
    public bool top = false;
    public int posX; 
    public int posY;

    //Pathfinding part
    public bool sender = false;
    float JumpDistance;
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        StartCoroutine(wait());
    }
    public void CheckMove(GameObject Sender, GameObject Target, bool Send, int ourNumber)
    {
        ourNumber++;
        Collider2D[] tile = Physics2D.OverlapCircleAll(transform.position, Sender.GetComponent<Brain>().jumpLength);
        int i = tile.Length;
        float ourDistmax = 999f;
        GameObject ToSend;
        ToSend = null;
        while (i > 0)
        {
            if (tile[i].gameObject.layer == _tile && !Physics2D.Raycast(transform.position, (tile[i].transform.position - transform.position).normalized, Vector2.Distance(tile[i].transform.position, transform.position), LayerMask.NameToLayer("environnement")))
            {
                float ourDist = Vector2.Distance(tile[i].transform.position, transform.position);
                if (ourDist < ourDistmax) ourDistmax = ourDist; ToSend = tile[i].gameObject;
            }
            i--;
        }
        if (ToSend != null) ToSend.GetComponent<Tile>().CheckMove(Sender, Target, Send, ourNumber);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.gameObject.layer == LayerMask.NameToLayer("environnement"))
        {
            Destroy(this.gameObject);
        }
    }
    IEnumerator wait()
    {
        bool no = false;
        yield return new WaitForSeconds(0.005f);
        if (GameObject.Find($"Tile {posX} {posY + 1}") == null)
        {
            _renderer.color = _groundedColor;
            no = true;
        }
        if (GameObject.Find($"Tile {posX} {posY - 1}") == null)
        {
            _renderer.color = _groundedColor;
            top = true;
            no = true;
        }
        if (GameObject.Find($"Tile {posX - 1} {posY}") == null)
        {
            _renderer.color = _sideColor;
            no = true;
        }
        if (GameObject.Find($"Tile {posX + 1} {posY}") == null)
        {
            _renderer.color = _sideColor;
            no = true;
        }
        if(no == false)
        {
            Destroy(this.gameObject);
        }
    }

    public void Pathfind(Transform envoyer, Transform targetObj, bool onlyWalk, float distanceJump, List<GameObject> nbtile)
    {
        bool first = true;
        if (sender == false && !nbtile.Contains(this.gameObject))
        {
            nbtile.Add(this.gameObject);
            sender = true;
            Collider2D[] _tiles = Physics2D.OverlapCircleAll(transform.position, distanceJump, _tile);
            GameObject currentTile;
            JumpDistance = distanceJump;
            if (_tiles.Length > 0)
            {
                int i = _tiles.Length - 1;
                float closest;
                while (i >= 0)
                {
                    if (targetObj.GetComponent<PathReceiver>().currentTile == this.gameObject)
                    {
                        //send back true
                        envoyer.GetComponent<PathTest>().finalList = nbtile;
                        i = -1;
                    }
                    else if (_tiles[i].gameObject.GetComponent<Tile>().top == true)
                    {
                        closest = Vector2.Distance(_tiles[i].transform.position, transform.position);
                        Vector2 Direction = (_tiles[i].transform.position - transform.position).normalized;
                        if (!Physics2D.Raycast(transform.position, Direction, closest, LayerMask.NameToLayer("environnement")))
                        {
                            //set List
                            currentTile = _tiles[i].gameObject;
                            currentTile.GetComponent<Tile>().sender = false;
                            if (first)
                            {
                                first = false;
                                currentTile.GetComponent<Tile>().Pathfind(envoyer, targetObj, onlyWalk, distanceJump, nbtile);
                            }
                            else
                            {
                                List<GameObject> ourList = nbtile;
                                currentTile.GetComponent<Tile>().Pathfind(envoyer, targetObj, onlyWalk, distanceJump, ourList);
                            }
                        }
                    }
                    i--;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(JumpDistance > 0)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, JumpDistance);
        }
    }
}