using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteSwap : MonoBehaviour
{
    public Image image;
    public Sprite onSprite;
    public Sprite offSprite;

    public void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            image.sprite = onSprite;
        }
        else
        {
            image.sprite = offSprite;
        }
    }
}
