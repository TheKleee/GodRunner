using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;


public enum TutorialHelp
{
    start,
    swipe,
    jump,
    slide,
    regular,
}

public class Tutorial : MonoBehaviour
{
    #region Private like Pirate :O

    [HideInInspector] public bool hasDone;
    private PlayerBehaviour playerBehaviour;

    #endregion


    [Header("Selected Tutorial:")]
    public TutorialHelp help;

    [Header("--- TUTORIAL DURATION ---")]
    public float tutDur = 2.5f;


    [Header("Tutorial Canvas:")]
    public Canvas tutCanvas;

    #region Main Elemetns:

    [Header("List Of Components:")]
    public GameObject startGame;
    public GameObject swipeElement;
    public GameObject jumpOver;
    public GameObject slideBelow;
    public GameObject regularElement;


    #endregion

    [Header("Player Controller Ref:")]
    public PlayerController Player;

    private void Start()
    {
        if (Player == null)
            Player = FindObjectOfType<PlayerController>();

        playerBehaviour = Player.GetComponent<PlayerBehaviour>();

        if (help == TutorialHelp.start)
        {
            startGame.SetActive(true);
            Timing.RunCoroutine(_Helper().CancelWith(gameObject));
        }

    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.GetComponent<PlayerController>() != null)
        {
            Player = player.GetComponent<PlayerController>();

            Player.canMove = false;

            switch (help)
            {
                case TutorialHelp.swipe:
                    swipeElement.SetActive(true);
                    Timing.RunCoroutine(_Helper().CancelWith(gameObject));
                    break;

                case TutorialHelp.jump:
                    playerBehaviour.isTutorial = false;
                    jumpOver.SetActive(true);
                    Timing.RunCoroutine(_Helper().CancelWith(gameObject));
                    break;

                case TutorialHelp.slide:
                    playerBehaviour.isTutorial = false;
                    slideBelow.SetActive(true);
                    Timing.RunCoroutine(_Helper().CancelWith(gameObject));
                    break;

                case TutorialHelp.regular:
                    Player.canMove = true;
                    regularElement.SetActive(true);
                    Timing.RunCoroutine(_Helper().CancelWith(gameObject));
                    break;

            }
        }
    }

    IEnumerator<float> _Helper()
    {
        do
        {
            switch (help)
            {
                case TutorialHelp.start:
                    hasDone = playerBehaviour.startedMoving;
                    break;

                case TutorialHelp.swipe:
                    hasDone = playerBehaviour.hasSwiped;
                    break;

                case TutorialHelp.jump:
                    hasDone = playerBehaviour.jumping;
                    break;

                case TutorialHelp.slide:
                    hasDone = !playerBehaviour.jumping;
                    break;
            }


            if (hasDone)
                break;



            tutDur -= Time.deltaTime;
            yield return 0;
        } while (tutDur > 0);

        switch (help)
        {
            case TutorialHelp.start:
                startGame.SetActive(false);
                break;

            case TutorialHelp.swipe:
                swipeElement.SetActive(false);
                break;

            case TutorialHelp.jump:
                jumpOver.SetActive(false);
                break;

            case TutorialHelp.slide:
                slideBelow.SetActive(false);
                break;

            case TutorialHelp.regular:
                regularElement.SetActive(false);
                break;

        }

        if (playerBehaviour.startedMoving)
            Player.canMove = true;
    }
}
