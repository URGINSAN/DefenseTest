using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject Breakable;
    public GameObject Explosion;
    public enum Type
    {
        Crate = 0,
        Barrel = 1
    }
    public Type type;

    public void Break()
    {
        if (type == Type.Crate)
        {
            GameObject go = Instantiate(Breakable, transform.position, transform.rotation);
            go.transform.localScale = transform.localScale;
            go.GetComponent<ExplodeBarrel>().Explode();

            Destroy(gameObject);
        }
        if (type == Type.Barrel)
        {
            GameObject go = Instantiate(Breakable, transform.position, transform.rotation);
            go.transform.localScale = transform.localScale;
         

            Instantiate(Explosion, transform.position, Quaternion.identity);
            //GetComponent<DestroyByTime>().enabled = true;
            go.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);
            go.GetComponent<Rigidbody>().AddForce(Vector3.left * 50, ForceMode.Impulse);

            Destroy(gameObject);
        }
    }
}
