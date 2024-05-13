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
            mainCamera = Camera.main;
            InitMap();
            SpawnPlayer();
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

        private void InitMap()
        {
            var position = Vector2.zero;
            var rotation = Quaternion.Euler(Vector2.zero);
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Map", out object mapId))
            {
                PhotonNetwork.InstantiateRoomObject($"Maps/{(int)mapId}", position, rotation);
            }
        }

        // public override void

        private void SpawnPlayer()
        {
            var startObject = GameObject.Find("Start");
            Debug.Log(startObject);
            // var rotation = Quaternion.Euler(Vector2.zero);
            // player = PhotonNetwork.Instantiate("Player", startObject.transform.position, rotation);
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
    }
}
