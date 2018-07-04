using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity.Example;
using Yarn.Unity;

/*By Björn Andersson*/

/*Script that handles all saving and loading of information to and from XML- and Yarn-files.*/

public class XMLManager : MonoBehaviour
{
    static int saveNo;

    static bool loadingGame = false, listenerAdded = false;

    static GameObject savedGamesMenu;

    static XPathNavigator currentGameNav;

    static XmlDocument currentGameDoc = new XmlDocument();

    SceneLoader sL;

    void Start()
    {
        sL = GetComponent<SceneLoader>();
        if (!listenerAdded)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        listenerAdded = true;
    }

    public static void LoadGame(int saveToLoad)
    {
        saveNo = saveToLoad;
        loadingGame = true;
        SceneManager.LoadScene(2);
    }

    public static void NewGame()
    {
        SceneManager.LoadScene(2);
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            GameObject.Find("NewGameButton").GetComponent<Button>().onClick.AddListener(NewGame);
            savedGamesMenu = GameObject.Find("SavedGamesMenu");
            for (int i = 0; i < 6; i++)
            {
                GameObject saveButton = GameObject.Find("SavedGameButton" + i.ToString());
                if (File.Exists(Application.dataPath + ("/Saved_Game_" + i.ToString() + ".xml")))
                {
                    int saveDelegate = i;
                    saveButton.GetComponent<Button>().onClick.AddListener(delegate { LoadGame(saveDelegate); });
                    //TODO: add saved game sprites
                }
                else
                {
                    saveButton.SetActive(false);
                }
            }
            savedGamesMenu.SetActive(false);
        }
        else if (scene.buildIndex == 2)
        {
            if (loadingGame)
            {
                //TODO: add more loading functionality
                currentGameDoc.Load(Application.dataPath + ("/Saved_Game_" + saveNo.ToString() + ".xml"));
                loadingGame = false;
            }
            else
            {
                currentGameDoc.Load(Application.streamingAssetsPath + "/Save_Template_XML.xml");
            }
            currentGameNav = currentGameDoc.CreateNavigator();
            ExampleDialogueUI example = FindObjectOfType<ExampleDialogueUI>();
            example.SaveMethod = SaveConversation;
            example.DialogueRetriever = RetrieveConversation;
            //example.dialogueStarted.AddListener(SendConversation);
        }
    }

    public static void SaveConversation(List<string>[] messages, string name, List<string> nodes)
    {
        NoNewMessages(name);
        /*
        if (currentGameNav == null)
        {
            currentGameDoc.Load(Application.streamingAssetsPath + "/Save_Template_XML.xml");
            currentGameNav = currentGameDoc.CreateNavigator();
        }
        */
        SaveMessages(messages, name);
        SaveYarnNodes(nodes, name);
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.CloseOutput = true;
        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/Saved_Game_" + saveNo + ".xml", settings))
        {
            currentGameDoc.Save(writer);
        }
    }

    public static string GetHelpText(string currentUser, string currentlyAccessed)
    {
        //TODO: Add XML-nodes & retrieval for help messages.
        return "";
    }

    static Dialogue RetrieveConversation(string name)
    {
        List<string>[] conversation = new List<string>[2];
        for (int i = 0; i < 2; i++)
            conversation[i] = new List<string>();
        /*
        if (currentGameNav == null)
        {
            currentGameDoc.Load(Application.streamingAssetsPath + "/Save_Template_XML.xml");
            currentGameNav = currentGameDoc.CreateNavigator();
        }
        */
        XPathNavigator conversationNav = currentGameNav.SelectSingleNode("/Game/Yarn/Conversations/" + name);
        if (conversationNav == null)
            return null;
        XPathNodeIterator messages = conversationNav.SelectChildren(XPathNodeType.All);
        foreach (XPathNavigator message in messages)
        {
            conversation[0].Add(message.GetAttribute("Text", ""));
            conversation[1].Add(message.GetAttribute("Name", ""));
        }
        return new Dialogue(conversation, RetrieveYarnNodes(name));
    }

    [YarnCommand("NewNode")]
    public static void NewYarnNode(string person, string node)
    {
        XPathNavigator availableNodes = currentGameNav.SelectSingleNode("/Game/Yarn/AvailableNodes");
        foreach (XPathNavigator availableNode in availableNodes.SelectChildren(XPathNodeType.All))
        {
            if (availableNode.GetAttribute("Name", "") == person)
            {
                /*
                XPathNavigator nodeName = availableNode.SelectSingleNode("/@NodeName");
                nodeName.SetValue(node);
                */
                availableNode.MoveToAttribute("NodeName", "");
                availableNode.SetValue(node);
                return;
            }
        }
    }

    public static string GetLatestNode(string person)
    {
        if (currentGameNav == null)
        {
            currentGameNav = currentGameDoc.CreateNavigator();
        }
        XPathNavigator availableNodes = currentGameNav.SelectSingleNode("/Game/Yarn/AvailableNodes");
        foreach (XPathNavigator availableNode in availableNodes.SelectChildren(XPathNodeType.All))
        {
            if (availableNode.GetAttribute("Name", "") == person)
            {
                return availableNode.GetAttribute("NodeName", "");
            }
        }
        print("Error: no node found for " + person);
        return "";
    }

    static void SaveMessages(List<string>[] messages, string name)
    {
        NoNewMessages(name);
        XmlNodeList previousChildren = currentGameDoc.SelectNodes("//Message");
        if (previousChildren.Count > 0)
            for (int i = previousChildren.Count - 1; i > -1; i--)
            {
                print("Loop 1: " + i);
                previousChildren[i].ParentNode.RemoveChild(previousChildren[i]);
            }
        XPathNavigator currentConversation = currentGameNav.SelectSingleNode("/Game/Yarn/Conversations/" + name);
        for (int i = 0; i < messages[0].Count; i++)
        {
            currentConversation.AppendChild("<Message Text=\"" + messages[0][i] + "\" Name=\"" + messages[1][i] + "\"/>");
        }
    }

    public static void SetNewMessageAvailable(string person, string messageNode)
    {
        XPathNavigator allNewMessages = currentGameNav.SelectSingleNode("/Game/Yarn/NewMessages");
        foreach (XPathNavigator child in allNewMessages.SelectChildren(XPathNodeType.All))
        {
            if (child.GetAttribute("Name", "") == person)
            {
                child.MoveToAttribute("NewMessage", "");
                child.SetValue("true");
            }
        }
        XPathNavigator nodes = currentGameNav.SelectSingleNode("/Game/Yarn/AvailableNode");
        foreach (XPathNavigator child in nodes.SelectChildren(XPathNodeType.All))
        {
            if (child.GetAttribute("Name", "") == person)
            {
                child.MoveToAttribute("AvailableNode", "");
                child.SetValue(messageNode);
            }
        }
    }

    public static string GetNewMessageAvailable(string person)
    {
        XPathNavigator allNewMessages = currentGameNav.SelectSingleNode("/Game/Yarn/NewMessages");
        foreach (XPathNavigator child in allNewMessages.SelectChildren(XPathNodeType.All))
        {
            if (child.GetAttribute("Name", "") == person)
                return child.GetAttribute("NewMessage", "");
        }
        print(person + " resulted in error.");
        return "";
    }

    static void NoNewMessages(string person)
    {
        XPathNavigator allNewMessages = currentGameNav.SelectSingleNode("/Game/Yarn/NewMessages");
        foreach (XPathNavigator child in allNewMessages.SelectChildren(XPathNodeType.All))
        {
            if (child.GetAttribute("Name", "") == person)
            {
                child.MoveToAttribute("NewMessage", "");
                child.SetValue("false");
            }
        }
    }

    static void SaveYarnNodes(List<string> nodes, string name)
    {
        XPathNavigator nodesNav = currentGameNav.SelectSingleNode("/Game/Yarn/VisitedNodes/" + name);
        foreach (string node in nodes)
        {
            nodesNav.AppendChild("<Node Name =\"" + node + "\"/>");
        }
    }

    static List<string> RetrieveYarnNodes(string name)
    {
        List<string> nodes = new List<string>();
        XPathNavigator nodesNav = currentGameNav.SelectSingleNode("/Game/Yarn/VisitedNodes/" + name);
        XPathNodeIterator iterator = nodesNav.SelectChildren(XPathNodeType.All);
        foreach (XPathNavigator node in iterator)
            nodes.Add(node.GetAttribute("Name", ""));
        return nodes;
    }

    public static string GetPassword(string name)
    {
        XmlDocument loginXML = new XmlDocument();
        loginXML.Load(Application.streamingAssetsPath + "/Login_XML.xml");
        XPathNavigator loginNav = loginXML.CreateNavigator();
        foreach (XPathNavigator user in loginNav.Select("//User"))
        {
            print("User found!");
            if (user.GetAttribute("Name", "").ToUpper() == name)
            {
                return user.GetAttribute("Password", "");
            }
        }
        return "";
    }

    public static void SetMailContents(string subject)
    {
        XPathNavigator inboxNode = currentGameNav.SelectSingleNode("/Game/Inbox");
        foreach (XPathNavigator mail in inboxNode.SelectChildren(XPathNodeType.All))
        {
            if (mail.GetAttribute("Subject", "") == subject)
            {
                OpenedMailSizer.MailContents = mail.GetAttribute("Message", "");
                return;
            }
        }
    }

    public static List<string[]> GetInbox()
    {
        List<string[]> inbox = new List<string[]>();
        XPathNavigator inboxNode = currentGameNav.SelectSingleNode("/Game/Inbox");
        foreach(XPathNavigator mailNode in inboxNode.SelectChildren(XPathNodeType.All))
        {
            string[] mail = new string[4];
            mail[0] = mailNode.GetAttribute("Sender", "");
            mail[1] = mailNode.GetAttribute("Receiver", "");
            mail[2] = mailNode.GetAttribute("Subject", "");
            mail[3] = mailNode.GetAttribute("Read", "");
            inbox.Add(mail);
        }
        return inbox;
    }

    public static string GetConsoleText()
    {
        if (currentGameNav == null)
            print("nullnav");
        return currentGameNav.SelectSingleNode("/Game/Console").GetAttribute("Text", "");
    }

    public static void SaveConsoleText(string newText)
    {
        string currentText = GetConsoleText();
        currentText += newText;
        XPathNavigator consoleTextNode = currentGameNav.SelectSingleNode("/Game/Console");
        consoleTextNode.MoveToAttribute("Text", "");
        consoleTextNode.SetValue(currentText);
    }
}