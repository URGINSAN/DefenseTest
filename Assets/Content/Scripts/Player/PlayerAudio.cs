using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource SFX;
    public AudioClip FireSnd;
    public AudioClip[] MoveSnd;

    public void PlaySnd(string type)
    {
        switch (type)
        {
            case "Shoot":
                SFX.volume = 0.5f;
                SFX.PlayOneShot(FireSnd);
                break;
            case "Move":
                SFX.volume = 0.3f;
                SFX.PlayOneShot(MoveSnd[Random.Range(0, MoveSnd.Length)]);
                break;
        }
    }
}
