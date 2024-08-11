using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float Delay = 2.5f;

    void Start()
    {
        Destroy(gameObject, Delay);
    }
}
