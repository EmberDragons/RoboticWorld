using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    public bool[] isFull;
    public GameObject[] slots;
    public GameObject[] pickUps;
    public GameObject[] objRef;
    public Transform selectedHand;
    private void Update()
    {
        if (GetComponent<PlayerController>()._currentHorizontalSpeed < 0f) selectedHand = GameObject.Find("HandLeft").transform;
        if (GetComponent<PlayerController>()._currentHorizontalSpeed > 0f) selectedHand = GameObject.Find("HandRight").transform;
    }
}
