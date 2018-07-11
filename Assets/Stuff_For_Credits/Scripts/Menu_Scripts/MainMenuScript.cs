using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*By Björn Andersson*/

    /*Handles all the main menu functionality*/

public class MainMenuScript : MonoBehaviour {

    [SerializeField]
    GameObject confirmQuitMenu, settingsMenu, savedGamesMenu;

    public void NewGame()
    {
        SceneManager.LoadScene(2);
    }

    //Loads saved games via XML
    public void LoadGame(int saveNo)
    {
        XMLManager.LoadGame(saveNo);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSavedGames(bool show)
    {
        savedGamesMenu.SetActive(show);
    }

    public void ShowQuitConfirm(bool show)
    {
        confirmQuitMenu.SetActive(show);
    }
}
