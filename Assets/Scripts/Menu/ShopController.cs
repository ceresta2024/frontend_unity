using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class ShopController : MonoBehaviour
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
        public TMP_Text goldText;
        public Transform shopContent;
        public Transform inventoryContent;
        public GameObject shopModal;

        void Start()
        {
            StartCoroutine(GetShopItems());
            StartCoroutine(GetInventoryItems());
            goldText.text = PlayerPrefs.GetString("Gold");
        }

        public void OnKeywordSubmit(string keyword)
        {
            StartCoroutine(GetShopItems(keyword));
            StartCoroutine(GetInventoryItems(keyword));
        }

        private IEnumerator GetShopItems(string keyword = "")
        {
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var url = $"{Config.baseUrl}/shop/get_store_list/?keyword={keyword}";
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                shopContent.DetachChildren();
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<ShopResponse>(responseText);
                foreach (var itemData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("ShopItem");
                    var itemObject = GameObject.Instantiate(prefab, shopContent);
                    var item = itemObject.GetComponent<Item>();
                    item.itemName.text = itemData.name;
                    item.price.text = itemData.price.ToString();
                    item.quantity.text = itemData.quantity.ToString();
                    var sp = Resources.Load<Sprite>($"Items/{itemData.item_id}");
                    item.image.sprite = sp;
                    item.SetItemId(itemData.item_id);
                    item.SetShopModal(shopModal);
                }
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        private IEnumerator GetInventoryItems(string keyword = "")
        {
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var url = $"{Config.baseUrl}/shop/get_inventory_list/?keyword={keyword}";
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                inventoryContent.DetachChildren();
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<ShopResponse>(responseText);
                foreach (var itemData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("ShopItem");
                    var itemObject = GameObject.Instantiate(prefab, inventoryContent);
                    itemObject.SetActive(true);
                    var item = itemObject.GetComponent<Item>();
                    item.itemName.text = itemData.name;
                    item.price.text = itemData.price.ToString();
                    item.quantity.text = itemData.quantity.ToString();
                    var sp = Resources.Load<Sprite>($"Items/{itemData.item_id}");
                    item.image.sprite = sp;
                    item.SetItemId(itemData.item_id);
                    item.SetShopModal(shopModal);
                    item.SetOwned(true);
                }
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}

