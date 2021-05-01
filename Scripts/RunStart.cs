using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class RunStart : MonoBehaviour
{
    #region Private stuff... 18+ : C

    private Collider col;

    #endregion

    [Header("--- Is 90 Degree Rotation: ---")]
    public bool angle;

    [Header("Player Controller:")]
    public PlayerController Player;

    [Header("Gates:")]
    public GameObject leftGate;
    public GameObject rightGate;

    [Header("Outlines:")]
    public GameObject[] outlines = new GameObject[2];


    private void Start()
    {
        col = GetComponent<Collider>();

        if (Player == null)
            Player = FindObjectOfType<PlayerController>();
    }

    public void OpenGate()
    {
        Timing.RunCoroutine(_PlaySound().CancelWith(gameObject));

        col.enabled = false;
        foreach (GameObject glow in outlines)
            glow.SetActive(false);

        Player.RunStart();

        if (!angle)
        {
            LeanTween.rotateY(leftGate, leftGate.transform.eulerAngles.y - 90, .25f);
            LeanTween.rotateY(rightGate, rightGate.transform.eulerAngles.y + 90, .25f);
        }
        else
        {
            LeanTween.rotateY(leftGate, leftGate.transform.eulerAngles.y - 90, .25f);
            LeanTween.rotateY(rightGate, rightGate.transform.eulerAngles.y + 90, .25f);
        }
    }

    IEnumerator<float> _PlaySound()
    {
        yield return Timing.WaitForSeconds(.2f);
        GetComponent<AudioSource>().Play();
    }

    private void OnTriggerEnter(Collider player)
    {
        if(player.GetComponent<PlayerController>() != null)
        {
            OpenGate();
        }
    }
}
