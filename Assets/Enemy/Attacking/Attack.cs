using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform attackPos;
    private float waitTime = 0f;
    public float Range;
    public LayerMask _alive;
    public Brain brain;
    public Animator anim;
    ParticleSystem attParticle;
    bool Collision = false;
    bool attack = true;
    float normSpeed = 0;
    bool timeStamp = true;
    bool grabbed = false;
    bool TimeHasPassed = false;
    bool TimeToAttack = false;
    float BackLives;
    bool canGrab = true;
    void Start()
    {
        brain = GetComponent<Brain>();
        Range = brain.Size*1.2f;
        _alive = GetComponent<Sens>()._alive;
        waitTime = (brain.attackSpeed / 2f) + .5f;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (brain.fighting && attack && brain._Live > 0f)
        {
            attackTrigger();
        }
    }
    void attackTrigger()
    {
        if(brain.Attack == "bite")
        {
            Collider2D[] _enemies = Physics2D.OverlapCircleAll(attackPos.position, Range, _alive);
            if (_enemies.Length > 0 && _enemies[0].GetComponent<Brain>() != null && _enemies[0].GetComponent<Brain>().Name != brain.Name && anim.GetBool("att") == false)
            {
                anim.SetBool("att", true);
                //attack
                /////anim
                /////event on anims
                /////if true then damage enemy
            }
        }
        if (brain.Attack == "charge")
        {
            Collider2D[] _enemies = Physics2D.OverlapCircleAll(attackPos.position, Range * 2.5f, _alive);
            if (_enemies.Length > 0 && _enemies[0].GetComponent<Brain>() != null && _enemies[0].GetComponent<Brain>().Name != brain.Name && anim.GetBool("att") == false)
            {
                anim.SetBool("att", true); 
                if(normSpeed == 0 && timeStamp == true)
                {
                    timeStamp = false;
                    normSpeed = brain.Speed;
                    brain.Speed = brain.Speed * 2.5f;
                }
                //attack
                /////anim
                /////event on anims
                /////if true then damage enemy
            }
        }
        if (brain.Attack == "mouthGrab")
        {
            Collider2D[] _enemies = Physics2D.OverlapCircleAll(attackPos.position, Range, _alive);
            if (_enemies.Length > 0 && _enemies[0].GetComponent<Brain>() != null && _enemies[0].GetComponent<Brain>().Name != brain.Name && anim.GetBool("att") == false)
            {
                anim.SetBool("att", true);
                //attack
                /////anim
                /////event on anims
                /////if true then damage enemy
            }
        }
        if (brain.Attack == "grab")
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++){
                if(transform.GetChild(0).GetChild(i).tag == "Graber")
                {
                    transform.GetChild(0).GetChild(i).GetComponent<multipleAttacking>().Target = brain.Target;
                }
            }
        }
    }
    IEnumerator wait()
    {
        Collider2D[] _enemies = Physics2D.OverlapCircleAll(attackPos.position, Range, _alive);
        if (_enemies.Length > 0 && _enemies[0].GetComponent<Brain>().Name != brain.Name)
        {
            //bite attack
            if (brain.Attack == "bite")
            {
                //buff attack
                float buff;
                //sneakAttack buff
                if (brain.sneaking)
                {
                    buff = Random.Range(1f, 2f);
                }
                else buff = Random.Range(0f, 1f);
                //damage
                if (_enemies[0].transform.GetChild(1).GetComponent<ParticleSystem>() != null)
                {
                    _enemies[0].transform.GetChild(1).transform.rotation = Quaternion.LookRotation(_enemies[0].transform.position - transform.position, Vector2.up);
                    _enemies[0].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                    if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 0.85f);
                    else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 1f);
                    else FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 2f);
                }
                else
                {
                    _enemies[0].transform.GetChild(3).transform.rotation = Quaternion.LookRotation(_enemies[0].transform.position - transform.position, Vector2.up);
                    _enemies[0].transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                    if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 0.85f);
                    else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 1f);
                    else FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 2f);
                }

                _enemies[0].GetComponent<Brain>()._Live -= (brain.attackDamage + buff);
                if (_enemies[0].GetComponent<PlayerBrainReader>() != null)
                {
                    _enemies[0].GetComponent<PlayerBrainReader>().Hit++;
                }
            
            }
            

        }
        Collider2D[] _enemies2 = Physics2D.OverlapCircleAll(attackPos.position, Range, _alive);
        if (_enemies2.Length > 0 && _enemies2[0].GetComponent<Brain>().Name != brain.Name)
        {
            //charge attack
            if (brain.Attack == "charge")
            {

                //buff attack
                float buff;
                float force;
                //sneakAttack buff
                if (brain.sneaking)
                {
                    buff = Random.Range(1f, 2f);
                }
                else buff = Random.Range(0f, 1f);
                force = Random.Range(0f, 2f);
                //damage
                if (_enemies2[0].transform.GetChild(1).GetComponent<ParticleSystem>() != null)
                {
                    _enemies2[0].transform.GetChild(1).transform.rotation = Quaternion.LookRotation(_enemies2[0].transform.position - transform.position, Vector2.up);
                    _enemies2[0].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                    if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 0.85f);
                    else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 1f);
                    else FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 2f);
                }
                else
                {
                    _enemies2[0].transform.GetChild(3).transform.rotation = Quaternion.LookRotation(_enemies2[0].transform.position - transform.position, Vector2.up);
                    _enemies2[0].transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                    if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 0.85f);
                    else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 1f);
                    else FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 2f);
                }

                _enemies2[0].GetComponent<Brain>()._Live -= (brain.attackDamage + buff);
                if (_enemies2[0].GetComponent<Rigidbody2D>() != null && _enemies2[0].GetComponent<PlayerController>() == null)
                {
                    _enemies2[0].GetComponent<Rigidbody2D>().AddForce((transform.position - _enemies2[0].transform.position) * (brain.attackDamage + force) * 5f);
                    _enemies2[0].GetComponent<Rigidbody2D>().AddForce(transform.up * (brain.attackDamage + force) * 3f);
                }
                else if (_enemies2[0].GetComponent<PlayerController>() != null)
                {
                    if (GetComponent<Rigidbody2D>().velocity.x > 0)
                        _enemies2[0].GetComponent<PlayerController>()._currentHorizontalSpeed = (brain.attackDamage + force) * 12f;
                    else
                        _enemies2[0].GetComponent<PlayerController>()._currentHorizontalSpeed = -(brain.attackDamage + force) * 12f;
                    _enemies2[0].GetComponent<PlayerController>()._currentVerticalSpeed = (brain.attackDamage + force) * 6f;
                }
                if (_enemies2[0].GetComponent<PlayerBrainReader>() != null)
                {
                    _enemies2[0].GetComponent<PlayerBrainReader>().Hit++;
                }

            }
        }
        Collider2D[] _enemies3 = Physics2D.OverlapCircleAll(attackPos.position, Range, _alive);
        if (_enemies3.Length > 0 && _enemies3[0].GetComponent<Brain>().Name != brain.Name)
        {
             //charge attack
             if (brain.Attack == "mouthGrab" && canGrab)
             {
                 //buff attack
                 float buff;
                 float force;
                 //sneakAttack buff
                 if (brain.sneaking)
                 {
                     buff = Random.Range(0f, 1f);
                 }
                 else buff = Random.Range(0f, 0.5f);
                 force = Random.Range(0f, 1f);
                 //damage
                 if (_enemies3[0].transform.GetChild(1).GetComponent<ParticleSystem>() != null)
                 {
                     _enemies3[0].transform.GetChild(1).transform.rotation = Quaternion.LookRotation(_enemies3[0].transform.position - transform.position, Vector2.up);
                     _enemies3[0].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                     if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 0.85f);
                     else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 1f);
                     else FindObjectOfType<AudioManager>().PlayInfos("EnemyHitBite", 2f);
                 }
                 else
                 {
                     _enemies3[0].transform.GetChild(3).transform.rotation = Quaternion.LookRotation(_enemies3[0].transform.position - transform.position, Vector2.up);
                     _enemies3[0].transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                     if (brain.Size > 1.5f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 0.85f);
                     else if (brain.Size > 1f) FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 1f);
                     else FindObjectOfType<AudioManager>().PlayInfos("PlayerHitBite", 2f);
                 }

                 _enemies3[0].GetComponent<Brain>()._Live -= (brain.attackDamage + buff);
                 EnemyPos(_enemies3[0].transform);
                 grabbed = true;
                 if (_enemies3[0].GetComponent<PlayerBrainReader>() != null)
                 {
                     _enemies3[0].GetComponent<PlayerBrainReader>().Hit++;
                 }

             }
        }
        attack = false;
        yield return new WaitForSeconds(waitTime);
        attack = true;
        
    }
    public void EnemyPos(Transform enemy)
    {
        BackLives = brain._Live;
        while (grabbed == true)
        {
            if (TimeHasPassed)
            {
                StartCoroutine(ResetPos(enemy));
            }
            if (TimeToAttack)
            {
                StartCoroutine(Crush(enemy));
            }
            if(brain._Live <= BackLives / 1.5f)
            {
                grabbed = false;
                canGrab = false;
                StartCoroutine(CanGrab());
            }
        }
    }
    // animation event
    public void setCollision()
    {
        if (Collision == false) {
            Collision = true;
            StartCoroutine(wait());
        }
        else
            Collision = false;
    }
    public void setAnimation()
    {
        anim.SetBool("att", false);
        if (normSpeed != 0)
        {
            brain.Speed = normSpeed;
            normSpeed = 0;
            StartCoroutine(TimeStamp());
        }
    }
    IEnumerator CanGrab()
    {
        yield return new WaitForSeconds(2.5f);
        canGrab = true;
    }
    IEnumerator ResetPos(Transform enemy)
    {
        enemy.transform.position = attackPos.position;
        TimeHasPassed = false;
        yield return new WaitForSeconds(0.05f);
        TimeHasPassed = true;
    }
    IEnumerator Crush(Transform enemy)
    {
        enemy.GetComponent<Brain>()._Live -= brain.attackDamage/2f;
        TimeToAttack = false;
        yield return new WaitForSeconds(brain.attackSpeed*1.5f);
        TimeToAttack = true;
    }
    IEnumerator TimeStamp()
    {
        yield return new WaitForSeconds(3f);
        timeStamp = true;
    }
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(attackPos.position, Vector3.forward, Range);
    }
}
