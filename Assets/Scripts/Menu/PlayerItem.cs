using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceresta
{
    public class PlayerItem : MonoBehaviour
    {
        public TMP_Text userId;
        public TMP_Text score;
        public Image image;

        public void Initialize(string userName, string job, int score)
        {
            userId.text = userName;
            this.score.text = score.ToString();
            image.sprite = Resources.Load<Sprite>($"Jobs/{job}");
        }
    }
}