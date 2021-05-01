using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{
    #region Simple is best -.- but we are complicated...

    /*[HideInInspector]*/
    public bool canMove;  //Call this from another .cs to start moving!!!
    private Rigidbody rb;
    [HideInInspector] public Camera cam;
    private Quaternion storeAngle;
    Quaternion deltaRotation;
    private bool testF;
    private Swipe swipe;
    private bool isDead;
    private UiController uiController;
    [HideInInspector] public SfxManager sfxManager;
    private PlayerBehaviour beh;

    #endregion

    [Space]
    [Header("Powerups")]
    public bool autoCast;
    public bool isFlying;

    [Header("Animator:")]
    public Animator anim;

    [Header("Dirt Trail:")]
    public GameObject dirt;

    [Header("Current Level:")]
    public int curLvl;

    [Header("Life State:")]
    public lifeState lifeStatus;

    [Header("Movement Parameters:")]
    public float moveSpeed;
    public float rotSpeed;

    [Header("Win UI:")]
    public GameObject winUI;
    public GameObject totalPoints;
    public GameObject progressBar;

    [Header("--- TARGET ---")]
    public Transform target;   //make this a transform later on -.-

    private void Awake()
    {
        if (curLvl == 0)
            curLvl = SaveData.instance.lvl;

        rb = GetComponent<Rigidbody>();

        cam = GetComponentInChildren<Camera>();

        swipe = GetComponent<Swipe>();

        transform.eulerAngles = Vector3.zero;

        uiController = GetComponent<UiController>();

        sfxManager = GetComponent<SfxManager>();

        beh = GetComponent<PlayerBehaviour>();
    }

    private void Start()
    {
        TinySauce.OnGameStarted(levelNumber: "Level_ " + (SaveData.instance.lvl - 1));
    }

    void FixedUpdate()
    {
        if (!reviving)
        {
            if (lifeStatus == lifeState.ALIVE && target != null && canMove)
            {
                if (!testF)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                #region Move this rotation to a Qoroutine!!!
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotSpeed * Time.deltaTime);
                #endregion

                if (transform.localEulerAngles.x > 0)
                {
                    rb.angularVelocity = Vector3.zero;
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        new Quaternion(0, transform.rotation.y, 0, transform.rotation.w),
                        rotSpeed * Time.deltaTime); //Test... : /
                }

                #region Decide what you wanna do with this!!! -.-

                if (!isJumping)
                {
                    if (Physics.Raycast(transform.position, -transform.up, out RaycastHit ground, 5))   //We'll be removing this!!!
                    {
                        if (ground.transform != null)
                        {
                            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if (!testF)
                        {
                            TestForce();
                        }
                    }
                }
                else
                    transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                #endregion

            }

            if (transform.position.y < -23f)
                Revive(); //Fix this later!!!
        }
    }

    private void TestForce()
    {
        beh.isTutorial = true;
        rb.useGravity = true;
        testF = true;

        anim.SetBool("fall", true);
        Vector3 checkPos = transform.position;
        Timing.RunCoroutine(_StandingInPlace(checkPos).CancelWith(gameObject));

        rb.AddForce(transform.forward * 5, ForceMode.Impulse);

        Timing.RunCoroutine(_CanMoveDelay().CancelWith(gameObject));
    }
    IEnumerator<float> _CanMoveDelay()
    {
        rb.freezeRotation = false;
        //rb.isKinematic = false;
        Timing.RunCoroutine(_DirtDelay().CancelWith(gameObject));
        canMove = false;
        yield return Timing.WaitForSeconds(.45f);

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5))
        {
            if (hit.transform != null && !isDead)
            {
                beh.isTutorial = false;
                RunStart();
                anim.SetBool("fall", false);
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out RaycastHit death, 5))
        {
            if (death.transform != null)
            {
                Revive();
            }
        }
        else
            canMove = false;
    }

    [Header("Progress:")]
    public ProgressBar pBar;
    /// <summary>
    /// Call this when you open the first gate!!! >:)
    /// ... and all the other times when you need to set canMove to true... :|
    /// </summary>
    public void RunStart()
    {
        pBar.CheckProgress();
        rb.useGravity = false;
        rb.freezeRotation = true;
        //rb.isKinematic = true;
        canMove = true;
        dirt.SetActive(true);
        anim.SetBool("run", true);
    }

    /// <summary>
    /// Run this whenever the player needs to run the death animation!!!
    /// </summary>
    public void Dead()
    {
        TinySauce.OnGameFinished(levelNumber: "Level_" + (SaveData.instance.lvl - 1), false, SaveData.instance.points);


        isDead = true;
        canMove = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = new Vector3(.5f, 0, .5f);


        anim.SetBool("dead", true);
        Timing.RunCoroutine(_DirtDelay().CancelWith(gameObject));

        ShakeCamera();


        rb.AddRelativeForce((-transform.forward + transform.up) * 5, ForceMode.Impulse);

        Timing.RunCoroutine(_TimeToDie().CancelWith(gameObject));
        //Has died :C
    }
    IEnumerator<float> _TimeToDie()
    {
        yield return Timing.WaitForSeconds(.5f);

        SceneManager.LoadSceneAsync(0);
    }


    /// <summary>
    /// This is called from TargetController.cs!!! >:O
    /// </summary>
    public void ChangeTarget(targetType tType, Transform newTarget)
    {
        switch (tType)
        {
            default:
                //Bezier
                target = newTarget;
                break;

            case targetType.WIN:
                //sfxManager.CallSfx(4);

                SaveData.instance.lvl = curLvl + 1; //We'll load the next level from here!

                totalPoints.SetActive(false);
                progressBar.SetActive(false);
                winUI.SetActive(true);
                uiController.SetEndPoints(bonusPts);

                anim.SetBool("won", true);
                cam.GetComponentInParent<CamParent>().GameWon(transform);

                Timing.RunCoroutine(_DirtDelay().CancelWith(gameObject));

                target = null;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;
        }
    }

    /// <summary>
    /// Call this whenever you want your camera to shake!!!
    /// You can expose and change the values from different scripts if you want to... :\
    /// </summary>
    [Header("Shake Values:")]
    public float shakeMagnitude = 3f;
    public float shakeRoughness = 3.5f;
    public float shakeFadeInTime = .3f;
    public float shakeFadeOutTime = 1f;
    public void ShakeCamera()
    {
        CameraShaker.Instance.ShakeOnce(
            shakeMagnitude,
            shakeRoughness,
            shakeFadeInTime,
            shakeFadeOutTime
            );   //Yes officer, the bomb's right here! >:|
    }


    IEnumerator<float> _DirtDelay()
    {
        yield return Timing.WaitForSeconds(.5f);
        dirt.SetActive(false);
    }


    #region Special Powerups:

    public void Autocast()
    {
        autoCast = true;
        Timing.RunCoroutine(_Auto().CancelWith(gameObject));
    }

    private IEnumerator<float> _Auto()
    {
        yield return Timing.WaitForSeconds(5f);
        autoCast = false;
    }

    //public void Flying()
    //{
    //    isFlying = true;
    //    Timing.RunCoroutine(_Fly().CancelWith(gameObject));
    //}

    //private IEnumerator<float> _Fly()
    //{
    //    //float flyDur = 3f;

    //    yield return Timing.WaitForSeconds(15);

    //    rb.velocity = Vector3.zero;
    //    isFlying = false;
    //}

    #endregion

    #region Jump And Slide:

    private bool isJumping;
    public void Jump()
    {
        int randSfx = Random.Range(0, 2);
        sfxManager.CallSfx(randSfx);

        isJumping = true;
        anim.SetBool("jump", true);
        //GetComponent<BoxCollider>().size = new Vector3(.5f, .5f, .5f);
        GetComponent<CapsuleCollider>().height = 1;
        //transform.position += new Vector3(0, .35f, 0);

        Timing.RunCoroutine(_Jumping().CancelWith(gameObject));
    }

    public void Slide()
    {
        int randSfx = Random.Range(2, 4);
        sfxManager.CallSfx(randSfx);


        anim.SetBool("slide", true);
        //GetComponent<BoxCollider>().size = new Vector3(.5f, .5f, .5f);
        GetComponent<CapsuleCollider>().height = 1;
        //transform.position += new Vector3(0, .35f, 0);

        Timing.RunCoroutine(_Sliding().CancelWith(gameObject));
    }

    IEnumerator<float> _Jumping()
    {
        yield return Timing.WaitForSeconds(.5f);
        anim.SetBool("jump", false);
        yield return Timing.WaitForSeconds(.25f);
        //GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
        GetComponent<CapsuleCollider>().height = 2;
        if(!beh.isTutorial)
            isJumping = false;
    }

    IEnumerator<float> _Sliding()
    {
        yield return Timing.WaitForSeconds(.5f);
        //GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
        GetComponent<CapsuleCollider>().height = 2;
        anim.SetBool("slide", false);
    }

    #endregion

    private IEnumerator<float> _StandingInPlace(Vector3 curPos)
    {
        float movedDist = Vector3.Distance(transform.position, curPos);
        yield return Timing.WaitForSeconds(.5f);
        float newDist = Vector3.Distance(transform.position, curPos);
        if (movedDist + .05f >= newDist)
            Revive();
    }

    public void YouWon()
    {
        SaveData.instance.SaveGame();
        TinySauce.OnGameFinished(levelNumber: "Level_" + (SaveData.instance.lvl - 1), true, SaveData.instance.points);
        SceneManager.LoadSceneAsync(0);
    }

    #region Wisps to the rescue! >: D

    [Header("Wisps:")]
    public GameObject smallWisp;
    public GameObject largeWisp;    //3 Small wisps combine into a large one!!!

    [Header("Wisp Parent:")]
    public Transform wispParent;
    [HideInInspector] public int wispCount;
    [HideInInspector] public int largeWispCount;
    /*[HideInInspector]*/ public Transform checkPoint;  //Feed targets to revive the player...

    private List<GameObject> SWisps = new List<GameObject>();
    private List<GameObject> LWisps = new List<GameObject>();

    public void SummonWisp()
    {
        //Summon a small wisp...
        GameObject wisp = Instantiate(smallWisp,
            new Vector3(
                wispParent.position.x,
                wispParent.position.y + .75f,
                wispParent.position.z
                ),
            Quaternion.identity);
        wisp.transform.SetParent(wispParent);
        SWisps.Add(wisp);

        //Check for the wisp count...
        wispCount++;

        if (wispCount == 3)
        {
            //Summon a large wisp... destroy 3 small wisps...
            foreach (GameObject smallWisp in SWisps)
                Destroy(smallWisp);

            wispCount = 0;

            GameObject LargeWisp = Instantiate(largeWisp,
                new Vector3(
                    wispParent.position.x,
                    wispParent.position.y + .75f,
                    wispParent.position.z
                    ),
                Quaternion.identity);
            LargeWisp.GetComponent<WispController>().bonusID = largeWispCount;
            LargeWisp.transform.SetParent(wispParent);

            LWisps.Add(LargeWisp);
            largeWispCount++;
        }
    }

    private bool reviving;
    public void Revive()
    {
        if (checkPoint != null)
        {
            if (largeWispCount > 0)
            {
                anim.SetBool("fall", false);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                reviving = true;
                Timing.RunCoroutine(_SpecialSummon().CancelWith(gameObject));
            }
        }
        else
            Dead();
    }

    IEnumerator <float> _SpecialSummon()
    {
        Timing.RunCoroutine(_ReviveSpawnDelay().CancelWith(gameObject));
        yield return Timing.WaitForSeconds(.5f);
        SummonWisp();
        yield return Timing.WaitForSeconds(.15f);
        SummonWisp();
        //Revive at last checkpoint... No idea how will this look xD
    }
    IEnumerator<float> _ReviveSpawnDelay()
    {
        LWisps[LWisps.Count - 1].GetComponent<WispController>().GoToCheckPoint(checkPoint, transform);
        LWisps.RemoveAt(LWisps.Count - 1);
        largeWispCount--;
        RunStart();

        yield return Timing.WaitForSeconds(.5f);
        reviving = false;
    }

    [HideInInspector] public int bonusPts;
    public void EndLevelBonus()
    {

        beh.isTutorial = true;
        isJumping = true;

        if (largeWispCount > 0)
        {
            //Add next target...
            //Summon Stairs
            //Remove an LWisp!! >: D
            ChangeTarget(targetType.CURVED, target.GetComponent<Target>().target);
            LWisps[0].GetComponent<WispController>().CallStairs();
            LWisps.RemoveAt(0);
            largeWispCount--;

        } else {
            ChangeTarget(targetType.WIN, target);
        }
    }

    #endregion
}
