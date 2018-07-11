using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

/*Short script to add a blinking dash in the code window's input field.*/
public class DashBlinker : MonoBehaviour
{
    [SerializeField]
    float blinkTime;

    Text placeholder;

    private void Awake()
    {
        placeholder = GetComponent<Text>();
        StartCoroutine("BlinkDash");
    }

    IEnumerator BlinkDash()
    {
        yield return new WaitForSeconds(blinkTime);
        string newPlaceholder = placeholder.text;
        if (newPlaceholder[newPlaceholder.Length - 1] == '_')
            newPlaceholder = newPlaceholder.Remove(newPlaceholder.Length - 1);
        else
            newPlaceholder += '_';
        placeholder.text = newPlaceholder;
        StartCoroutine("BlinkDash");
    }
}
