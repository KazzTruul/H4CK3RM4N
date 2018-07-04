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

    IEnumerator AutoFocus()     //Activates the field when the GameObject is activated.
    {
        yield return new WaitForEndOfFrame();
        field.ActivateInputField();
    }

    public void SubmitCommand()
    {
        if (!Input.GetKey(KeyCode.End) && !Input.GetKey(KeyCode.Return))        //Makes sure the user actually wanted to submit their input and not just deselect the field.
            return;
        string[] command = field.text.Split();
        field.text = "";
        currentInputChecker(command);
        field.ActivateInputField();
        scrollbar.value = 0f;
    }

    void InterpretCommand(string[] command)
    {     
        switch(command[0].ToUpper())
        {
            case "HELP":
                DisplayHelp();
                break;

            case "ACCESS":
                if (command.Length < 2 || command[1] == null)
                {
                    AddConsoleText("Access command must be followed by valid path to access.");
                    return;
                }
                Access(command[1]);
                break;

            case "LOGIN":
                if (command.Length < 2 || command[1] == null)
                {
                    AddConsoleText("Login command must be followed by valid account to access.");
                    return;
                }
                Login(command[1]);
                break;

            case "EXECUTE":
                if (command.Length < 2 || command[1] == null)
                {
                    AddConsoleText("Execute command must be followed by valid process to execute.");
                    return;
                }
                Execute(command[1]);
                break;

            default:
                AddConsoleText("Command \"" + command[0] + "\" not regognized.");
                return;
        }
    }

    void UserHacked(string[] command)
    {
        switch(command[0].ToUpper())
        {
            case "ACCESS":

                break;

            case "EXECUTE":

                break;

            case "LOGOFF":
                currentUser = "You";
                currentlyAccessed = "Base";
                break;

            default:
                AddConsoleText("Command \"" + command[0] + "\" unknown.");
                return;
        }
    }

    void DisplayHelp()
    {
        AddConsoleText(XMLManager.GetHelpText(currentUser, currentlyAccessed));
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
            case "INBOX":

                break;

            case "CHATTR":

                break;

            default:
                AddConsoleText(location + " not found.");
                return;
        }
        currentlyAccessed = location;
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
            currentlyAccessed = "Base";
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
