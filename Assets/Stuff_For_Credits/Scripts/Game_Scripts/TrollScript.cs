﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

    /*Short script to activate Hard Mode.*/
public class TrollScript : MonoBehaviour {

    [SerializeField]
    RectTransform wallpaper;

    public void HardMode(int activate)
    {
        if (activate == 1)
            StartCoroutine("HardModeCoRoutine");
        else if (activate == 0)
        {
            StopCoroutine("HardModeCoRoutine");
            wallpaper.rotation = new Quaternion(0f, 0f, 0f, wallpaper.rotation.w);
        }
    }

    IEnumerator HardModeCoRoutine()
    {
        /*
        Quaternion rot = wallpaper.localRotation;
        rot.z = Random.Range(-180f, 180f);
        print(rot.z);
        wallpaper.localRotation = rot;
        */
        wallpaper.localPosition = new Vector3(Random.Range(-200f, 200f), Random.Range(-200f, 200f), 0f);
        wallpaper.Rotate(new Vector3(0f, 0f, Random.Range(-360f, 360f)));
        yield return new WaitForSecondsRealtime(Random.Range(0.2f, 1.4f));
        StartCoroutine("HardModeCoRoutine");
    }

}
