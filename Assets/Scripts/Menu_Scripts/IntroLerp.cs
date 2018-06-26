using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*By Björn Andersson*/

    /*Short script for the custom splash screen.*/

public class IntroLerp : MonoBehaviour
{
    [SerializeField]
    Text gameNameText;

    [SerializeField]
    float loadSceneWaitTime, fadeWaitTime, lerpValue;

    bool fading = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("StartFadeWait");
    }

    // Update is called once per frame
    void Update()
    {
        if (fading && gameNameText.color.a < 255)
        {
            gameNameText.color = new Color(gameNameText.color.r, gameNameText.color.g, gameNameText.color.b, Mathf.Lerp(gameNameText.color.a, 255, lerpValue * Time.deltaTime));
        }
        if (Input.anyKey)
        {
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator StartFadeWait()
    {
        yield return new WaitForSeconds(fadeWaitTime);
        fading = true;
        StartCoroutine("LoadSceneTimer");
    }

    IEnumerator LoadSceneTimer()
    {
        yield return new WaitForSeconds(loadSceneWaitTime);
        SceneManager.LoadScene(1);
    }
}
