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
        public GameObject loadingSpinner;
        public TMP_Text username;
        public TMP_Text gold;
        public TMP_Text jobText;
        public Image avatar;

        void Awake()
        {
            username.text = PlayerPrefs.GetString("Username");
            gold.text = PlayerPrefs.GetString("Gold");
        }

        public void OnLogoutClicked()
        {
            StartCoroutine(Logout());
            loadingSpinner.SetActive(true);
        }

        private IEnumerator Logout()
        {
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var url = Config.baseUrl + "/user/logout";
            var request = UnityWebRequest.Post(url, "", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerPrefs.DeleteKey("AccessToken");
                SceneManager.LoadScene("AuthScene");
            }
        }
    }
}
