using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrainReader : MonoBehaviour
{
    // Start is called before the first frame update
    public Brain brain;
    [Header("STATS")]
    public float health; // = Brain._Lives
    public float strength;
    public float speed;
    [Header("Second varibles")]
    [SerializeField] private float maxBoost = 3;//for the health/speed/strenght selet system
    public int Hit;
    private bool decreasingHits = false;
    private float currentDamage;
    private float currentSpeed;
    private float currentDamageWeapon;
    private float currentSpeedWeapon;
    [SerializeField] private bool canAttack;
    private float healthbefore;
    private bool gotAttacked = false;
    private float basedTime = 20f;
    private float basedLives;
    private bool healing = false;
    private void Start()
    {
        brain = GetComponent<Brain>();
        brain._Live = health;
        healthbefore = brain._Live;
        basedLives = health;
    }
    private void Update()
    {
        if (healthbefore > brain._Live)
        {
            gotAttacked = true;
            FindObjectOfType<bloodScreen>().Hit();
            healthbefore = brain._Live;
            StartCoroutine(HealthTimer());
        }
        if (gotAttacked == false && brain._Live < basedLives && healing == false)
        {
            StartCoroutine(Heal());
        }
        if (brain._Live > basedLives)
        {
            brain._Live = basedLives;
            healthbefore = brain._Live;
        }


        DamageandSpeedManager();
    }
    IEnumerator HealthTimer()
    {
        float timeToWait = basedTime / (FindObjectOfType<FillAmount>().Fill[1].Scroll+1f);//depends on the food
        yield return new WaitForSeconds(timeToWait);
        gotAttacked = false;
    }
    IEnumerator Heal()
    {
        healing = true;
        yield return new WaitForSeconds(2.5f);
        if (gotAttacked == false) brain._Live++;
        healthbefore = brain._Live;
        healing = false;
    }
    void DamageandSpeedManager()
    {
        //damage
        //set the weaponDamage
        if (GetComponent<Inventory>().isFull[GetComponent<InventoryNavigation>().slotSelected])
        {
            currentDamageWeapon = GetComponent<Inventory>().pickUps[GetComponent<InventoryNavigation>().slotSelected].GetComponent<UIvariables>().damage;
            currentSpeedWeapon = GetComponent<Inventory>().pickUps[GetComponent<InventoryNavigation>().slotSelected].GetComponent<UIvariables>().speed;
        }
        else
            currentDamageWeapon = 0f;
        if (currentDamageWeapon == 0f) canAttack = false;
        else canAttack = true;
        //set the OverallDamage and speed
        currentDamage = (currentDamageWeapon * (strength + 2f)) / 1.5f;
        currentSpeed = (currentSpeedWeapon * (speed + 2f))/1.5f;
        brain.attackDamage = currentDamage;
        brain.attackSpeed = currentSpeed;
        brain.attackDamage = currentDamage;
    }
}
