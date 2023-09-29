using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Brain : MonoBehaviour
{
    // Start is called before the first frame update
    //pathfinding
    public Transform Target;
    public ParticleSystem particleEffect;
    public string Name;
    public string TypeOfEater;
    public int FoodRequired;
    public int FoodWorth;
    public bool noisy;
    public bool canFly;
    
    public float _Agressivity;

    public float _Live;
    float _originalLive;

    public float Size = 1f;
    public float Speed = 1f;
    public float BasedSpeed = 1f;
    public float maxSpeed = 3f;
    public float normalMaxSpeed;
    public float sneakingMaxSpeed;
    public float attackSpeed = 1f;
    public float attackDamage = 1f;
    //Jump
    public float jumpLength;

    //booleans
    public bool underAttack; //+++
    public bool alive = true; //++++
    public bool eating; //++
    public bool runing;
    public bool sneaking = false;
    public bool fighting; //+
    //
    public string Attack;
    //

    int mostImportant;
    public float Dist;

    public Transform home;
    Sens Sens;

    //enemy
    public Transform currentEnemy;
    public Transform FollowingSmell;

    //destination
    public Transform destination;
    private Rigidbody2D rb;
    private bool TimeHomeIsElapsed = false;
    bool once_UnderAttack = false;
    bool checkDeath = false;
    float OldLive;

    //random things
    public SpecieSpawner specieSpawner;
    public bool gotAttacked = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Sens = this.GetComponent<Sens>();
        
        BasedSpeed = Speed;
        maxSpeed = (Speed * 3f) + 2f;
        normalMaxSpeed = maxSpeed;
        sneakingMaxSpeed = maxSpeed / 2f;
        OldLive = _Live;
        _originalLive = _Live;
        mostImportant = 0;
        destination = new GameObject($"Destination : {Name}").transform;
        if (canFly && rb != null)
        {
            maxSpeed /= 1.5f;
            Speed /= 1.2f;
            rb.gravityScale = 0f;
        }
        else if(rb != null) rb.gravityScale = 2f;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canFly && _Live>0f)
        {
            if(Physics2D.Raycast(transform.position, -transform.up, 2f, Sens._environnement)) 
            {
                float length = Physics2D.Raycast(transform.position, -transform.up, 2f, Sens._environnement).distance;
                rb.AddForce(new Vector2(0f, 20f*1/length));
            }
        }
        if (OldLive != _Live)
        {
            StartCoroutine(GotAttacked());
        }
        if ((rb != null) && Sens._smellTarget != null && runing == false || sneaking && runing == false) noisy = false;
        else noisy = true;
        if (_originalLive != _Live)
        {
            float lives = normalMaxSpeed*(_Live/_originalLive)+0.75f;
            normalMaxSpeed = lives;
            maxSpeed = normalMaxSpeed;
            _originalLive = _Live;
        }
        if (currentEnemy != null && sneaking && currentEnemy.GetComponent<Brain>()!= null)
        {
            if (currentEnemy.GetComponent<Brain>()._Live < 0 || runing || currentEnemy.GetComponent<Brain>().currentEnemy == transform || currentEnemy.GetComponent<Brain>().underAttack || currentEnemy.GetComponent<Rigidbody2D>() == null)
            {
                sneaking = false;
                maxSpeed = normalMaxSpeed;
            }
            else
            {
                maxSpeed = sneakingMaxSpeed;
            }
        }
        if (underAttack && (rb != null))
        {
            Collider2D[] friend = Physics2D.OverlapCircleAll(transform.position, 15f, Sens._alive);
            
            for (int i = 0; i < friend.Length; i++)
            {
                if (friend[i].GetComponent<Brain>() != null)
                {
                    if (friend[i].GetComponent<Brain>().Name == Name)
                    {
                        friend[i].GetComponent<Brain>().underAttack = true;
                        friend[i].GetComponent<Brain>().currentEnemy = currentEnemy;
                    }
                }
            }
        }
        if (home != null && Target == home)
        {
            if (transform.position.x < (home.position.x + 0.5f) && transform.position.x > (home.position.x - 0.5f))
            {
                if (transform.position.y < (home.position.y + 2f) && transform.position.y > (home.position.y - 2f))
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (i > 1) Instantiate(transform.GetChild(i).GetComponent<Pickup>().itemButton.GetComponent<UIvariables>().Object, transform.position, transform.rotation, null);
                    }
                    specieSpawner.Createenemy();
                    Destroy(this.gameObject);
                }
            }
        }
        if (eating == true && (rb != null)) rb.velocity = new Vector2(0f, rb.velocity.y);
        if ((rb != null) && rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = (Vector2)Vector3.ClampMagnitude((Vector3)rb.velocity, (maxSpeed +Speed));
        }
        if (currentEnemy != null)
        {
            if (currentEnemy.GetComponent<Brain>() != null && currentEnemy.GetComponent<Brain>()._Live <= 0f)
            {
                fighting = false;
                currentEnemy = null;
                underAttack = false;
            }
        }
        if (_Live <= 0f && checkDeath == false && (rb != null)) // ++++
        {
            Dead();
            checkDeath = true;
        }
        if (_Live > 0f)
        {
            if ((rb != null) && Target == home && TimeHomeIsElapsed) {
                Target = null;
                TimeHomeIsElapsed = false;
            }
            mostImportant = 0;
            //underAttack part ( bool is set by attacker ) +++
            if (underAttack && once_UnderAttack == false)
            {
                once_UnderAttack = true;
                if(rb != null)UnderAttack();
                StartCoroutine(underAttackTime());
                mostImportant = 4;
            }

            //Checking endengered ++
            if ((rb != null) && _Live <= (_originalLive / _originalLive) && currentEnemy != null && fighting || (rb != null) && mostImportant <= 3 && GameObject.Find("WorldInfos").GetComponent<Infos>().nightTime)
            {
                Target = home;
                //get in home and launch enemy creation
                //cf l.146
                mostImportant = 3;
            }

            //Eating +
            if ((rb != null) && Sens._feedTarget != null && underAttack == false && mostImportant <= 2)
            {
                //carnivor
                if(TypeOfEater == "carnivor")
                {
                    if (Sens._feedTarget.gameObject.GetComponent<Brain>().FoodWorth > 0)
                    {
                        Target = Sens._feedTarget;
                        mostImportant = 2;
                    }
                    if (Sens._seekTarget != null)
                    {
                        if (Sens._seekTarget.gameObject.GetComponent<Brain>().FoodWorth > 0)
                        {
                            Eat();
                            mostImportant = 2;
                        }
                    }
                }
                //herbivor
                if (TypeOfEater == "herbivor")
                {
                    if (Sens._feedTarget.gameObject.GetComponent<VegetableFood>().FoodWorth > 0)
                    {
                        Target = Sens._feedTarget;
                        mostImportant = 2;
                    }
                    if (Sens._seekTarget != null)
                    { 
                        if (Sens._seekTarget.gameObject.GetComponent<VegetableFood>().FoodWorth > 0)
                        {
                            Eat();
                            mostImportant = 2;
                        }
                    }
                }
            }
            //Attack part -
            if ((rb != null) && underAttack == false && mostImportant <= 0)
            {
                ChoseToAttack();
                mostImportant = 0;
            }

            //Roaming --
            //In attack part
        }
    }
    //Categories

    
    void ChoseToAttack()
    { 
            if (Sens._viewTarget != null && underAttack == false)
            {
                currentEnemy = Sens._viewTarget;
                if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) && TypeOfEater == "carnivor" || currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) && _Agressivity > 1.5f) Fight();
                if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) == false && TypeOfEater == "carnivor") Run();
            }
            else //can't see enemy
            {
                if (Sens._hearTarget != null && underAttack == false)
                {
                    currentEnemy = Sens._hearTarget;
                    if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) && TypeOfEater == "carnivor" || currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) && _Agressivity > 1.5f) Fight();
                    if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) == false && TypeOfEater == "carnivor") Run();
                }
                else //can't hear enemy
                {
                    //---
                    if (Sens._smellTarget != null && underAttack == false)
                    {
                        FollowingSmell = Sens._smellTarget;
                        currentEnemy = null;
                        if (EvaluateSmellStrength(FollowingSmell) && TypeOfEater == "carnivor")
                        {
                            Target = FollowingSmell;
                        }
                        if (EvaluateSmellStrength(FollowingSmell) == false)
                        {
                            Roam();
                            currentEnemy = null;
                            FollowingSmell = null;
                        }
                    }
                    else if (underAttack == false) //can't sens enemy at all
                    {
                        //nothing
                        Roam();
                        currentEnemy = null;
                        FollowingSmell = null;
                    }

                }
            }
    }
    void UnderAttack()
    {
        if (Sens._viewTarget != null)
        {
            currentEnemy = Sens._viewTarget;
            if (currentEnemy.GetComponent<Brain>()!=null && EvaluateStrength(currentEnemy)) Fight();
            if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) == false && fighting == false) Run();
        }
        else //can't see enemy
        {
            if (Sens._hearTarget != null)
            {
                currentEnemy = Sens._hearTarget;
                if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy)) Fight();
                if (currentEnemy.GetComponent<Brain>() != null && EvaluateStrength(currentEnemy) == false && fighting == false) Run();
            }
            else //can't hear enemy
            {
                if (GetComponent<Rigidbody2D>() != null && gotAttacked)//got attacked
                {
                    Collider2D[] _closest = Physics2D.OverlapCircleAll(transform.position,Mathf.Infinity,Sens._alive);
                    if (_closest[0].transform.GetComponent<Brain>()._Live != 0) currentEnemy = _closest[0].transform;
                    else Target = null;
                }
                else
                    once_UnderAttack = false;
            }
        }
    }/*
    IEnumerator CheckAround()
    {
        float rotationY = transform.rotation.y;
        
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (i==1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, rotationY + 180f)), Time.deltaTime * 40f);
            }
            else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, rotationY)), Time.deltaTime * 40f);

        }
    }*/
    IEnumerator GotAttacked()
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //launch backwards + all effects on book
        gotAttacked = true;
        yield return new WaitForSeconds(1f);
        gotAttacked = false;
    }
    IEnumerator underAttackTime()
    {
        yield return new WaitForSeconds(7f);
        once_UnderAttack = false;
        if(currentEnemy == null)
            underAttack = false;
    }
    void Run()
    {
        runing = true;
        if ((rb != null) && currentEnemy != null)
        {
            //set destination to the opposite side of the enemy andset it as Target
            Vector2 dir = new Vector2(transform.position.x - currentEnemy.position.x, 0f);
            Debug.DrawRay(transform.position, dir, Color.red);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, Sens._environnement);
            if (ray.transform != null)
            {
                destination.position = ray.transform.position;
                if (transform.position.x >= (ray.transform.position.x - 1f) && underAttack || transform.position.x <= (ray.transform.position.x + 1f) && underAttack)
                {
                    Fight();
                }
                Target = destination;
            }
            else
            {
                Target = home;
            }
        }
        if (fighting || sneaking || currentEnemy == null) runing = false;
    }
    public void Roam()
    {
        //GoTo a random place?
        //set destination to a random place and then set it as Target
        if (Sens._placeTarget != null)
            destination.position = Sens._placeTarget.position;
        else
            destination.position = home.position;
        Target = destination;
    }
    void Dead()
    {
        this.gameObject.layer = LayerMask.NameToLayer("food");
        alive = false;
        Target = null;
        //play dead anim
        GetComponent<Animator>().SetBool("dead", true);
        StartCoroutine(waitTime(0.2f, "death"));
        if(currentEnemy != null && currentEnemy.GetComponent<Brain>() != null)
            currentEnemy.GetComponent<Brain>().underAttack = false;
    }
    void Eat()
    {
        eating = true;
        //play eating anim
        GetComponent<Attack>().anim.SetBool("eat", true);
        StartCoroutine(waitTime(3f, "eatFood"));
    }
    //tools
    
    void Fight()
    {
        //GoTo + attack
        fighting = true;
        if ((rb != null) && currentEnemy != null)
        {
            if(currentEnemy.GetComponent<Brain>().Name != "" && currentEnemy.GetComponent<Brain>().currentEnemy != transform && currentEnemy.GetComponent<Brain>().underAttack == false)
            {
                //sneak up attack
                sneaking = true;
            }
            else
                currentEnemy.GetComponent<Brain>().underAttack = true;
            if (fighting == true)
            {
                StartCoroutine(waitTime(0.1f, "fighting"));
            }
            waitTime(0.4f, "fight");
        }

    }
    IEnumerator waitTime(float wait, string reason)
    {
        if (reason == "death")
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
            rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 3f;
            Destroy(GetComponent<EnemyAI>());
            Destroy(GetComponent<Sens>());
            Destroy(GetComponent<Seeker>());
        }
        yield return new WaitForSeconds(wait);
        if (reason == "fighting")
        {
            Target = currentEnemy;
        }
        if (reason == "eatFood" && eating)
        {
            eating = false;
            if (TypeOfEater == "carnivor")
            {
                FoodRequired -= Sens._seekTarget.gameObject.GetComponent<Brain>().FoodWorth;
                FoodWorth++;
                Sens._seekTarget.GetComponent<Brain>().FoodWorth = 0;
            }
            if (TypeOfEater == "herbivor")
            {
                FoodRequired -= Sens._seekTarget.gameObject.GetComponent<VegetableFood>().FoodWorth;
                FoodWorth++;
                Sens._seekTarget.GetComponent<VegetableFood>().FoodWorth = 0;
            }
            GetComponent<Animator>().SetBool("eat", false);
        }
        if (reason == "fight" && underAttack == false)
        {
            fighting = false;
        }
        if (reason == "fly")
        {
            canFly = true;
            rb.gravityScale = 0f;
        }
    }
    //Function called from outside()
    public void Fall()
    {
        canFly = false;
        StartCoroutine(GoDown());
        StartCoroutine(waitTime(4f,"fly"));
    }
    IEnumerator GoDown()
    {
        rb.gravityScale = 5f;
        yield return new WaitForSeconds(0.01f);
        if (canFly == false) StartCoroutine(GoDown());
    }
    //boolean stuff
    bool EvaluateStrength(Transform currentEnemy)
    {
        float enStrength = currentEnemy.GetComponent<Brain>().Size + currentEnemy.GetComponent<Brain>().attackDamage + (currentEnemy.GetComponent<Brain>()._Live / 2f);
        return (enStrength / _Agressivity) < Size + attackDamage + (_Live / 2);
    }
    bool EvaluateSmellStrength(Transform currentEnemy)
    {
        float enStrength = currentEnemy.GetComponent<publicsmellInfo>().Size + currentEnemy.GetComponent<publicsmellInfo>().attackDamage + (currentEnemy.GetComponent<publicsmellInfo>()._Live / 2f);
        return (enStrength / _Agressivity) < Size + attackDamage + (_Live / 2);
    }
}
