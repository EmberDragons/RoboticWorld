using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ResortSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer)
            spriteRenderer.sortingOrder = (int)(-10 * transform.position.z);

        else
            spriteRenderer.sortingOrder = (int)(-10 * transform.position.z);
    }
}
