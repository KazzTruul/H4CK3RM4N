using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*By Björn Andersson*/

/*Script for handling any input the player enters into the code window.*/

public class CodeInputHandler : MonoBehaviour
{
    [SerializeField]
    Text text;

    [SerializeField]
    Scrollbar scrollbar;

    InputField field;

    delegate void InputChecker(string[] command);

    InputChecker currentInputChecker;

    string currentlyAccessed = "Base";

    string currentUser = "You", pwToCheck;

    bool pwSubmitted = false;

    void Awake()
    {
        field = GetComponent<InputField>();
        currentInputChecker = InterpretCommand;
        StartCoroutine("AutoFocus");
    }

    IEnumerator AutoFocus()
    {
        yield return new WaitForEndOfFrame();
        field.ActivateInputField();
    }

    public void SubmitCommand()
    {
        if (!Input.GetKey(KeyCode.End) && !Input.GetKey(KeyCode.Return))
            return;
        string[] command = field.text.Split();
        field.text = "";
        currentInputChecker(command);
        field.ActivateInputField();
        scrollbar.value = 0f;
    }

    void UserHacked(string[] command)
    {
        switch(command[0].ToUpper())
        {
            case "LOGOFF":
                currentUser = "You";
                break;

            default:
                AddConsoleText("Command \"" + command[0] + "\" unknown.");
                return;
        }
    }

    void InterpretCommand(string[] command)
    {     
        switch(command[0].ToUpper())
        {
            case "ACCESS":
                Access(command[1]);
                break;

            case "LOGIN":
                Login(command[1]);
                break;

            case "EXECUTE":
                Execute(command[1]);
                break;

            default:
                AddConsoleText("Command \"" + command[0] + "\" unknown.");
                return;
        }
    }

    void AddConsoleText(string newText)
    {
        text.text += newText + "\n" ;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, 45f * text.cachedTextGenerator.lineCount);
        XMLManager.SaveConsoleText(newText);
        scrollbar.value = 0f;
    }

    void Access(string location)
    {
        switch (location.ToUpper())
        {
            case "MAIL":

                break;

            case "CHATTR":

                break;

            default:
                AddConsoleText(location + " not found.");
                return;
        }
    }

    void Login(string userName)
    {
        string password = XMLManager.GetPassword(userName.ToUpper());
        if (password != "")
            StartCoroutine(AwaitingPassword(userName, password));
        else
            AddConsoleText("User " + userName + " not found.");
    }

    void PasswordSubmitted(string[] passwordArr)
    {
        pwToCheck = passwordArr[0];
        pwSubmitted = true;
    }

    IEnumerator AwaitingPassword(string userName, string password)
    {
        currentInputChecker = PasswordSubmitted;
        AddConsoleText("Please enter password.");
        yield return new WaitUntil(() => pwSubmitted == true);
        if (password == pwToCheck)
        {
            AddConsoleText("Login Successful! Welcome " + userName +"!");
            currentUser = userName;
            currentInputChecker = UserHacked;
        }
        else
            AddConsoleText("Password incorrect. Login failed.");
        currentInputChecker = InterpretCommand;
        pwSubmitted = false;
    }

    void Execute(string command)
    {
        switch(command)
        {
            default:
                AddConsoleText("Command " + command + " not recognized.");
                return;
        }
    }
}
