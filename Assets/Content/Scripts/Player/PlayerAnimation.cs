using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator Anim;
    private string currentAnim;

    public void PlayAnim(string anim)
    {
        if (currentAnim == anim)
            return;

        Anim.CrossFade(anim, 0.05f);

        currentAnim = anim;
    }
}
