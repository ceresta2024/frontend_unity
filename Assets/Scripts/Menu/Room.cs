using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ceresta
{
    public class Room : MonoBehaviourPunCallbacks
    {
        public string roomId;
        public int mapId;
        public int maxPlayers = 20;
        public TMP_Text infoText;
        public GameObject loadingSpinner;

        public void OnClicked()
        {
            loadingSpinner.SetActive(true);
            var roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayers,
                PublishUserId = true,
            };
            PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);
        }
    }
}
