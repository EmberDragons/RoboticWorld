using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class publicsmellInfo : MonoBehaviour
{
    // Start is called before the first frame update      
    //attributes

    [Header("Attributes")]
    //To sens
    public float _Live;
    public string Name;
    public float Size = 1f;
    public float attackDamage = 1f;
    public bool smelledBy = false;

    // Update is called once per frame
    void Update()
    {
        if (smelledBy)
        {
            Destroy(this.gameObject, 5f);
        }
        else Destroy(this.gameObject, 60f);
    }
    // Update is called once per frame
}
