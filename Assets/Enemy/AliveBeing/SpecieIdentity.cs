using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecieIdentity : MonoBehaviour
{

    [Header("Attributes")]
    public GameObject _prefab;
    public string specieName;
    public string TypeOfEater = "carnivor";
    public int FoodRequired = 3;
    public int FoodWorth = 2;
    public bool canFly;
    public bool usesTentacles;

    [Header("Agressivity")]
    public string _attack;
    public float _minAgressivity = 1;
    public float _maxAgressivity = 5;

    [Header("Live")]
    public int _minLive = 3; 
    public int _maxLive = 6;

    [Header("Strength")]
    public int _minStrength = 3;
    public int _maxStrength = 4;


    [Header("Sens")]
    [Range(1, 360)] public int viewAngle;
    public float viewRange;

    public float hearRange;

    public float smellRange;

    public float seekRange;

    public LayerMask _alive;
    public LayerMask _food;
    public LayerMask _smells;
    public LayerMask _environnement;
}
