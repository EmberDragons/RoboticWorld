using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multipleAttacking : MonoBehaviour
{
    public Transform StartPoint;
    public Transform Target;

    private Attack attack;
    public void Start()
    {
        attack = transform.parent.parent.GetComponent<Attack>(); 
        StartPoint = transform;
    }
    public void Update()
    {
        if(Target != null && Target.gameObject.layer == LayerMask.NameToLayer("alive"))
        {
            Collider2D[] _enemies = Physics2D.OverlapCircleAll(attack.attackPos.position, attack.Range, attack._alive);
            if (_enemies.Length > 0 && _enemies[0].GetComponent<Brain>() != null && _enemies[0].GetComponent<Brain>().Name != attack.brain.Name && attack.anim.GetBool("att") == false)
            {
                attack.anim.SetBool("att", true);
                //attack
                /////anim
                /////event on anims
                /////if true then damage enemy
            }
        }
    }
}
