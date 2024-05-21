using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Ceresta
{
    public class WebRequestHandler
    {
        public static IEnumerator Get<T>(string url, Action<T> callback, Action errorCallback = null)
        {
            var fullUrl = CerestaGame.baseUrl + url;
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var request = UnityWebRequest.Get(fullUrl);
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            Debug.Log("Sending GET request");
            Debug.Log("Url: " + fullUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);
                var res = JsonUtility.FromJson<T>(responseText);
                callback.Invoke(res);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        public static IEnumerator Post<T>(string url, object data, Action<T> callback, Action errorCallback = null, GameObject loadingSpinner = null)
        {
            var fullUrl = CerestaGame.baseUrl + url;
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var body = JsonUtility.ToJson(data);
            var request = UnityWebRequest.Post(fullUrl, body, "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            Debug.Log("Sending POST request");
            Debug.Log("URL: " + fullUrl);
            Debug.Log("Body: " + body);
            if (loadingSpinner != null) loadingSpinner.SetActive(true);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);
                var res = JsonUtility.FromJson<T>(responseText);
                if (loadingSpinner != null) loadingSpinner.SetActive(false);
                callback.Invoke(res);
            }
            else
            {
                if (loadingSpinner != null) loadingSpinner.SetActive(false);
                Debug.Log("Status Code: " + request.responseCode);
                Debug.Log("Error: " + request.downloadHandler.text);
                if (request.responseCode == 403 && SceneManager.GetActiveScene().name != "AuthUI")
                {
                    PlayerPrefs.DeleteKey("AccessKey");
                    SceneManager.LoadScene("AuthUI");
                }
                else
                {
                    errorCallback?.Invoke();
                }
            }
        }
    }
}