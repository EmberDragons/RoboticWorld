using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class FillClass
{
    public string name;
    public Image image;
    [Range(0f, 1f)]
    public float Scroll;
    public float RealScroll;
}