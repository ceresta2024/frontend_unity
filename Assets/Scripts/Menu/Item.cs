using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceresta
{
    public class Item : MonoBehaviour
    {
        public TMP_Text itemName;
        public TMP_Text price;
        public TMP_Text quantity;
        public Image image;
        private int itemId;
        private GameObject shopModal;
        private bool owned = false;
        public Button button;

        public void Initialize(int item_id, string name, int price, int quantity)
        {
            itemId = item_id;
            itemName.text = name;
            this.price.text = price.ToString();
            this.quantity.text = quantity.ToString();
            var sp = Resources.Load<Sprite>($"Items/{item_id}");
            image.sprite = sp;
        }
    }
}

