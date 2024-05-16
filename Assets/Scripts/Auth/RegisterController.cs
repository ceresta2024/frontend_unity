using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ceresta
{
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
        }

        private IEnumerator RegisterWithCredential(string username, string email, string password)
        {
            var body = new RegisterRequest { username = username, email = email, password = password };
            return WebRequestHandler.Post<RegisterResponse>("/user/register/", body, OnRegisterSuccess, spinner);
        }

        private void OnRegisterSuccess(RegisterResponse res)
        {
            notification.text = res.message;
            notificationPanel.SetActive(true);

            loginPage.SetActive(true);
        }
    }
}
