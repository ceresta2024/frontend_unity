using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QtyInput : MonoBehaviour
{
    public int qty = 0;
    public Button minusButton;
    public Button plusButton;
    public TMP_InputField input;
    public int maxQty = int.MaxValue;

    void Start()
    {
        input.text = qty.ToString();
    }

    public void OnMinusClicked()
    {
        if (qty == 0) return;
        qty -= 1;
        input.text = qty.ToString();
    }

    public void OnPlusClicked()
    {
        if (qty == maxQty) return;
        qty += 1;
        input.text = qty.ToString();
    }
}
