using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableFood : MonoBehaviour
{
    public int FoodWorth;
    private void Start()
    {
        FoodWorth = (int) (transform.localScale.x + transform.localScale.y);
        if (FoodWorth == 0) FoodWorth++;
    }
}
