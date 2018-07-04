using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

/*Short script to adjust the size of parent object's RectTransform to allow scrolling in the inbox.*/

public class InboxSizer : MonoBehaviour
{
    [SerializeField]
    RectTransform rt;

    [SerializeField]
    GameObject inboxMail;

    [SerializeField]
    Sprite openedMailSprite;

    List<GameObject> inbox;

    void Awake()
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (this.transform.childCount) * 100f);
        rt.GetComponent<ScrollRect>().content = GetComponent<RectTransform>();
        inbox = new List<GameObject>();
        StartCoroutine("SpawnAllInboxMail");
    }

    IEnumerator SpawnAllInboxMail()
    {
        foreach (string[] mailTexts in XMLManager.GetInbox())
        {
            SpawnInboxMail(mailTexts[0], mailTexts[1], mailTexts[2], mailTexts[3]);
            yield return null;
        }
    }

    void SpawnInboxMail(string sender, string receiver, string subject, string read)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 10f + (inbox.Count + 1) * 100f);
        foreach (GameObject oldMail in inbox)
            oldMail.transform.position = new Vector3(oldMail.transform.position.x, oldMail.transform.position.y - 100f, oldMail.transform.position.z);
        GameObject mail = Instantiate(inboxMail, this.transform);
        Transform iconTransform = mail.transform.Find("MailStatusIcon");
        if (read == "True")
            iconTransform.GetComponent<Image>().sprite = openedMailSprite;
        Transform senderTextTransform = iconTransform.transform.Find("SenderText");
        senderTextTransform.GetComponent<Text>().text = sender;
        Transform receiverTextTransform = senderTextTransform.Find("ReceiverText");
        receiverTextTransform.GetComponent<Text>().text = receiver;
        Transform subjectTextTransform = receiverTextTransform.Find("SubjectText");
        subjectTextTransform.GetComponent<Text>().text = subject;
        mail.GetComponent<Button>().onClick.AddListener(() => XMLManager.SetMailContents(subject));
        inbox.Add(mail);
    }

    private void OnDisable()
    {
        foreach (GameObject mail in inbox)
            Destroy(mail);
        inbox = null;
    }
}