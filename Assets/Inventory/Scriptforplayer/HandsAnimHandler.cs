using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsAnimHandler : MonoBehaviour
{
    public GameObject pickUp;
    public GameObject Inventory;
    public void EndAnimationSword()
    {
        Inventory.GetComponent<InventoryNavigation>().StopAnim(transform.gameObject, pickUp, "swordAttack");
    }
    public void EndAnimationSpear()
    {
        Inventory.GetComponent<InventoryNavigation>().StopAnim(transform.gameObject, pickUp, "spearAttack");
    }
    public void EatSound()
    {
        FindObjectOfType<AudioManager>().Play("Eating");
    }
}
