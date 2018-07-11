using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*By Björn Andersson*/


    /*Short script that allows the player to drag UI-elements around.*/

public class DraggableWindowScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerClickHandler
{
    float xOffset, yOffset;

    Vector3 mousePos;

    public void OnPointerClick(PointerEventData pointerEvent)
    {
        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData pointerEvent)
    {
        transform.SetAsLastSibling();
        mousePos = Input.mousePosition;
        xOffset =  mousePos.x - this.transform.position.x;
        yOffset =  mousePos.y - this.transform.position.y;
    }

    public void OnDrag(PointerEventData pointerEvent)
    {
        mousePos = Input.mousePosition;
        this.transform.position = new Vector3(mousePos.x - xOffset, mousePos.y - yOffset, 0f);
    }
}