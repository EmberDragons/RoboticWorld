using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FillAmount : MonoBehaviour
{
    public FillClass[] Fill;
    public void Update()
    {
        FindObjectOfType<PlayerBrainReader>().speed = Fill[0].RealScroll;
        FindObjectOfType<PlayerBrainReader>().strength = Fill[1].RealScroll;
    }
    public void CalculateSize()
    {
        FillClass FirstInOrder = Fill[0];
        FillClass SecondInOrder = Fill[1];

        FirstInOrder.Scroll = FirstInOrder.RealScroll;
        SecondInOrder.Scroll = FirstInOrder.Scroll + SecondInOrder.RealScroll;
        StartCoroutine(Wait(FirstInOrder, SecondInOrder));
        //set the calculated values to the sprite
    }
    IEnumerator Wait(FillClass FirstInOrder, FillClass SecondInOrder)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        Fill[0].image.fillAmount = Mathf.Lerp(Fill[0].image.fillAmount, FirstInOrder.Scroll, Time.deltaTime * 5f);
        Fill[1].image.fillAmount = Mathf.Lerp(Fill[1].image.fillAmount, SecondInOrder.Scroll, Time.deltaTime * 5f);
        if (Fill[0].image.fillAmount != FirstInOrder.Scroll || Fill[1].image.fillAmount != SecondInOrder.Scroll)
        {
            StartCoroutine(Wait(FirstInOrder, SecondInOrder));
        }
    }
}
