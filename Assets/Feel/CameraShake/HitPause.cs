using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public void PauseHit(float time)
    {
        Time.timeScale = 0.001f;
        StartCoroutine(Wait(time));
    }
    private IEnumerator Wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
    }
}
