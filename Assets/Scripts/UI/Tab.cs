using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    public Image image;
    public Sprite sprite;
    public Sprite spriteSelected;
    public GameObject tabPanel;

    public void OnClicked(bool selected)
    {
        if (selected) image.sprite = spriteSelected;
        else image.sprite = sprite;
        tabPanel.SetActive(selected);
    }
}
