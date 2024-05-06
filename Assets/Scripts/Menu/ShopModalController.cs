using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        public Image image;
        private int itemId;
        private int qty;
        private bool owned;

        public void Show(int itemId)
        {
            var sp = Resources.Load<Sprite>($"Items/{itemId}");
            image.sprite = sp;
            gameObject.SetActive(true);
            this.itemId = itemId;
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

        public void SetOwned(bool owned)
        {
            this.owned = owned;
        }

        private IEnumerator BuyItem(int itemId, int qty)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = Config.baseUrl + "/shop/buy_item/";
            var body = new ShopRequest
            {
                item_id = itemId,
                quantity = qty,
            };
            var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<ShopResponse>(responseText);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        private IEnumerator SellItem(int itemId, int qty)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = Config.baseUrl + "/shop/sell_item/";
            var body = new ShopRequest
            {
                item_id = itemId,
                quantity = qty,
            };
            var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                Debug.Log(responseText);
                var res = JsonUtility.FromJson<ShopResponse>(responseText);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}