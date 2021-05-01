using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class ProgressBar : MonoBehaviour
{
    private float maxLevelDur;

    [Header("Level Duration In Seconds:")]
    public float levelDur = 60;

    [Header("Level Id:")]
    public Text levelId;

    [Header("Level Progress Bar:")]
    [Space]
    public Image levelProgress;
    [Space]
    public Image currentPoint;



    private void Start()
    {
        maxLevelDur = levelDur;

        //if (levelId != null)
        //    levelId.text = SaveData.instance.lvl.ToString();
    }

    public void CheckProgress()
    {
        Timing.RunCoroutine(_Progress().CancelWith(gameObject));
    }

    IEnumerator<float> _Progress()
    {
        float tick = maxLevelDur / 100;
        do
        {
            //Check if you reached the end:
            if (levelDur <= 0)
                break;

            //Current Point Position:
            if (levelProgress.fillAmount > 0 && currentPoint.GetComponent<RectTransform>().localPosition.x < 350)
                currentPoint.GetComponent<RectTransform>().anchoredPosition
                    = new Vector3(350*levelProgress.fillAmount, 0, 0);

            //Progress Bar Controller:
            levelProgress.fillAmount += (tick / maxLevelDur);

            levelDur -= tick;
            yield return Timing.WaitForSeconds(tick);
        } while (levelDur > 0);
    }
}
