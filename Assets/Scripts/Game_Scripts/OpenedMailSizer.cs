using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

    /*Short script to adjust the parent object's RectTransform to allowing scrolling an opened email.*/

public class OpenedMailSizer : MonoBehaviour
{
    [SerializeField]
    RectTransform rt;

    [SerializeField]
    Text mailText;

    static string mailContents;

    public static string MailContents
    {
        set { mailContents = value; }
    }

    // Use this for initialization
    void Awake () {
        mailText.text = mailContents;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 20f + (rt.childCount + 1) * 20f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
