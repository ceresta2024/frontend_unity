using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ceresta
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        public CountdownTimer timer;

        [Header("Pages")]
        public GameObject lobbyPage;
        public GameObject roomPage;

        [Header("Player List")]
        public GameObject playerListContent;
        public TMP_Text totalPlayers;
        public Dictionary<string, Player> players;

        [Header("Spinner")]
        public GameObject loadingSpinner;

        public override void OnJoinedRoom()
        {
            SetLoading(false);
            timer.enabled = true;
            totalPlayers.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / 20";
            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

            playerListContent.transform.DetachChildren();
            players = new Dictionary<string, Player>();
            foreach (var p in PhotonNetwork.PlayerList)
            {
                var prefab = Resources.Load<GameObject>("PlayerListItem");
                var playerObject = GameObject.Instantiate(prefab, playerListContent.transform);
                var player = playerObject.GetComponent<Player>();
                if (p.CustomProperties.TryGetValue("Job", out object job) && p.CustomProperties.TryGetValue("Score", out object score))
                {
                    player.Initialize(p.NickName, (string)job, (int)score);
                    players.Add(p.NickName, player);
                }
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join room: " + message);
        }

        public void OnBackToLobbyClicked()
        {
            SetLoading(true);
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
            timer.enabled = false;
            lobbyPage.SetActive(true);
            roomPage.SetActive(false);
            SetLoading(false);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            totalPlayers.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / 20";

            var prefab = Resources.Load<GameObject>("PlayerListItem");
            var playerObject = GameObject.Instantiate(prefab, playerListContent.transform);
            var player = playerObject.GetComponent<Player>();

            if (newPlayer.CustomProperties.TryGetValue("Job", out object job) && newPlayer.CustomProperties.TryGetValue("Score", out object score))
            {
                player.Initialize(newPlayer.NickName, (string)job, (int)score);
                players.Add(newPlayer.NickName, player);
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            totalPlayers.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / 20";

            Destroy(players[otherPlayer.NickName].gameObject);
            players.Remove(otherPlayer.NickName);
        }

        private void SetLoading(bool loading)
        {
            loadingSpinner.SetActive(loading);
        }

        private void OnCountdownTimerIsExpired()
        {
            StartCoroutine(AddUserToServer(PhotonNetwork.CurrentRoom.Name));
        }

        private IEnumerator AddUserToServer(string roomName)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = $"{Config.baseUrl}/game/add_user/?room_id={roomName}";
            var request = UnityWebRequest.Post(url, "", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                Debug.Log(responseText);

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                PhotonNetwork.LoadLevel("GameScene");
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}
