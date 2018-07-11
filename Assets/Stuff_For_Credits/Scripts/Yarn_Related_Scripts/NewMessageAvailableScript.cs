using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity.Example;
using Yarn.Unity;
using System.IO;

/*By Björn Andersson*/

/*Short script to allow the player to access new conversations and advance the narrative.*/


public class NewMessageAvailableScript : MonoBehaviour
{
    [SerializeField]
    string nameOfAI, defaultNode;

    [SerializeField]
    Text newMessageText;

    Button myButton;

    string currentNode;

    bool showNewMessageText = true;

    private void OnEnable()
    {
        myButton = GetComponent<Button>();
        string availableNode = XMLManager.GetLatestNode(nameOfAI);

        NewMessage(availableNode);
        if (currentNode != availableNode && XMLManager.GetNewMessageAvailable(nameOfAI) == "true")
        {
            StartCoroutine("BlinkNewMessageText");
        }
        else
        {
            newMessageText.text = "";
        }
    }

    public void NewCurrentNode(string newNode)
    {
        this.currentNode = newNode;
    }

    public void NewMessage(string newNode)
    {
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => FindObjectOfType<ExampleDialogueUI>().CurrentName = nameOfAI);
        myButton.onClick.AddListener(() => FindObjectOfType<ExampleDialogueUI>().StartDialogue(newNode));
        myButton.onClick.AddListener(() => this.NewCurrentNode(newNode));
    }

    IEnumerator BlinkNewMessageText()
    {
        newMessageText.gameObject.SetActive(showNewMessageText);
        yield return new WaitForSeconds(0.7f);
        showNewMessageText = !showNewMessageText;
        StartCoroutine("BlinkNewMessageText");
    }
}