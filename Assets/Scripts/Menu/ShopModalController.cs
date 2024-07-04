using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class ShopModalController : MonoBehaviour
    {
        class ShopRequest
        {
            public int item_id;
            public int quantity;
        }

        class ShopResponse
        {
            public string message;
            public int gold;
        }

        public GameObject loadingSpinner;
        public TMP_Text itemName;
        public TMP_Text price;
        public QtyInput qtyInput;
        public Image image;
        public SalesController salesController;
        public InventoryController inventoryController;
        public TMP_Text goldText;


        private int itemId;
        private int qty;
        private bool owned;

        public void ShowToBuy(int itemId, string itemName, int price, int maxQty)
        {
            this.itemId = itemId;
            this.itemName.text = itemName;
            this.price.text = price.ToString();
            var sp = Resources.Load<Sprite>($"Items/{itemId}");
            image.sprite = sp;
            owned = false;
            qtyInput.maxQty = maxQty;
            gameObject.SetActive(true);
        }

        public void ShowToSell(int itemId, int maxQty)
        {
            this.itemId = itemId;
            var sp = Resources.Load<Sprite>($"Items/{itemId}");
            image.sprite = sp;
            owned = true;
            qtyInput.maxQty = maxQty;
            gameObject.SetActive(true);
        }

        public void OnConfirmClicked()
        {
            if (owned)
            {
                Debug.Log("Selling Item Id: " + itemId + ", Qty: " + qty);
                StartCoroutine(SellItem(itemId, qty));
            }
            else
            {
                Debug.Log("Buying Item Id: " + itemId + ", Qty: " + qty);
                StartCoroutine(BuyItem(itemId, qty));
            }
        }

        public void SetQty(string inputValue)
        {
            qty = int.Parse(inputValue);
        }

        private IEnumerator BuyItem(int itemId, int qty)
        {
            var body = new ShopRequest
            {
                item_id = itemId,
                quantity = qty,
            };
            return WebRequestHandler.Post<ShopResponse>("/shop/buy_item/", body, OnBuySuccess, null, loadingSpinner);
        }

        private IEnumerator SellItem(int itemId, int qty)
        {
            var body = new ShopRequest
            {
                item_id = itemId,
                quantity = qty,
            };
            return WebRequestHandler.Post<ShopResponse>("/shop/sell_item/", body, OnSellSuccess, null, loadingSpinner);
        }

        private void OnBuySuccess(ShopResponse res)
        {
            salesController.RefreshItems();
            PlayerPrefs.SetInt("Gold", res.gold);
            goldText.text = res.gold.ToString();
            gameObject.SetActive(false);
        }

        private void OnSellSuccess(ShopResponse res)
        {
            inventoryController.RefreshItems();
            PlayerPrefs.SetInt("Gold", res.gold);
            goldText.text = res.gold.ToString();
            gameObject.SetActive(false);
        }
    }
}