using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

/*Short script to adjust the parent object's RectTransform to allowing scrolling an opened email.*/

public class OpenedMailSizer : MonoBehaviour
{
    [SerializeField]
    Scrollbar myBar;

    Text messageText;

    static string mailContents;

    float originalYDelta;

    public static string MailContents
    {
        set { mailContents = value; }
    }

    // Use this for initialization
    void Awake()
    {
        myBar.gameObject.SetActive(true);
        transform.parent.GetComponent<ScrollRect>().verticalScrollbar = myBar;
        originalYDelta = GetComponent<RectTransform>().sizeDelta.y;
        if (messageText == null)
            messageText = GetComponent<Text>();
        messageText.text = mailContents;
        //rt.GetComponent<ScrollRect>().content = GetComponent<RectTransform>();
        StartCoroutine("BugFix");
    }

    IEnumerator BugFix()
    {
        yield return null;
        print(messageText.cachedTextGenerator.lineCount);
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 20f + (messageText.cachedTextGenerator.lineCount + 1) * 25f);
    }

    private void OnDisable()
    {
        myBar.gameObject.SetActive(false);
        messageText.text = "";
        messageText = null;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, originalYDelta);
    }
}
