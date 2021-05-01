using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    #region Private stuff 18+ xD

    private UiController uiCont;

    #endregion
    [Header("Is Wisp?")]
    public bool wisp;

    [Header("Is Pickupable?")]
    public bool pickUp;     //If true... see OnTriggerEnter() :D

    [Header("Value:")]
    public int pointValue;  //The number of points you'll get from picking this object up/running the script

    [Header("Vfx:")]
    public GameObject vfx;

    public void GetPoints()
    {
        //Just get the points...
        //SaveData.instance.points += pointValue;

        if (vfx != null)
            Instantiate(vfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider player)
    {
        if (pickUp)
        {
            if (player.GetComponent<UiController>() != null)
            {
                uiCont = player.GetComponent<UiController>();
                uiCont.SetPickupSprite();

                GetPoints();
            }
        }

        else if (wisp)
        {
            if(player.GetComponent<PlayerController>() != null)
            {
                player.GetComponent<PlayerController>().SummonWisp();

                GetPoints();
            }
        }
    }
}
