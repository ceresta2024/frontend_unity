using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Ceresta
{
    public class Ranking : MonoBehaviour
    {
        public TMP_Text number;
        public TMP_Text username;
        public TMP_Text score;
        public Image image;

        public void Initialize(int no, string name, string job, int score)
        {
            number.text = no.ToString();
            username.text = name;
            this.score.text = score.ToString();
            if (job == "") job = "normal";
            image.sprite = Resources.Load<Sprite>($"Jobs/{job}");
        }
    }
}

