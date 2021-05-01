using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Sfx List:")]
    public AudioClip[] sfx;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CallSfx(int id)
    {
        audioSource.clip = sfx[id];

        audioSource.Play();
    }
}
