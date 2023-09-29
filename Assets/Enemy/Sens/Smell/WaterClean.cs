using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterClean : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer == LayerMask.NameToLayer("alive")) collision.gameObject.GetComponent<LeftSmells>().CalculatedTime = collision.gameObject.GetComponent<LeftSmells>().TimeBetween;
    }
}
