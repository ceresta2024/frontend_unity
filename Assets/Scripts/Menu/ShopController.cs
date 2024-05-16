using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Ceresta
{
    public class ShopController : MonoBehaviour
    {
        public TMP_Text goldText;

        void Start()
        {
            goldText.text = PlayerPrefs.GetInt("Gold").ToString();
        }
    }
}

