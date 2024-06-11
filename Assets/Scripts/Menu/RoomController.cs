using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        class AddUserResponse
        {

        }

        public Button startButton;

        [Header("Pages")]
        public GameObject lobbyPage;
        public GameObject roomPage;

        [Header("Player List")]
        public GameObject playerListContent;
        public TMP_Text totalPlayers;
        public Dictionary<string, PlayerItem> players;

        [Header("Spinner")]
        public GameObject loadingSpinner;

        public override void OnJoinedRoom()
        {
            SetLoading(false);
            startButton.gameObject.SetActive(CheckPlayersReady());
            totalPlayers.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / 20";

            StartCoroutine(AddUserToServer(PhotonNetwork.CurrentRoom.Name));

            playerListContent.transform.ClearChildren();
            players = new Dictionary<string, PlayerItem>();
            foreach (var p in PhotonNetwork.PlayerList)
            {
                var prefab = Resources.Load<GameObject>("PlayerListItem");
                var playerObject = GameObject.Instantiate(prefab, playerListContent.transform);
                var player = playerObject.GetComponent<PlayerItem>();
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
            StartCoroutine(RemoveUserFromServer(PhotonNetwork.CurrentRoom.Name));
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            lobbyPage.SetActive(true);
            roomPage.SetActive(false);
            SetLoading(false);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            totalPlayers.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / 20";

            var prefab = Resources.Load<GameObject>("PlayerListItem");
            var playerObject = GameObject.Instantiate(prefab, playerListContent.transform);
            var player = playerObject.GetComponent<PlayerItem>();

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

        public void OnStartClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("GameScene");
        }

        private void SetLoading(bool loading)
        {
            loadingSpinner.SetActive(loading);
        }

        private IEnumerator AddUserToServer(string roomName)
        {
            return WebRequestHandler.Post<AddUserResponse>($"/game/add_user/?room_id={roomName}", "", null);
        }

        private IEnumerator RemoveUserFromServer(string roomName)
        {
            return WebRequestHandler.Post<AddUserResponse>($"/game/remove_user/?room_id={roomName}", "", null);
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                startButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            return true;
        }
    }
}
