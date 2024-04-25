using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class LoginController : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;

    public GameObject spinner;

    public GameObject notificationPanel;
    public TMP_Text notification;

    class LoginRequest
    {
        public string email;
        public string password;
    }

    class LoginResponse
    {
        public int id;
        public string name;
        public string email;
        public int gold;
        public int score;
        public int job_id;
        public int status;
        public string access_token;
    }

    // Start is called before the first frame update
    void Start()
    {
        var accessToken = PlayerPrefs.GetString("AccessToken");
        if (!accessToken.IsNullOrEmpty())
        {
            StartCoroutine(CheckIfLoggedIn(accessToken));
            spinner.SetActive(true);
        }
    }

    public void LoginWithCredential()
    {
        StartCoroutine(LoginWithCredential(username.text, password.text));
        spinner.SetActive(true);
    }

    IEnumerator LoginWithCredential(string username, string password)
    {
        var body = new LoginRequest
        {
            email = username,
            password = password
        };
        var url = Config.baseUrl + "/user/login/";
        var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseText = request.downloadHandler.text;
            var res = JsonUtility.FromJson<LoginResponse>(responseText);

            PlayerPrefs.SetString("AccessToken", res.access_token);
            PlayerPrefs.SetString("Gold", res.gold.ToString());
            PlayerPrefs.SetString("Username", res.name);
            PlayerPrefs.SetString("Score", res.score.ToString());

            notification.text = "You have successfully logged in.";
            notificationPanel.SetActive(true);
            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            Debug.Log("Error: " + request.downloadHandler.text);
            spinner.SetActive(false);
        }
    }

    IEnumerator CheckIfLoggedIn(string accessToken)
    {
        var url = Config.baseUrl + "/user/getinfo";
        var request = UnityWebRequest.Post(url, "", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseText = request.downloadHandler.text;
            var res = JsonUtility.FromJson<LoginResponse>(responseText);

            PlayerPrefs.SetString("Gold", res.gold.ToString());
            PlayerPrefs.SetString("Username", res.name);
            PlayerPrefs.SetString("Score", res.score.ToString());

            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            Debug.Log("Error: " + request.downloadHandler.text);
            PlayerPrefs.DeleteKey("AccessToken");
            spinner.SetActive(false);
        }
    }
}
