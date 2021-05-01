using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MEC;

public class TestPlayer : MonoBehaviour
{
    private PlayerController pCont; //You know what this is >:V
    [SerializeField] private float rotationSpeed = 7f;  //We can see this 0.0

    void Awake()
    {
        pCont = GetComponentInParent<PlayerController>();
        //Start Coroutine if you want the special drop-launch effect...
    }

    // Update is called once per frame
    void Update()
    {
        if (pCont.target != null && pCont.canMove)
        {
            //transform.LookAt(pCont.target.position);
            #region Move this rotation to a Qoroutine!!!
            var lookPos = pCont.target.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            #endregion
        }
    }

}
