using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceresta
{
    [Serializable]
    public class RequiredItem : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text priceText;
        public Image image;
    }
}