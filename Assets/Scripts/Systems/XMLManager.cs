using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity.Example;

/*By Björn Andersson*/

/*Script that handles all saving and loading of information to and from XML-files. Also handles the transition and transfer of information between scenes.*/

public class XMLManager : MonoBehaviour
{

    static int saveNo;

    static bool loadingGame = false, listenerAdded = false;

    static GameObject savedGamesMenu;

    static XPathNavigator yarnXMLnav;

    static XmlDocument yarnXML = new XmlDocument();

    void Start()
    {
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
                GameObject saveButton = GameObject.Find("SavedGameButton" + i);
                if (File.Exists(Application.dataPath + ("/Saved_Game_" + i.ToString() + ".xml")))
                {
                    saveButton.GetComponent<Button>().onClick.AddListener(delegate { LoadGame(i); });
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
            ExampleDialogueUI example = FindObjectOfType<ExampleDialogueUI>();
            example.SaveMethod = SaveConversation;
            example.dialogueStarted.AddListener(SendConversation);
            /*
            XmlDocument refXML = new XmlDocument();
            refXML.Load(Application.dataPath + "/Reference_XML.xml");
            XPathNavigator nav = refXML.CreateNavigator();
            */
            if (loadingGame)
            {
                //TODO: add loading functionality
            }
        }
    }

    public static void SaveConversation(List<string>[] messages, string name, List<string> nodes)
    {
        if (yarnXMLnav == null)
        {
            yarnXML.Load(Application.dataPath + "/XMLs/Yarn_XML.xml");
            yarnXMLnav = yarnXML.CreateNavigator();
        }
        SaveMessages(messages, name);
        SaveYarnNodes(nodes, name);
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.CloseOutput = true;
        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/Saved_Game_" + saveNo + ".xml", settings))
        {
            yarnXML.Save(writer);
        }
    }

    static List<string>[] RetrieveConversation(string name)
    {
        List<string>[] conversation = new List<string>[2];
        for (int i = 0; i < 2; i++)
            conversation[i] = new List<string>();
        if (yarnXMLnav == null)
        {
            yarnXML.Load(Application.dataPath + "/XMLs/Yarn_XML.xml");
            yarnXMLnav = yarnXML.CreateNavigator();
        }
        XPathNavigator conversationNav = yarnXMLnav.SelectSingleNode("/Yarn/Conversations/" + name);
        XPathNodeIterator messages = conversationNav.SelectChildren(XPathNodeType.All);
        foreach (XPathNavigator message in messages)
        {
            conversation[0].Add(message.GetAttribute("Text", ""));
            conversation[1].Add(message.GetAttribute("Name", ""));
        }
        return conversation;
    }

    static void SendConversation()
    {
        FindObjectOfType<ExampleDialogueUI>().CurrentConversation = RetrieveConversation(FindObjectOfType<ExampleDialogueUI>().CurrentName);
    }

    static void SaveMessages(List<string>[] messages, string name)
    {
        XmlNodeList previousChildren = yarnXML.SelectNodes("//Message");
        if (previousChildren.Count > 0)
            for (int i = previousChildren.Count - 1; i > -1; i--)
                previousChildren[i].ParentNode.RemoveChild(previousChildren[i]);
        XPathNavigator currentConversation = yarnXMLnav.SelectSingleNode("/Yarn/Conversations/" + name);
        for (int i = 0; i < messages[0].Count; i++)
        {
            currentConversation.AppendChild("<Message Text=\"" + messages[0][i] + "\" Name=\"" + messages[1][i] + "\"/>");
        }
    }

    static void SaveYarnNodes(List<string> nodes, string name)
    {
        XPathNavigator nodesNav = yarnXMLnav.SelectSingleNode("/Yarn/VisitedNodes/" + name);
        foreach (string node in nodes)
        {
            nodesNav.AppendChild("<Node Name =\"" + node + "\"/>");
        }
        print("bajs");
    }
}