using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Is Clickable:")]
    public bool isClickable = true;
    [Space]
    public bool isPoint;

    [Header("Player Ref:")]
    public Transform player;

    void OnEnable()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>().transform;

        if (!isPoint)
        {
            if (isClickable)
            {
                LeanTween.scale(gameObject,
                    new Vector3(transform.localScale.x * 1.05f,
                    transform.localScale.y * 1.05f,
                    transform.localScale.z * 1.05f),
                    .75f)
                    .setLoopPingPong();
            }
            else
            {
                LeanTween.move(gameObject,
                    new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z),
                    .5f)
                    .setLoopPingPong();
            }
        }
    }

    private void LateUpdate()
    {
        if (!isClickable)
        {
            transform.LookAt(player);
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }
}
