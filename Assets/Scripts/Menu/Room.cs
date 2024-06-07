using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceresta
{
    public class Room : MonoBehaviourPunCallbacks
    {
        private string roomId;
        private int mapId;
        private DateTime startTime;
        private int maxPlayers = 20;

        public TMP_Text infoText;
        public Button button;

        public void OnClicked()
        {
            var timestamp = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
            var playerProps = new Hashtable
            {
                {"Job", PlayerPrefs.GetString("Job")},
                {"Score", PlayerPrefs.GetInt("Score")}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

            var props = new Hashtable
            {
                {"Map", mapId},
            };
            var roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayers,
                PublishUserId = true,
                CustomRoomProperties = props
            };
            PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);
        }

        public void Initialize(string roomId, int mapId, DateTime startTime)
        {
            this.roomId = roomId;
            this.mapId = mapId;
            this.startTime = startTime;
        }
    }
}
