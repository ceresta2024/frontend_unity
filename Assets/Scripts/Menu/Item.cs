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

        public void OnClicked()
        {
            var modalController = shopModal.GetComponent<ShopModalController>();
            modalController.Show(itemId);
        }

        public void SetItemId(int itemId)
        {
            this.itemId = itemId;
        }

        public void SetShopModal(GameObject shopModal)
        {
            this.shopModal = shopModal;
        }
    }
}

