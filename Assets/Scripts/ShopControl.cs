using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopControl : MonoBehaviour
{
    public GameObject contentParent;

    [Header("Shop Ref")]
    public Transform templateShopParent;
    public GameObject buttonShopTemplate;

    [Header("Inventory Ref")]
    public Transform templateInventoryParent;
    public GameObject buttonInventoryTemplate;

    public static ShopControl instance;
    public int lastSelectedItemID;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }
    private void OnEnable()
    {
        GetShopList();
    }
    public void GetInventoryList()
    {
        ShopManager.Instance.GetInventoryList();
    }
    public void GetShopList()
    {
        ShopManager.Instance.GetStoreList();
    }

    public void BuyFromShop()
    {
        ShopManager.Instance.BuyItem(UIController.Instance.selectedShopItemID, UIController.Instance.selectedShopItemQuantity); //Buys Tomato Soup
    }

    public void SellFromInventory()
    {
        ShopManager.Instance.SellItem();
    }

    public void LoadShopList()
    {
        templateShopParent = buttonShopTemplate.transform.parent;
        foreach (Transform obj in buttonShopTemplate.transform.parent)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                Destroy(obj.gameObject);
            }
            Debug.Log(":Des");
        }
        if (buttonShopTemplate != null)
        {
            templateShopParent = buttonShopTemplate.transform.parent;

            Debug.Log("::");

            foreach (var item in ShopManager.Instance.allShopItems)
            {
                GameObject obj = Instantiate(buttonShopTemplate, buttonShopTemplate.transform.parent);
                obj.GetComponent<ShopTemplate>().refShopItem = item;
                obj.GetComponent<ShopTemplate>().PopulateDetails();
                obj.SetActive(true);

                Debug.Log(":::");
            }


        }
    }

    public void LoadInventoryList()
    {
        templateInventoryParent = buttonInventoryTemplate.transform.parent;
        foreach (Transform obj in buttonInventoryTemplate.transform.parent)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                Destroy(obj.gameObject);
            }
            Debug.Log(":Des");
        }
        if (buttonInventoryTemplate != null)
        {
            templateInventoryParent = buttonInventoryTemplate.transform.parent;

            Debug.Log("::");

            foreach (var item in ShopManager.Instance.allShopItems)
            {
                GameObject obj = Instantiate(buttonInventoryTemplate, templateInventoryParent);
                obj.GetComponent<ShopTemplate>().refShopItem = item;
                obj.GetComponent<ShopTemplate>().PopulateDetails();
                obj.SetActive(true);


            }


        }
    }
}

