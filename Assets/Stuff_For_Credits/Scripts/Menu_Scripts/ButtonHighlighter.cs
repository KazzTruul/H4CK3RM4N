using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*By Björn Andersson*/

    /*Short script to handle minor button functionality, such as highlighting and audio.*/

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    AudioClip buttonHoverSound, buttonPressSound;

    [SerializeField]
    AudioSource buttonPressAS, buttonHoverAS;

    void Start()
    {
        if (buttonPressSound == null)
            buttonPressSound = Resources.Load("Button_Press_Sound") as AudioClip;
    }
 
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        buttonPressAS.clip = buttonPressSound;
        buttonPressAS.Play();
        GetComponentInChildren<Outline>().enabled = true;
    }

    public void OnPointerExit(PointerEventData pointerEvent)
    {
        GetComponentInChildren<Outline>().enabled = false;
    }
}
