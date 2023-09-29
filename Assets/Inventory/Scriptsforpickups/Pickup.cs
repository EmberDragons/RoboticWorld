using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Color colorToSelect;
    private Inventory inventory;
    public GameObject itemButton;
    //private stuff
    private Color color;
    public bool canBeTaken = true;
    private bool canSwitchColor = true;
    private GameObject player;
    //attack
    public float damage;
    public bool canAttack = false;
    public LayerMask _alive;
    public float attackRangeX, attackRangeY;
    public bool rotateTovelocity = false;
    public bool metalic = true;
    private void Start()
    {
        color = this.GetComponent<SpriteRenderer>().color;
        attackRangeX = transform.localScale.x;
        attackRangeY = transform.localScale.y;
    }
    private void Update()
    {
        if (rotateTovelocity)
        {
            RotateToVelocity();
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        //change to a key pressed to take the weapon / other thinggy
        if (col.transform.parent != null && col.transform.parent.transform.GetChild(0).gameObject.GetComponent<Inventory>() != null)
        {
            player = col.transform.parent.transform.GetChild(0).gameObject;
            if (canBeTaken)
            {
                if (Input.GetKey(player.GetComponent<InventoryNavigation>().KeyGrab))
                {
                    GetComponent<Rigidbody2D>().simulated = true;
                    if (gameObject.activeSelf) this.GetComponent<SpriteRenderer>().color = color;
                    inventory = col.transform.parent.transform.GetChild(0).gameObject.GetComponent<Inventory>();
                    if(inventory.isFull[inventory.transform.GetComponent<InventoryNavigation>().slotSelected] == false)
                    {
                        int i = inventory.transform.GetComponent<InventoryNavigation>().slotSelected;
                        inventory.objRef[i] = gameObject;
                        inventory.objRef[i].transform.SetParent(inventory.transform);
                        inventory.isFull[i] = true;
                        GameObject obj = Instantiate(itemButton, inventory.slots[i].transform, false);
                        inventory.pickUps[i] = obj;
                        inventory.objRef[i].transform.rotation = inventory.selectedHand.rotation;
                        inventory.objRef[i].transform.position = inventory.selectedHand.position + (inventory.selectedHand.transform.up * inventory.objRef[i].transform.localScale.y);

                        if (inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected] != null && inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected] == transform.gameObject)
                            inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected].SetActive(true);
                        else gameObject.SetActive(false);
                        canBeTaken = false;
                    }
                    else
                    {
                        for (int i = 0; i < inventory.slots.Length; i++)
                        {
                            if (inventory.isFull[i] == false)
                            {
                                inventory.objRef[i] = gameObject;
                                inventory.objRef[i].transform.SetParent(inventory.transform);
                                inventory.isFull[i] = true;
                                GameObject obj = Instantiate(itemButton, inventory.slots[i].transform, false);
                                inventory.pickUps[i] = obj;
                                inventory.objRef[i].transform.rotation = inventory.selectedHand.rotation;
                                inventory.objRef[i].transform.position = inventory.selectedHand.position + (inventory.selectedHand.transform.up * inventory.objRef[i].transform.localScale.y);

                                if (inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected] != null && inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected] == transform.gameObject)
                                    inventory.objRef[inventory.transform.GetComponent<InventoryNavigation>().slotSelected].SetActive(true);
                                else gameObject.SetActive(false);
                                canBeTaken = false;
                                break;
                            }
                        }
                    }
                }
                if (canSwitchColor && gameObject.activeSelf)
                {
                    canSwitchColor = false;
                    this.GetComponent<SpriteRenderer>().color = colorToSelect;
                    StartCoroutine(wait());
                }

            }
        }
        if (player != null && Vector2.Distance(player.transform.position, transform.position) > 2f)
        {
            canSwitchColor = true;
        }
    }
    void RotateToVelocity()
    {
        if (GetComponent<Rigidbody2D>().velocity.x > 0)
        {
            var dir = GetComponent<Rigidbody2D>().velocity;
            var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            var q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 10f * Time.deltaTime);
        }
        else
        {
            var dir = GetComponent<Rigidbody2D>().velocity;
            var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            angle *= -1;
            angle += 180;
            var q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 10f * Time.deltaTime);
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.07f);
        this.GetComponent<SpriteRenderer>().color = color;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //throwedAttacks with spear
        if (collision.gameObject.layer == LayerMask.NameToLayer("environnement"))
        {
            FindObjectOfType<AudioManager>().Play("SpearHit");
            transform.Translate(-Vector2.up * 0.3f);
            rotateTovelocity = false;
            GetComponent<Collider2D>().isTrigger = false;
            transform.SetParent(collision.transform);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            gameObject.layer = LayerMask.NameToLayer("itemOnGround");
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("alive") && collision.GetComponent<EnemyAI>() != null)
        {
            FindObjectOfType<AudioManager>().Play("FleshSound");
            collision.gameObject.GetComponent<Brain>()._Live -= (damage * 1.5f);
            if (collision.gameObject.GetComponent<Brain>().canFly == true) collision.gameObject.GetComponent<Brain>().Fall();
            transform.Translate(-Vector2.up * 0.3f);
            rotateTovelocity = false;
            GetComponent<Collider2D>().isTrigger = false;
            transform.SetParent(collision.transform);
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("itemOnGround");
            collision.transform.GetChild(1).transform.rotation = Quaternion.LookRotation(collision.transform.position - transform.position, Vector2.up);
            collision.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //normalAttacks
        if (canAttack)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("alive") && collision.gameObject.GetComponent<EnemyAI>() != null)
            {
                if(transform.position.x > collision.transform.position.x)
                    Instantiate(GameObject.Find("Manager").GetComponent<Storage>().hitEffect, transform.position + new Vector3(-1f,1f,0f), Quaternion.identity);
                else
                    Instantiate(GameObject.Find("Manager").GetComponent<Storage>().hitEffect, transform.position + new Vector3(1f, 1f,0f), Quaternion.identity);
                FindObjectOfType<AudioManager>().Play("FleshSound");
                FindObjectOfType<HitPause>().PauseHit(0.02f);
                collision.gameObject.GetComponent<Brain>()._Live -= damage;
                collision.transform.GetChild(1).transform.rotation = Quaternion.LookRotation(collision.transform.position-transform.position, Vector2.up);
                collision.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
        }
        //throwedAttacks //no spear
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("alive") && collision.gameObject.GetComponent<EnemyAI>() != null)
            {
                FindObjectOfType<HitPause>().PauseHit(0.01f);
                FindObjectOfType<CameraShake>().ShakeCamera(0.35f, 0.2f);
                collision.gameObject.GetComponent<Brain>()._Live -= (damage * 1.5f);
                if (collision.gameObject.GetComponent<Brain>().canFly == true) collision.gameObject.GetComponent<Brain>().Fall();
                FindObjectOfType<AudioManager>().Play("FleshSound");
                collision.transform.GetChild(1).transform.rotation = Quaternion.LookRotation(collision.transform.position - transform.position, Vector2.up);
                collision.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("environnement") && gameObject.layer != LayerMask.NameToLayer("itemOnGround") && metalic)
            {
                FindObjectOfType<HitPause>().PauseHit(0.06f);
                FindObjectOfType<CameraShake>().ShakeCamera(0.5f, 0.2f);
                FindObjectOfType<AudioManager>().Play("MetallicHit");
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("environnement") && gameObject.layer != LayerMask.NameToLayer("itemOnGround") && metalic == false)
            {
                FindObjectOfType<HitPause>().PauseHit(0.06f);
                FindObjectOfType<CameraShake>().ShakeCamera(0.5f, 0.2f);
                FindObjectOfType<AudioManager>().Play("GroundHit");
            }
            gameObject.layer = LayerMask.NameToLayer("itemOnGround");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRangeX, attackRangeY, 1));
    }
}
