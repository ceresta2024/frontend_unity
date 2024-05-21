using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ceresta
{
    public class ProfileController : MonoBehaviour
    {
        public TMP_Text username;
        public TMP_Text gold;
        public TMP_Text jobText;
        public Image avatar;

        void Awake()
        {
            username.text = PlayerPrefs.GetString("Username");
            gold.text = PlayerPrefs.GetInt("Gold").ToString();
            jobText.text = PlayerPrefs.GetString("Job");
        }

        public void OnLogoutClicked()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("AuthUI");
        }
    }
}
