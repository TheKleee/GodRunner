using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerBehaviour : MonoBehaviour
{
    #region 18+

    private Camera cam;         //Told ya not to look >:0
    [HideInInspector] public Tapable interact;   //This is what we are interacting with C:<
    [HideInInspector] public bool isInteracting;
    private bool slideOrJump = true;
    private bool isSwiping;
    public bool isTutorial;

    #endregion

    [Header("Swipe Info:")]
    public Swipe swipe;

    #region Remove this:
    public bool startedMoving;
    #endregion
    private void Awake()
    {
        Application.targetFrameRate = 60;   //Test...
        cam = FindObjectOfType<Camera>();
    }

    //This will need more stuff... We'll see : D
    private void Update()
    {
        #region Mobile:

        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isSwiping = true;
            Ray raycast = cam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit rayHit;
            if (Physics.Raycast(raycast, out rayHit))
            {
                if (rayHit.transform.GetComponent<Tapable>() != null)
                {
                    interact = rayHit.transform.GetComponent<Tapable>();
                    if (!interact.isTapped && interact.isTriggered)
                    {
                        isInteracting = true;
                        Timing.RunCoroutine(_TapFix().CancelWith(gameObject));
                    }
                }

                if (rayHit.transform.GetComponent<RunStart>() != null && !startedMoving)
                {
                    startedMoving = true;
                    rayHit.transform.GetComponent<RunStart>().OpenGate();
                }
            }
        }

        #endregion

        #region pc:

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/fileName");
        }

        //the pc input => this should be remvoed before building the project (or commented out at least) : )
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            Ray clickcast = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickcast, out RaycastHit hit))
            {
                if (hit.transform.GetComponent<Tapable>() != null)
                {
                    interact = hit.transform.GetComponent<Tapable>();
                    if (!interact.isTapped && interact.isTriggered)
                    {
                        isInteracting = true;
                        Timing.RunCoroutine(_TapFix().CancelWith(gameObject));
                    }
                }

                if (hit.transform.GetComponent<RunStart>() != null && !startedMoving)
                {
                    startedMoving = true;
                    hit.transform.GetComponent<RunStart>().OpenGate();
                }
            }
        }

        #endregion

        #region Swiping Detection:

        if (isSwiping && !swipe.swiping)
        {
            isSwiping = false;
            swipe.swiping = true;
            swipe.CheckSwipe();
            Timing.RunCoroutine(_StopSwipeDetection().CancelWith(gameObject));
        }

        #endregion



        #region Interaction:

        if (isInteracting)
        {
            if (swipe.swipeLeft || swipe.swipeRight || swipe.swipeDown || swipe.swipeUp)
            {
                hasSwiped = true;
                isInteracting = false;
                interact.PostTapAction();
                interact = null;
                Timing.RunCoroutine(_HasSwipedAction().CancelWith(gameObject));
            }
        }

        #endregion


        #region Slide or Jump:

        else if (slideOrJump && !isInteracting)
        {
            if (!isTutorial)
            {
                if (swipe.swipeUp)
                {
                    jumping = true;
                    Timing.RunCoroutine(_JumpOrSlide(.75f).CancelWith(gameObject));
                }
                if (swipe.swipeDown)
                {
                    jumping = false;
                    Timing.RunCoroutine(_JumpOrSlide(.5f).CancelWith(gameObject));
                }
            }
        }

        #endregion
    }

    IEnumerator<float> _HasSwipedAction()
    {
        yield return Timing.WaitForSeconds(.2f);
        hasSwiped = false;
    }

    [HideInInspector] public bool hasSwiped;

    [HideInInspector] public bool jumping = true;
    private IEnumerator<float> _JumpOrSlide(float duration)
    {
        slideOrJump = false;

        if (jumping)
            GetComponent<PlayerController>().Jump();
        else
            GetComponent<PlayerController>().Slide();


        yield return Timing.WaitForSeconds(duration);  //Delay before you can click this again!!!

        //Restart everything to default!
        slideOrJump = true;
    }


    private IEnumerator<float> _StopSwipeDetection()
    {
        yield return Timing.WaitForSeconds(.2f);

        if (!swipe.canSwipe)
            swipe.swiping = false;
    }

    private IEnumerator<float> _TapFix()
    {
        yield return Timing.WaitForSeconds(.25f);
        interact = null;
        isInteracting = false;
    }
}
