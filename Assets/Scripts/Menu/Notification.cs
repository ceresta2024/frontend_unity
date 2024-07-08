using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Ceresta
{
    public class Notification : MonoBehaviour
    {
        public TMP_Text noti_name;
        public TMP_Text noti_content;
        public TMP_Text noti_start_date;
        public TMP_Text end_date;

        public void Initialize(string name, string content, string start_date)
        {
            noti_name.text = name;
            noti_content.text = content;
            noti_start_date.text = start_date;
        }
    }
}

