using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using Facebook.Unity;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

namespace Ceresta
{
    public class LoginController : MonoBehaviour
    {
        public TMP_InputField email;
        public TMP_InputField password;

        public GameObject spinner;

        public GameObject notificationPanel;
        public TMP_Text notification;

        private string Token;

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
            public string job;
            public int status;
            public string access_token;
        }

        void Awake()
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }

            var config = new PlayGamesClientConfiguration.Builder()
            // Requests an ID token be generated.  
            // This OAuth token can be used to
            // identify the player to other services such as Firebase.
            .RequestIdToken()
            .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
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

        public void OnLoginClicked()
        {
            StartCoroutine(LoginWithCredential(email.text, password.text));
            spinner.SetActive(true);
        }

        IEnumerator LoginWithCredential(string email, string password)
        {
            var body = new LoginRequest
            {
                email = email,
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
                PlayerPrefs.SetInt("Gold", res.gold);
                PlayerPrefs.SetString("Username", res.name);
                PlayerPrefs.SetInt("Score", res.score);
                PlayerPrefs.SetString("Job", res.job);

                notification.text = "You have successfully logged in.";
                notificationPanel.SetActive(true);
                SceneManager.LoadScene("MainUI");
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

                PlayerPrefs.SetInt("Gold", res.gold);
                PlayerPrefs.SetString("Username", res.name);
                PlayerPrefs.SetInt("Score", res.score);
                PlayerPrefs.SetString("Job", res.job);

                SceneManager.LoadScene("MainUI");
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
                PlayerPrefs.DeleteKey("AccessToken");
                spinner.SetActive(false);
            }
        }

        public void OnFacebookButtonClicked()
        {
            // Define the permissions
            var perms = new List<string>() { "public_profile", "email" };

            FB.LogInWithReadPermissions(perms, result =>
            {
                if (FB.IsLoggedIn)
                {
                    Token = AccessToken.CurrentAccessToken.TokenString;
                    Debug.Log($"Facebook Login token: {Token}");
                    // TODO: verify the access token in backend and login
                }
                else
                {
                    Debug.Log("[Facebook Login] User cancelled login");
                }
            });
        }

        public void OnGoogleButtonClicked()
        {
            Social.localUser.Authenticate(OnGoogleLogin);
        }

        private void OnGoogleLogin(bool success)
        {
            if (success)
            {
                // Call Unity Authentication SDK to sign in or link with Google.
                Debug.Log("Login with Google done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
            }
            else
            {
                Debug.Log("Unsuccessful login");
            }
        }
    }
}
