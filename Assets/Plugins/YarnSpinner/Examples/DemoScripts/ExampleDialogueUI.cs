/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Yarn.Unity.Example
{
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    /** Note that this is just one way of presenting the
     * dialogue to the user. The only hard requirement
     * is that you provide the RunLine, RunOptions, RunCommand
     * and DialogueComplete coroutines; what they do is up to you.
     */
    public class ExampleDialogueUI : Yarn.Unity.DialogueUIBehaviour
    {
        public static UnityEvent onNewNode = new UnityEvent();

        [SerializeField]
        GameObject messageInput, closeChatButton, frame, chatsMenu, aiBubble, playerBubble, scrollRect;
        
        [SerializeField]
        AudioClip[] clickSounds;

        public delegate void SaveDialogueMethod(List<string>[] messages, string name, List<string> nodes);

        GameObject currentAIBubble, currentPlayerBubble;

        SaveDialogueMethod saveMethod;

        public UnityEvent dialogueStarted;

        string currentName;

        List<string> visitedNodes;

        List<string>[] currentConversation = new List<string>[2];

        List<GameObject> chatBubbles;

        public List<string>[] CurrentConversation
        {
            set { this.currentConversation = value; }
        }
        
        /// The object that contains the dialogue and the options.
        /** This object will be enabled when conversation starts, and 
         * disabled when it ends.
         */

        /// The UI element that displays lines
        public Text lineText;

        /// A UI element that appears after lines have finished appearing
        public GameObject continuePrompt;

        /// A delegate (ie a function-stored-in-a-variable) that
        /// we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;

        /// The buttons that let the user choose an option
        public List<Button> optionButtons;

        /// Make it possible to temporarily disable the controls when
        /// dialogue is active and to restore them when dialogue ends
        public RectTransform gameControlsContainer;

        public string CurrentName
        {
            get { return this.currentName; }
            set { this.currentName = value; }
        }

        public SaveDialogueMethod SaveMethod
        {
            set { this.saveMethod = value; }
        }

        public List<string> VisitedNodes
        {
            get { return this.visitedNodes; }
        }

        void Awake()
        {
            // Start by hiding the container, line and option buttons

            messageInput.gameObject.SetActive(false);

            //lineText.gameObject.SetActive(false);

            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }

            // Hide the continue prompt if it exists
            if (continuePrompt != null)
                continuePrompt.SetActive(false);

            onNewNode.AddListener(NewNode);
            frame.SetActive(false);
        }

        GameObject NewBubble(GameObject bubblePrefab, string name)
        {
            RectTransform rt = scrollRect.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 20f + (rt.childCount + 1) * 70f);
            foreach(GameObject bubble in chatBubbles)
            {
                bubble.transform.position = new Vector3(bubble.transform.position.x, bubble.transform.position.y + 70f, bubble.transform.position.z);
            }
            GameObject newBubble = Instantiate(bubblePrefab, scrollRect.transform, false);
            chatBubbles.Add(newBubble);
            lineText = newBubble.transform.Find("MessageText").GetComponent<Text>();
            newBubble.transform.Find("NameText").GetComponent<Text>().text = name;


            return newBubble;
        }

        void NewNode()
        {
            visitedNodes.Add(GetComponent<DialogueRunner>().currentNodeName);
        }

        /// Show a line of dialogue, gradually
        public override IEnumerator RunLine(Yarn.Line line)
        {
            // Show the text
            currentAIBubble = NewBubble(aiBubble, "test");
            //lineText.gameObject.SetActive(true);

            if (textSpeed > 0.0f)
            {
                // Display the line one character at a time
                var stringBuilder = new StringBuilder();

                foreach (char c in line.text)
                {
                    stringBuilder.Append(c);
                    lineText.text = stringBuilder.ToString();
                    yield return new WaitForSeconds(textSpeed);
                }
            }
            else
            {
                // Display the line immediately if textSpeed == 0
                lineText.text = line.text;
            }
            currentConversation[0].Add(line.text);
            currentConversation[1].Add(currentName);

            // Show the 'press any key' prompt when done, if we have one
            if (continuePrompt != null)
                continuePrompt.SetActive(true);

            // Wait for any user input
            /*
            while (Input.anyKeyDown == false) {
                yield return null;
            }

            // Hide the text and prompt
            lineText.gameObject.SetActive (false);

            if (continuePrompt != null)
                continuePrompt.SetActive (false);
                */
        }

        public override IEnumerator NodeComplete(string nextNode)
        {
            //visitedNodes.Add(GetComponent<DialogueRunner>().currentNodeName);
            return base.NodeComplete(nextNode);
        }

        /// Show a list of options, and wait for the player to make a selection.
        public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                                Yarn.OptionChooser optionChooser)
        {
            messageInput.GetComponentInChildren<Text>().text = "";
            // Do a little bit of safety checking
            if (optionsCollection.options.Count > optionButtons.Count)
            {
                Debug.LogWarning("There are more options to present than there are" +
                                 "buttons to present them in. This will cause problems.");
            }

            // Display each option in a button, and make it visible
            int i = 0;
            foreach (var optionString in optionsCollection.options)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<Text>().text = (i + 1).ToString() + ". " + optionString;
                i++;
            }

            // Record that we're using it
            SetSelectedOption = optionChooser;

            // Wait until the chooser has been used and then removed (see SetOption below)
            while (SetSelectedOption != null)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Input.GetKeyDown((j + 1).ToString()) && optionButtons[j].gameObject.activeInHierarchy)
                    {
                        SetOption(j);
                        //HideOptions();
                    }
                }
                yield return null;
            }
            // Hide all the buttons
            //HideOptions();
        }

        void HideOptions()
        {
            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        /// Called by buttons to make a selection.
        public void SetOption(int selectedOption)
        {
            HideOptions();
            StartCoroutine(TypeAnswer(selectedOption));
        }

        IEnumerator TypeAnswer(int selectedOption)
        {
            string message = "";
            yield return new WaitForSeconds(1);
            for (int i = 3; i < optionButtons[selectedOption].GetComponentInChildren<Text>().text.Length; i++)
            {
                AudioSource clickSoundAS = gameObject.AddComponent<AudioSource>();
                clickSoundAS.clip = clickSounds[Random.Range(0, clickSounds.Length - 1)];
                clickSoundAS.pitch = Random.Range(0.9f, 1f);
                clickSoundAS.Play();
                //yield return new WaitForSeconds(0.03f);
                message += optionButtons[selectedOption].GetComponentInChildren<Text>().text[i];
                messageInput.GetComponentInChildren<Text>().text = message;
                yield return new WaitForSeconds(0.07f);
                StartCoroutine(ClickSoundASDestroyer(clickSoundAS));
            }
            currentConversation[0].Add(message);
            currentConversation[1].Add("Player");
            currentPlayerBubble = NewBubble(playerBubble, "You");
            lineText.text = message;
            // Call the delegate to tell the dialogue system that we've
            // selected an option.
            SetSelectedOption(selectedOption);

            // Now remove the delegate so that the loop in RunOptions will exit
            SetSelectedOption = null;
        }

        IEnumerator ClickSoundASDestroyer(AudioSource AS)
        {
            yield return new WaitForSeconds(1);
            Destroy(AS);
        }

        /// Run an internal command.
        public override IEnumerator RunCommand(Yarn.Command command)
        {
            // "Perform" the command
            Debug.Log("Command: " + command.text);

            yield break;
        }

        public void StartDialogue(string name)
        {
            this.currentName = name;
            chatsMenu.SetActive(false);
            frame.SetActive(true);
            GetComponent<DialogueRunner>().StartDialogue();
        }

        /// Called when the dialogue system has started running.
        public override IEnumerator DialogueStarted()
        {
            chatBubbles = new List<GameObject>();
            messageInput.SetActive(true);
            currentConversation = new List<string>[2];
            for (int i = 0; i < currentConversation.Length; i++)
                currentConversation[i] = new List<string>();
            visitedNodes = new List<string>();
            dialogueStarted.Invoke();
            Debug.Log("Dialogue starting!");

            // Enable the dialogue controls.
            /*
            if (dialogueContainer != null)
                dialogueContainer.SetActive(true);
                */

            
            // Hide the game controls.
            if (gameControlsContainer != null)
            {
                gameControlsContainer.gameObject.SetActive(false);
            }

            yield break;
        }

        /// Called when the dialogue system has finished running.
        public override IEnumerator DialogueComplete()
        {
            Debug.Log("Complete!");

            foreach (GameObject bubble in chatBubbles)
                Destroy(bubble);

            saveMethod(currentConversation, currentName, visitedNodes);

            // Hide the dialogue interface
            /*
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);
                */
            closeChatButton.SetActive(true);

            // Show the game controls.
            if (gameControlsContainer != null)
            {
                gameControlsContainer.gameObject.SetActive(true);
            }

            yield break;
        }

        public void CloseChat()
        {
            closeChatButton.SetActive(false);
            frame.SetActive(false);
            chatsMenu.SetActive(true);
        }
    }
}