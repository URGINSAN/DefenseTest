using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModule : MonoBehaviour
{
    public float Health = 100;
    public bool CanDeath = true;
    public enum Type
    {
        Player = 0,
        Crate = 1,
        Barrel = 2
    }
    public Type type;

    private void Update()
    {
        if (CanDeath)
        {
            if (Health < 0)
            {
                if (type == Type.Crate)
                {
                    GetComponent<Barrel>().Break();
                }
                if (type == Type.Barrel)
                {
                    GetComponent<Barrel>().Break();
                }

                CanDeath = false;
            }
        }
    }

    public void Damage(float dam)
    {
        Health += dam;
    }
}
