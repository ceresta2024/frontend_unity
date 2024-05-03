using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Ceresta
{
    public class ProfileController : MonoBehaviour
    {
        public GameObject loadingSpinner;

        public void OnLogoutClicked()
        {
            StartCoroutine(Logout());
            loadingSpinner.SetActive(true);
        }

        IEnumerator Logout()
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
