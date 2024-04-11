using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ShopTemplate : MonoBehaviour
{
    public ShopItem refShopItem;
    public TMP_Text amountText;
    public TMP_Text unitsText;
    // Start is called before the first frame update
    void Start()
    {
       // Invoke(nameof(PopulateDetails), 1f);
    }
    public void PopulateDetails()
    {
        amountText.text = refShopItem.price.ToString();
        unitsText.text = refShopItem.quantity.ToString();
    }
    public void OnClickShopItem()
    {

        UIController.Instance.selectedShopItemID = refShopItem.id;

        UIController.Instance.selectedShopItemQuantity = refShopItem.quantity;
       
    }
}
