using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ceresta
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        public Weather weather;
        public TMP_Text connectionStatusText;

        private readonly string connectionStatusMessage = "Connection Status: ";

        private GameObject player;
        private Camera mainCamera;
        private Vector3 offset;
        private float damping;
        private Vector3 vel = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            CountdownTimer.SetStartTime();
            Debug.Log(PhotonNetwork.CurrentRoom.Name);
            var position = Vector2.zero;
            var rotation = Quaternion.Euler(Vector2.zero);
            PhotonNetwork.InstantiateRoomObject("Maps/" + PhotonNetwork.CurrentRoom.Name, position, rotation);
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            connectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
            if (player)
            {
                Vector3 targetPosition = player.transform.position + offset;
                targetPosition.z = mainCamera.transform.position.z;

                mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref vel, damping);
            }
        }

        public void OnBackButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MainUI");
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        private void StartGame()
        {
            Debug.Log("Start game");
            var position = GameObject.Find("Start").transform.position;
            var rotation = Quaternion.Euler(Vector2.zero);
            player = PhotonNetwork.Instantiate("Player", position, rotation);
        }

        public IEnumerator EndOfGame()
        {
            float timer = 5.0f;
            while (timer > 0.0f)
            {
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }
            PhotonNetwork.LeaveRoom();
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
    }
}
