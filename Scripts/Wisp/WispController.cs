using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WispController : MonoBehaviour
{
    [Header("Stairs:")]
    public GameObject[] stairs;

    /*[HideInInspector]*/ public int bonusID;

    private void Start()
    {
        LeanTween.rotateAroundLocal(gameObject, transform.forward, 360, 0.75f).setLoopClamp();
    }

    public void CallStairs()
    {
        //Call stairs at position!!!
        //Fly To position
        //Destroy self  : \
        LeanTween.moveLocal(gameObject, new Vector3(transform.parent.transform.localPosition.x,
            transform.parent.transform.localPosition.y - 2f,
            transform.parent.transform.localPosition.z + 5), .125f);  //test this out >:O
        Timing.RunCoroutine(_Stairs().CancelWith(gameObject));
    }

    IEnumerator<float> _Stairs()
    {
        yield return Timing.WaitForSeconds(.5f);
        Instantiate(stairs[bonusID], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    public void GoToCheckPoint(Transform target, Transform player)
    {
        LeanTween.move(gameObject, target.position, .5f);
        Timing.RunCoroutine(_SetDestroyDelay(player).CancelWith(gameObject));
    }
    IEnumerator<float> _SetDestroyDelay(Transform player)
    {
        yield return Timing.WaitForSeconds(.5f);
        player.transform.position = transform.position;
        Destroy(gameObject);
    }
}
