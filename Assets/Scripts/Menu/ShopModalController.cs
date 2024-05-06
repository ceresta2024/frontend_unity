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
        class BuyRequest
        {
            public int item_id;
            public int quantity;
        }

        class BuyResponse
        {
            public string message;
        }

        public Image image;
        private int itemId;
        private int qty;

        public void Show(int itemId)
        {
            var sp = Resources.Load<Sprite>($"Items/{itemId}");
            image.sprite = sp;
            gameObject.SetActive(true);
            this.itemId = itemId;
        }

        public void OnConfirmClicked()
        {
            Debug.Log("Item Id: " + itemId + ", Qty: " + qty);
            StartCoroutine(BuyItem(itemId, qty));
        }

        public void SetQty(string inputValue)
        {
            qty = int.Parse(inputValue);
        }

        private IEnumerator BuyItem(int itemId, int qty)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = Config.baseUrl + "/shop/buy_item/";
            var body = new BuyRequest
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
                var res = JsonUtility.FromJson<BuyResponse>(responseText);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}