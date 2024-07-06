using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ceresta
{
    public class SalesController : MonoBehaviour
    {
        [Serializable]
        class ItemData
        {
            public int id;
            public string name;
            public int item_id;
            public int price;
            public int quantity;
        }

        [Serializable]
        class ShopResponse
        {
            [SerializeField]
            public ItemData[] data;
        }

        public TMP_InputField searchInput;
        public GameObject salesContent;
        public ShopModalController modalController;

        void OnEnable()
        {
            RefreshItems();
        }

        public void OnKeywordSubmit(string keyword)
        {
            StartCoroutine(GetShopItems(keyword));
        }

        private IEnumerator GetShopItems(string keyword = "")
        {
            return WebRequestHandler.Get<ShopResponse>($"/shop/get_store_list/?keyword={keyword}", OnSaleItemsSuccess);
        }

        private void OnSaleItemsSuccess(ShopResponse res)
        {
            salesContent.transform.ClearChildren();
            foreach (var itemData in res.data)
            {
                var prefab = Resources.Load<GameObject>("ShopItem");
                var itemObject = GameObject.Instantiate(prefab, salesContent.transform);
                var item = itemObject.GetComponent<Item>();
                item.Initialize(itemData.item_id, itemData.name, itemData.price, itemData.quantity);

                item.button.onClick.AddListener(() =>
                {
                    modalController.ShowToBuy(itemData.item_id, itemData.name, itemData.price, itemData.quantity);
                });
            }
        }

        public void RefreshItems()
        {
            StartCoroutine(GetShopItems(searchInput.text));
        }
    }

}