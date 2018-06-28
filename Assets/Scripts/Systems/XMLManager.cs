﻿using System.Collections;
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

    static XPathNavigator yarnXMLnav;

    static XmlDocument yarnXML = new XmlDocument();

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
                yarnXML.Load(Application.dataPath + ("/Saved_Game_" + saveNo.ToString() + ".xml"));
                loadingGame = false;
            }
            else
            {
                yarnXML.Load(Application.dataPath + "/XMLs/Yarn_XML.xml");
            }
            yarnXMLnav = yarnXML.CreateNavigator();
            ExampleDialogueUI example = FindObjectOfType<ExampleDialogueUI>();
            example.SaveMethod = SaveConversation;
            example.DialogueRetriever = RetrieveConversation;
            //example.dialogueStarted.AddListener(SendConversation);
            /*
            XmlDocument refXML = new XmlDocument();
            refXML.Load(Application.dataPath + "/Reference_XML.xml");
            XPathNavigator nav = refXML.CreateNavigator();
            */
        }
    }

    public static void SaveConversation(List<string>[] messages, string name, List<string> nodes)
    {
        NoNewMessages(name);
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

    static Dialogue RetrieveConversation(string name)
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
        return new Dialogue(conversation, RetrieveYarnNodes(name));
    }

    [YarnCommand("NewNode")]
    public static void NewYarnNode(string person, string node)
    {
        XPathNavigator availableNodes = yarnXMLnav.SelectSingleNode("/Yarn/AvailableNodes");
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
        XPathNavigator availableNodes = yarnXMLnav.SelectSingleNode("/Yarn/AvailableNodes");
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
        XmlNodeList previousChildren = yarnXML.SelectNodes("//Message");
        if (previousChildren.Count > 0)
            for (int i = previousChildren.Count - 1; i > -1; i--)
            {
                print("Loop 1: " + i);
                previousChildren[i].ParentNode.RemoveChild(previousChildren[i]);
            }
        XPathNavigator currentConversation = yarnXMLnav.SelectSingleNode("/Yarn/Conversations/" + name);
        for (int i = 0; i < messages[0].Count; i++)
        {
            currentConversation.AppendChild("<Message Text=\"" + messages[0][i] + "\" Name=\"" + messages[1][i] + "\"/>");
        }
    }

    public static void SetNewMessageAvailable(string person, string messageNode)
    {
        XPathNavigator allNewMessages = yarnXMLnav.SelectSingleNode("/Yarn/NewMessages");
        foreach (XPathNavigator child in allNewMessages.SelectChildren(XPathNodeType.All))
        {
            if (child.GetAttribute("Name", "") == person)
            {
                child.MoveToAttribute("NewMessage", "");
                child.SetValue("true");
            }
        }
        XPathNavigator nodes = yarnXMLnav.SelectSingleNode("/Yarn/AvailableNode");
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
        XPathNavigator allNewMessages = yarnXMLnav.SelectSingleNode("/Yarn/NewMessages");
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
        XPathNavigator allNewMessages = yarnXMLnav.SelectSingleNode("/Yarn/NewMessages");
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
        XPathNavigator nodesNav = yarnXMLnav.SelectSingleNode("/Yarn/VisitedNodes/" + name);
        foreach (string node in nodes)
        {
            nodesNav.AppendChild("<Node Name =\"" + node + "\"/>");
        }
    }

    static List<string> RetrieveYarnNodes(string name)
    {
        List<string> nodes = new List<string>();
        XPathNavigator nodesNav = yarnXMLnav.SelectSingleNode("/Yarn/VisitedNodes/" + name);
        XPathNodeIterator iterator = nodesNav.SelectChildren(XPathNodeType.All);
        foreach (XPathNavigator node in iterator)
            nodes.Add(node.GetAttribute("Name", ""));
        return nodes;
    }
}