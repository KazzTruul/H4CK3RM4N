using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Example;

/*By Björn Andersson*/

/*Short script to change the name of the person the player is chatting with.*/

public class ChatNameChanger : MonoBehaviour
{
    Text chatName;

    string currentName = "test";

    public string CurrentName
    {
        get { return this.currentName; }
    }

    private void Start()
    {
        chatName = GetComponent<Text>();
    }

    [YarnCommand("ChangeName")]
    public void ChangeChatName(string newName)
    {
        chatName.text = newName;
        FindObjectOfType<ExampleDialogueUI>().CurrentName = newName;
    }
}
