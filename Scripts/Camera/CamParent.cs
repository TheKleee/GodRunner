using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class CamParent : MonoBehaviour
{
    public void GameWon(Transform target)
    {
        Timing.RunCoroutine(_SetTarget(target).CancelWith(gameObject));
    }

    IEnumerator<float> _SetTarget(Transform t)
    {
        yield return Timing.WaitForSeconds(.35f);
        while(t != null)
        {
            if (GetComponentInChildren<Camera>().fieldOfView < 90)
                GetComponentInChildren<Camera>().fieldOfView += .05f;

            transform.RotateAround(t.position, Vector3.up, -35 * Time.deltaTime);
            yield return Timing.WaitForSeconds(0);
        }
    }
}
