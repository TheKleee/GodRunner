using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Target : MonoBehaviour
{
    #region Private stuff...

    private PlayerController Player;    //We'll call this later!!! >: O
    [Header("Bonus Points:")]
    public int xPts;   //These are the bonus pts..

    #endregion

    [Header("Type:")]
    public targetType tType;

    [Header("Next Target:")]
    public Transform target;

    [Header("--- Special ---")]
    public GameObject bonusPtsPlat;

    private void OnTriggerEnter(Collider player)
    {
        if(player.GetComponent<PlayerController>() != null)
        {
            Player = player.GetComponent<PlayerController>();    //Told ya! >:P

            if (tType != targetType.WIN)
            {
                Player.ChangeTarget(tType, target);  //Let's change you nicely >:D
                Player.checkPoint = transform;
            }
            else
            {
                Player.bonusPts = xPts;
                Player.EndLevelBonus();
                Timing.RunCoroutine(_BonusPts().CancelWith(gameObject));
            }
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator<float> _BonusPts()
    {
        yield return Timing.WaitForSeconds(.5f);
        bonusPtsPlat.SetActive(false);
    }
}
