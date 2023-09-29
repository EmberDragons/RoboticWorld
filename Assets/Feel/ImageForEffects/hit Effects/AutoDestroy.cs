using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float time = 0.25f;
    private void Start()
    {
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
