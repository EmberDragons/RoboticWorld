using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftSmells : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Attributes")]
    public Brain Brain;
    public GameObject Smell;
    publicsmellInfo _Smell;
    public float TimeBetween = 15f;

    public float CalculatedTime;

    void Start()
    {
        Brain = this.GetComponent<Brain>();
        CalculatedTime = TimeBetween;
        StartCoroutine(CalculateTimeBetweenSmell());
        StartCoroutine(SpawnSmell());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CalculateTimeBetweenSmell()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (CalculatedTime >= 1f) CalculatedTime = CalculatedTime - (TimeBetween / 600f);
        }
    }
    IEnumerator SpawnSmell()
    {
        while (true)
        {
            yield return new WaitForSeconds(CalculatedTime);
            _Smell = Smell.GetComponent<publicsmellInfo>();
            _Smell._Live = Brain._Live; 
            _Smell.Name = Brain.Name;
            _Smell.attackDamage = Brain.attackDamage;
            _Smell.Size = Brain.Size;
            if(GetComponent<Brain>()._Live > 0f)
                Instantiate(Smell, transform.position, Quaternion.identity);
        }
    }
    
}
