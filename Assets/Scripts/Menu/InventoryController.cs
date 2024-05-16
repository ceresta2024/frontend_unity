using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ceresta
{
    public class InventoryController : MonoBehaviour
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
        public GameObject inventoryContent;
        public ShopModalController modalController;

        void OnEnable()
        {
            RefreshItems();
        }

        public void OnKeywordSubmit(string keyword)
        {
            StartCoroutine(GetInventoryItems(keyword));
        }

        private IEnumerator GetInventoryItems(string keyword = "")
        {
            return WebRequestHandler.Get<ShopResponse>($"/shop/get_inventory_list/?keyword={keyword}", OnGetInventorySuccess);
        }

        private void OnGetInventorySuccess(ShopResponse res)
        {
            inventoryContent.transform.ClearChildren();
            foreach (var itemData in res.data)
            {
                var prefab = Resources.Load<GameObject>("ShopItem");
                var itemObject = GameObject.Instantiate(prefab, inventoryContent.transform);
                itemObject.SetActive(true);
                var item = itemObject.GetComponent<Item>();
                item.Initialize(itemData.item_id, itemData.name, itemData.price, itemData.quantity);

                item.button.onClick.AddListener(() =>
                {
                    modalController.ShowToSell(itemData.item_id, itemData.quantity);
                });
            }
        }

        public void RefreshItems()
        {
            StartCoroutine(GetInventoryItems(searchInput.text));
        }
    }

}