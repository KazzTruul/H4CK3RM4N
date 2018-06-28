using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*By Björn Andersson*/

    /*Short script to handle transitions between scenes.*/

public class SceneLoader : MonoBehaviour {

    public void LoadScene(int newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
