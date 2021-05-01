using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    [Header("Animator:")]
    public Animator anim;
    public AudioSource aud;

    private void OnTriggerEnter(Collider player)
    {
        if(player.GetComponent<PlayerController>() != null)
        {
            anim.enabled = true;
            aud.enabled = true;
        }
    }
}
