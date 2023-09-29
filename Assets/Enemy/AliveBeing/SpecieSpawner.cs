using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpecieSpawner : MonoBehaviour
{
    public SpecieIdentity Index;
    public Sens Sens { get; private set; }
    public Brain Brain{ get; private set; }
//
    [Header("Attributes")]
    public float TimeBetweenCreatureSpawn;
    public float NbCreature;
    GameObject _prefab;
//

    public string specieName { get; private set; }
    public string TypeOfEater { get; private set; }
    public int FoodRequired { get; private set; } 

    public string _attack { get; private set; }
    public float _Agressivity { get; private set; }

    public int _Live { get; private set; }

    public int _Strength { get; private set; }

    public float Size { get; private set; } = 1f;
    public float Speed { get; private set; } = 1f;
    public float attackSpeed { get; private set; } = 1f;
    public float attackDamage { get; private set; } = 1f;
    public float jumpLength { get; private set; } = 1f;
    // Start is called before the first frame update
    void Start()
    {
        specieName = Index.specieName;
        TypeOfEater = Index.TypeOfEater;
        FoodRequired = Index.FoodRequired;
        _prefab = Index._prefab;
        StartCoroutine(CreateEnemy());
    }
    public void Createenemy()
    {
        NbCreature++;
        StartCoroutine(CreateEnemy());
    }
    IEnumerator CreateEnemy()
    {
        while (NbCreature > 0f)
        {
            _Agressivity = Random.Range(Index._minAgressivity, Index._maxAgressivity + 1);
            _Live = Random.Range(Index._minLive, Index._maxLive + 1);
            _Strength = Random.Range(Index._minStrength, Index._maxStrength + 1);
            Size = (2f+(_Strength/2f)) / Random.Range(2f, 3f);
            Speed = Random.Range(4f, 8f) / (_Strength+2f);
            attackSpeed = (_Strength * 2f) / Random.Range(Speed, (Speed + 4));
            attackDamage = _Strength / Random.Range(2f, 2.7f);
            jumpLength = (3 + (Speed * 2f)) / ((Size / 3) + 0.1f);
            _attack = Index._attack;
            if (Index.usesTentacles)Brain = _prefab.GetComponentInChildren<Brain>(); 
            else Brain = _prefab.GetComponent<Brain>();
            if(Index.usesTentacles) Sens = _prefab.GetComponentInChildren<Sens>();
            else Sens = _prefab.GetComponent<Sens>();
            yield return new WaitForSeconds(TimeBetweenCreatureSpawn);

            Sens.viewAngle = Random.Range(Index.viewAngle - 4, Index.viewAngle + 4);
            Sens.viewRange = Random.Range(Index.viewRange - 2f, Index.viewRange + 2f);

            Sens.hearRange = Random.Range(Index.hearRange - 2f, Index.hearRange + 2f);

            Sens.smellRange = Random.Range(Index.smellRange - 2f, Index.smellRange + 2f);

            Sens.seekRange = Random.Range(Index.seekRange - 0.1f, Index.seekRange + 0.2f);

            Sens._food = Index._food;
            Sens._alive = Index._alive;
            Sens._smells = Index._smells;
            Sens._environnement = Index._environnement;
            Brain.home = this.transform;
            Brain.Name = specieName;
            Brain.FoodWorth = Index.FoodWorth;
            Brain.Speed = Speed;
            Brain._Live = _Live;
            Brain.Attack = _attack;
            Brain.TypeOfEater = TypeOfEater;
            Brain.FoodRequired = FoodRequired;
            Brain.attackSpeed = attackSpeed;
            Brain.attackDamage = attackDamage;
            Brain._Agressivity = _Agressivity;
            Brain.Size = Size;
            Brain.jumpLength = (jumpLength*5f) + 30f;
            Brain.canFly = Index.canFly;
            //special
            Brain.specieSpawner = this;

            _prefab.transform.localScale = new Vector2(_prefab.transform.localScale.x * Size, _prefab.transform.localScale.y * Size);
            Instantiate(_prefab, transform.position, Quaternion.identity);
            _prefab.transform.localScale = new Vector2(1f, 1f);

            NbCreature--;
        }
    }
}
