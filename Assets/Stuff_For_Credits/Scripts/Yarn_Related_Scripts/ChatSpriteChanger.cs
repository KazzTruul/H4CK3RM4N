using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

/*By Björn Andersson*/

/*A short script that changes the sprite of the person the player is chatting with.*/

public class ChatSpriteChanger : MonoBehaviour
{    
    Sprite[][] characterSprites = new Sprite[4][];

    Image chatImage;

    string suffix = "_Sprite_Sheet";

    private void Start()
    {
        chatImage = GetComponent<Image>();
        characterSprites[0] = Resources.LoadAll<Sprite>("Max" + suffix);
        characterSprites[1] = Resources.LoadAll<Sprite>("Hacker" + suffix);
        characterSprites[2] = Resources.LoadAll<Sprite>("Agent" + suffix);
        characterSprites[3] = Resources.LoadAll<Sprite>("Dude" + suffix);
    }

    [YarnCommand("ChangeSprite")]
    public void ChangeSprite(string newSprite)
    {
        foreach (Sprite[] spriteSheet in characterSprites)
        {
            foreach (Sprite sprite in spriteSheet)
                if (sprite.name == newSprite)
                    chatImage.sprite = sprite;
            return;
        }
        print(newSprite + " no existo");
    }
}
