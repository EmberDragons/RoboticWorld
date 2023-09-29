using System.Collections;
using System;
using System.Collections.Generic;
using TarodevController;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.Rendering;
using static UnityEngine.EventSystems.EventTrigger;

public class InventoryNavigation : MonoBehaviour
{
    public KeyCode KeyGrab;
    public KeyCode KeyUse;
    public KeyCode KeyThrow;
    private Inventory inventory;
    public Transform Highlight;
    public Transform SpawnThrow;
    public int slotSelected;
    public float x;
    public float y;
    private float timeBtwAttack;
    public LayerMask _alive;
    private Transform oldHand;

    public float timerHolding;
    // Start is called before the first frame update
    void Start()
    {
        inventory = gameObject.GetComponent<Inventory>();
        oldHand = inventory.selectedHand;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.objRef[slotSelected] != null)
        {
            inventory.objRef[slotSelected].transform.position = inventory.selectedHand.position + (inventory.selectedHand.transform.up * inventory.objRef[slotSelected].transform.localScale.y);
            inventory.objRef[slotSelected].transform.rotation = inventory.selectedHand.rotation;

        }
        for (int i = 0; i <= inventory.slots.Length; i++)
        {
            if(Input.GetKeyDown(""+i))
            {
                if (inventory.objRef[slotSelected] != null) inventory.objRef[slotSelected].SetActive(false);
                Highlight.position = inventory.slots[i-1].transform.position;
                slotSelected = i-1;
                if (inventory.pickUps[slotSelected] != null)
                    inventory.objRef[slotSelected].SetActive(true);
            }
        }
        if (inventory.objRef[slotSelected] != null && oldHand != null && oldHand != inventory.selectedHand)
        {
            oldHand.GetComponent<Animator>().SetBool("swordAttack", false);
            oldHand.GetComponent<Animator>().SetBool("spearAttack", false);
            oldHand.GetComponent<Animator>().SetBool("eating", false);
            inventory.objRef[slotSelected].GetComponent<Pickup>().canAttack = false;
            timeBtwAttack = 0.1f;
            timerHolding = 0f;
            oldHand = inventory.selectedHand;
        }
        if (Input.GetKeyDown(KeyThrow))
        {
            Throw();
        }
        //check if can Attack
        if (timeBtwAttack <= 0 && inventory.pickUps[slotSelected] != null && inventory.pickUps[slotSelected].GetComponent<UIvariables>() != null)
        {
            //check if key is pressed
            if (Input.GetKeyDown(KeyUse))
            {
                //then attack
                Attack();
                if(inventory.pickUps[slotSelected] != null) timeBtwAttack = 1 / (inventory.pickUps[slotSelected].GetComponent<UIvariables>().speed + (GetComponent<PlayerBrainReader>().speed));
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        //forEating
        if (Input.GetKey(KeyUse) && inventory.pickUps[slotSelected] != null)
        {
            timerHolding += Time.deltaTime; 
            if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().weapon == false)
                if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "fruit" || inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "meat")
                    inventory.selectedHand.GetComponent<Animator>().SetBool("eating", true);

            if (timerHolding >= inventory.pickUps[slotSelected].GetComponent<UIvariables>().TimeToEat) Use();
        }
        if (Input.GetKeyUp(KeyUse) && inventory.selectedHand != null)
        {
            timerHolding = 0f;
            GameObject.Find("HandLeft").transform.GetComponent<Animator>().SetBool("eating", false);
            GameObject.Find("HandRight").transform.GetComponent<Animator>().SetBool("eating", false);
        }

    }
    //throw
    void Throw()
    {
        if (inventory.pickUps[slotSelected] != null && inventory.isFull[slotSelected] != false)
        {
            FindObjectOfType<CameraShake>().ShakeCamera(0.35f,0.2f);
            inventory.isFull[slotSelected] = false;
            float nbToAdd = 0f;
            if (inventory.transform.GetComponent<PlayerController>()._currentHorizontalSpeed > 0f) nbToAdd = 0.5f;
            if (inventory.transform.GetComponent<PlayerController>()._currentHorizontalSpeed < 0f) nbToAdd = -0.5f;
            if (inventory.transform.GetComponent<PlayerController>()._currentHorizontalSpeed == 0f)
            {
                if(inventory.selectedHand.position.x > inventory.transform.position.x) nbToAdd = 0.5f;
                else nbToAdd = -0.5f;
            }
            if (nbToAdd > 0f)
            {
                GameObject obj = Instantiate(inventory.pickUps[slotSelected].GetComponent<UIvariables>().Object, new Vector3(SpawnThrow.transform.position.x + (nbToAdd/2.5f), inventory.transform.position.y, 0f), Quaternion.Euler(0,0,90), null);
                CodeToRun(obj, nbToAdd);
            }
            if (nbToAdd < 0f)
            {
                GameObject obj = Instantiate(inventory.pickUps[slotSelected].GetComponent<UIvariables>().Object, new Vector3(SpawnThrow.transform.position.x + (nbToAdd/2.5f), inventory.transform.position.y, 0f), Quaternion.Euler(0, 0, -90), null);
                CodeToRun(obj, nbToAdd);
            }
            Destroy(inventory.objRef[slotSelected]);
        }
    }
    void CodeToRun(GameObject obj, float nbToAdd)
    {
        FindObjectOfType<AudioManager>().Play("Throw");
        FindObjectOfType<HitPause>().PauseHit(0.02f);
        obj.GetComponent<Pickup>()._alive = _alive;
        obj.GetComponent<Pickup>().damage = (inventory.pickUps[slotSelected].GetComponent<UIvariables>().damage + GetComponent<PlayerBrainReader>().strength);

        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(nbToAdd * x, Mathf.Abs(nbToAdd * y)));
        obj.GetComponent<Pickup>().canBeTaken = false;
        StartCoroutine(Wait("canBeTaken", obj, 0.2f));
        obj.layer = LayerMask.NameToLayer("item");
        //for spear or rocs
        if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "roc")
        {
            obj.GetComponent<Pickup>().metalic = false;
        }
        if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "spear")
        {
            obj.GetComponent<Collider2D>().isTrigger = true;
            obj.GetComponent<Pickup>().rotateTovelocity = true;
        }
        Destroy(inventory.pickUps[slotSelected]);
        inventory.pickUps[slotSelected] = null;
    }

    //attack/eat/consume
    void Attack()
    {

        if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().weapon)
        {
            FindObjectOfType<CameraShake>().ShakeCamera(0.25f, 0.2f);
            //weapons
            inventory.objRef[slotSelected].layer = LayerMask.NameToLayer("item");
            inventory.objRef[slotSelected].GetComponent<Pickup>()._alive = _alive;
            inventory.selectedHand.GetComponent<Animator>().speed = (inventory.pickUps[slotSelected].GetComponent<UIvariables>().speed + GetComponent<PlayerBrainReader>().speed);
            inventory.selectedHand.GetComponent<HandsAnimHandler>().pickUp = inventory.objRef[slotSelected];
            inventory.selectedHand.GetComponent<HandsAnimHandler>().Inventory = transform.gameObject;
            inventory.objRef[slotSelected].GetComponent<Pickup>().damage = (inventory.pickUps[slotSelected].GetComponent<UIvariables>().damage + GetComponent<PlayerBrainReader>().strength);
            inventory.objRef[slotSelected].GetComponent<Pickup>().canAttack = true;
            //sword
            if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "sword")
            {
                inventory.selectedHand.GetComponent<Animator>().SetBool("swordAttack", true);
                FindObjectOfType<AudioManager>().Play("SwordAttackSound");
            }
            else if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "spear")
            {
                inventory.selectedHand.GetComponent<Animator>().SetBool("spearAttack", true);
                FindObjectOfType<AudioManager>().Play("SpearAttackSound");
            }
            else if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "roc")
            {
                inventory.selectedHand.GetComponent<Animator>().SetBool("swordAttack", true);
                FindObjectOfType<AudioManager>().Play("RocAttackSound");
            }
        }
    }
    void Use()
    {
        if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().weapon == false)
        {
            //speed++ / fruit
            if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "fruit")
            {
                if (FindObjectOfType<FillAmount>().Fill[0].RealScroll + FindObjectOfType<FillAmount>().Fill[1].RealScroll + inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth < 1)
                {
                    FindObjectOfType<FillAmount>().Fill[0].RealScroll += inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth;
                }
                else
                {
                    if (FindObjectOfType<FillAmount>().Fill[0].RealScroll + inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth > 1) FindObjectOfType<FillAmount>().Fill[0].RealScroll = 1;
                    else FindObjectOfType<FillAmount>().Fill[0].RealScroll += inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth;
                    FindObjectOfType<FillAmount>().Fill[1].RealScroll = 1 - FindObjectOfType<FillAmount>().Fill[0].RealScroll;
                }
                FindObjectOfType<FillAmount>().CalculateSize();
            }
            //strength++ / meat
            else if (inventory.pickUps[slotSelected].GetComponent<UIvariables>().TypeOfWeapon == "meat")
            {
                if (FindObjectOfType<FillAmount>().Fill[1].RealScroll + FindObjectOfType<FillAmount>().Fill[0].RealScroll + inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth < 1)
                {
                    FindObjectOfType<FillAmount>().Fill[1].RealScroll += inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth;
                }
                else
                {
                    if (FindObjectOfType<FillAmount>().Fill[1].RealScroll + inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth > 1) FindObjectOfType<FillAmount>().Fill[1].RealScroll = 1;
                    else FindObjectOfType<FillAmount>().Fill[1].RealScroll += inventory.pickUps[slotSelected].GetComponent<UIvariables>().foodWorth;
                    FindObjectOfType<FillAmount>().Fill[0].RealScroll = 1 - FindObjectOfType<FillAmount>().Fill[1].RealScroll;
                }
                FindObjectOfType<FillAmount>().CalculateSize();
            }
            timerHolding = 0f;
            Destroy(inventory.objRef[slotSelected]);
            Destroy(inventory.pickUps[slotSelected]);
            inventory.isFull[slotSelected] = false;
            inventory.pickUps[slotSelected] = null;
            timeBtwAttack = 0;
            GameObject.Find("HandLeft").transform.GetComponent<Animator>().SetBool("eating", false);
            GameObject.Find("HandRight").transform.GetComponent<Animator>().SetBool("eating", false);
        }
    }
    public void StopAnim(GameObject obj, GameObject obj2, string anim)
    {
        oldHand = inventory.selectedHand;
        obj.GetComponent<Animator>().SetBool(anim, false);
        obj2.GetComponent<Pickup>().canAttack = false;
        obj2.layer = LayerMask.NameToLayer("itemOnGround");
        if (obj2 != inventory.objRef[slotSelected])
            timeBtwAttack = 0.1f;
    }
    //waitIEnumerator
    public IEnumerator Wait(string reason, GameObject obj, float Time)
    {
        yield return new WaitForSeconds(Time);
        if (reason == "canBeTaken")
        {
            obj.GetComponent<Pickup>().canBeTaken = true;
        }
    }
    //eat
    //attack
    //fire/shoot
}
