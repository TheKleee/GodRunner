using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapParent : MonoBehaviour
{
    #region 18+ C:<
    private Tapable tapChild;    //This is the child ref!!!
    [HideInInspector] public PlayerController Player;
    #endregion

    void Awake()
    {
        tapChild = GetComponentInChildren<Tapable>();
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.GetComponent<PlayerController>() != null)
        {
            Player = player.GetComponent<PlayerController>(); //Delete if useless! >:O 

            //Player.addTappable(tapChild);
            //Debug.Log("added to tappables!");

            GetComponent<Collider>().enabled = false;

            tapChild.Player = Player;
            tapChild.SetDuration();
            tapChild.SetTrigger();

            if (Player.autoCast)
                tapChild.PostTapAction();
        }
    }
}
