using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterController : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField email;
    public TMP_InputField password;

    public GameObject spinner;
    public GameObject notificationPanel;
    public TMP_Text notification;

    public GameObject loginPage;

    class RegisterRequest
    {
        public string username;
        public string email;
        public string password;
    }

    class RegisterResponse
    {
        public string message;
    }

    public void RegisterWithCredential()
    {
        StartCoroutine(RegisterWithCredential(username.text, email.text, password.text));
        spinner.SetActive(true);
    }

    IEnumerator RegisterWithCredential(string username, string email, string password)
    {
        var body = new RegisterRequest { username = username, email = email, password = password };
        var url = Config.baseUrl + "/user/register/";
        var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var res = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
            notification.text = res.message;
            notificationPanel.SetActive(true);
            spinner.SetActive(false);
            loginPage.SetActive(true);
        }
        else
        {
            Debug.Log("Error: " + request.downloadHandler.text);
            spinner.SetActive(false);
        }
    }
}
