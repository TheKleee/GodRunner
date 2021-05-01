using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tapable))]
public class PotSize : MonoBehaviour
{
    [HideInInspector] public Tapable tap;

    public void RestorePotSize()
    {
        LeanTween.scale(gameObject, new Vector3(1, 1, 1),  tap.duration);
    }


}
