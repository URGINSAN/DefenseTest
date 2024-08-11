using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPusher : MonoBehaviour
{
    public float DelayMin = 1;
    public float DelayMax = 3;
    public GameObject[] Objects;
    public Transform[] Poses;

    private void Start()
    {
        StartCoroutine(IE());
    }
    
    IEnumerator IE()
    {
        Instantiate(Objects[Random.Range(0, Objects.Length)], Poses[Random.Range(0, Poses.Length)].position, Quaternion.identity);
        yield return new WaitForSeconds(Random.Range(DelayMin, DelayMax));
        StartCoroutine(IE());
    }
}