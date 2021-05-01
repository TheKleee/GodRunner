using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class LoadingTxtEffect : MonoBehaviour
{
    private void Start()
    {
        LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f, 1), .6f).setLoopPingPong();
        //Timing.RunCoroutine(_LoadLevel().CancelWith(gameObject));
        SceneManager.LoadSceneAsync(SaveData.instance.lvl);
    }

    //IEnumerator<float> _LoadLevel()
    //{
    //    yield return 0/*Timing.WaitForSeconds(.5f)*/;
    //    /*AsyncOperation map = */SceneManager.LoadSceneAsync(SaveData.instance.lvl);

    //    //while (map.progress < 1)
    //    //{
    //    //    //Do something if you want... xD
    //    //    yield return 0;
    //    //}
    //}
}


