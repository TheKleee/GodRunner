using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;


[RequireComponent(typeof(PlayerController))]
public class UiController : MonoBehaviour
{
    #region Private area...

    private PlayerController player;
    [HideInInspector] public int pointsGathered;    //Save these in SaveData.cs =/
    [HideInInspector] public int numOfSprites = 1;  //The number of instantiated sprites...


    #endregion
    [Header("Canvas Ref:")]
    public Canvas mainCanvas;

    [Header("Spawn Position:")]
    public Transform testUiPos;

    [Header("Points Img:")]
    public GameObject pointsSprite;

    [Header("Points Text:")]
    public Text pointsTxt;
    public GameObject totalPoints;

    [Header("End Game Points:")]
    public Text endPointsTxt;
    public Text tapToContinue;

    [Header("Move To Img Location:")]
    public Image pointsLabel;   //Main points on the canvas

    private void Start()
    {
        player = GetComponent<PlayerController>();

        pointsGathered = SaveData.instance.points;
        pointsTxt.text = pointsGathered.ToString();
    }

    public void SetPickupSprite()
    {
        int randSfx = Random.Range(5, 7);
        player.sfxManager.CallSfx(randSfx);

        Vector3 imgPos = player.cam.WorldToScreenPoint(testUiPos.position);
        var point = Instantiate(pointsSprite, imgPos, pointsSprite.transform.rotation);

        point.transform.SetParent(mainCanvas.transform);

        pointsGathered++;

        Timing.RunCoroutine(_CallPoints().CancelWith(gameObject));

        LeanTween.move(point.gameObject, pointsLabel.transform, .3f)
            .setDestroyOnComplete(point.gameObject);
    }

    IEnumerator<float> _CallPoints()
    {
        yield return Timing.WaitForSeconds(.3f);
        AddPoitns();

        LeanTween.scale(totalPoints, new Vector3(1.2f, 1.2f, 1.2f), .1f);
        yield return Timing.WaitForSeconds(.2f);
        LeanTween.scale(totalPoints, new Vector3(1f, 1f, 1f), .1f);
    }

    public void AddPoitns()
    {
        pointsTxt.text = pointsGathered.ToString();
    }

    public void SetEndPoints(int bonusPts)
    {
        pointsGathered *= bonusPts;
        endPointsTxt.text = pointsGathered.ToString();
        SaveData.instance.points = pointsGathered;
        SaveData.instance.SaveGame();
        LeanTween.scale(tapToContinue.gameObject, new Vector3(1.2f, 1.2f, 1.2f), .6f).setLoopPingPong();
    }
}
