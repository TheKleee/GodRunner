using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Tapable : MonoBehaviour
{
    #region Private:

    [HideInInspector] public bool isTapped;
    [HideInInspector] public bool isTriggered;  //Set this to true from another scriot -.-
    [HideInInspector] public PlayerController Player;
    private PotSize pot;

    [Header("Is Destructable:")]
    [SerializeField] private bool isDestructible;    //Change this if you want it to be an enum or whatever!!! xD

    #endregion

    [Header("LeanTween Data:")]
    [Space]
    public AnimationCurve animCurve;    //Use this for easing... let gameplay designers play with it! : D
    [Range(0f, 1f)]public float duration = .5f;        //This is the duration of the tweening animation
    [Range(0, .5f)]public float delay;                 //The delay before another action is called!
    [Space]

    [Header("Health:")]
    public int HP = 1;      //Read NoHealth() function for more info! >:)
    public int destHealth = 1;  //Use this value to call the specific sprite on a destructible object... destHealth is max 2!!! >:|

    [Header("Health indicator:")]
    public Sprite[] currentHp = new Sprite[2];  //This sprite represents the current hp based on destHealth...



    [Header("--- Special Vfx: ---")]
    public float vfxDelay = .5f;
    public float vfxDestDelay = .8f;
    [Space]
    public GameObject[] specVfx;
    public GameObject destSpecVfx;

    [Header("Target Position:")]
    public Transform targetPos;

    [Header("Scroll:")]
    public GameObject Scroll;

    [Header("Vfx:")]
    public GameObject RedVfx;   //If isDestructible
    public GameObject GreenVfx; //Else xD


    private void Start()
    {
        if (targetPos == null)
            targetPos = transform.parent.transform;

        if (GetComponent<PotSize>() != null)
        {
            pot = GetComponent<PotSize>();
            pot.tap = this;
        }
    }

    public void PostTapAction()
    {
        if (HP > 0)
        {
            if (isTriggered)
            {
                HP--;
                Player.ShakeCamera();

                if (!isDestructible)
                {
                    if(pot != null)
                        pot.RestorePotSize();


                    if (HP > 0)
                    {
                        RedVfx.SetActive(true);
                        Scroll.SetActive(true);
                        GreenVfx.SetActive(false);

                        isDestructible = true;
                    }
                    else
                        Player.GetComponent<UiController>().SetPickupSprite();

                    GreenVfx.SetActive(false);
                    //Turn off the visual indicator!!! >:D
                    isTapped = true;

                    if (specVfx.Length > 0)
                        Timing.RunCoroutine(_SpecVfxCallback().CancelWith(gameObject));

                    Timing.RunCoroutine(_FixPosIssues().CancelWith(gameObject));
                    Tapped();
                }

                //Do what you must:
                else
                {
                    if (destHealth > 0)
                        destHealth--;

                    if(destHealth == 0)
                    {
                        Instantiate(destSpecVfx, transform.position, Quaternion.identity);

                        if (HP > 0)
                        {
                            RedVfx.SetActive(false);
                            Scroll.SetActive(false);
                            GreenVfx.SetActive(true);

                            isDestructible = false;
                        } else {
                            //Instantiate vfx!
                            Player.GetComponent<UiController>().SetPickupSprite();
                            Destroy(transform.parent.gameObject);
                        }
                    }
                }
            }

            //We might actually never need this:
            else
                AboutToDie();
        }
        else
            NoHealth();
    }

    private IEnumerator<float> _SpecVfxCallback()
    {
        yield return Timing.WaitForSeconds(vfxDelay);
        foreach (GameObject vf in specVfx)
            vf.SetActive(true);

        yield return Timing.WaitForSeconds(vfxDestDelay);
        foreach (GameObject vf in specVfx)
            Destroy(vf);

    }

    private void NoHealth()
    {
        //This object can no longer be clicked => it has no further interactions!!!
        if (isDestructible)
            Destroy(transform.parent.gameObject);   //Let's remove unimportant assets from the scene...
    }

    /// <summary>
    /// Call this from PlayerController.cs inside TestForce() function!!! >:|
    /// </summary>
    public void AboutToDie()
    {
        //Turn off the visual indicator!!! >:D
        RedVfx.SetActive(false);
        Scroll.SetActive(false);
        GreenVfx.SetActive(false);

        isTapped = true;
    }

    public void SetTrigger()
    {
        if (isDestructible)
        {
            RedVfx.SetActive(true);
            Scroll.SetActive(true);
        }
        else
            GreenVfx.SetActive(true);

        isTriggered = true;
        //Turn on the visual indicator!!! >:\
    }


    public void Tapped()
    {
        LeanTween.rotate(gameObject, transform.parent.transform.eulerAngles, duration);

        LeanTween.move(gameObject, targetPos.position, duration)
            .setDelay(delay)
            .setEaseInCubic()
            .setEase(animCurve);    //Play around with this if you find the tweening animation lacking >:\
    }

    IEnumerator<float> _FixPosIssues()
    {
        yield return Timing.WaitForSeconds(duration+.05f);
        transform.position = transform.parent.position;
    }

    private void OnCollisionEnter(Collision player)
    {
        if (HP > 0 && isDestructible)
            if(player.transform.GetComponent<PlayerController>() != null)
            {
                //Kill the player...
                if (Player == null)
                    Player = player.transform.GetComponent<PlayerController>();

                Player.Revive();
            }

        if (!isDestructible)
            if (player.transform.GetComponent<PlayerController>() != null)
            {
                //Kill the player...
                if (Player == null)
                    Player = player.transform.GetComponent<PlayerController>();

                Player.Revive();
            }
    }


    #region 18+:

    public void SetDuration()
    {
        Timing.RunCoroutine(_SetDur().CancelWith(gameObject));
    }

    private bool isDead;

    [Header("--- Life Span Duration: ---")]
    public float lifespan = 10;
    public float playerCanMove = 1.1f;

    private IEnumerator<float> _SetDur()
    {
        do
        {
            if (isDead)
                break;

            if (!Player.canMove)
                Timing.RunCoroutine(_CheckIfAlive().CancelWith(gameObject));

            lifespan -= .1f;
            yield return Timing.WaitForSeconds(.1f);
        } while (lifespan > 0);

        if(Player.canMove)
            AboutToDie();
    }

    IEnumerator<float> _CheckIfAlive()
    {
        yield return Timing.WaitForSeconds(playerCanMove);

        if(!Player.canMove)
            isDead = true;
    }
    #endregion
}
