using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class MainController : MonoBehaviourPunCallbacks
    {
        [Serializable]
        class RoomData
        {
            public string room_id;
            public int map_id;
        }

        [Serializable]
        class RoomListResponse
        {
            [SerializeField]
            public RoomData[] data;
            [SerializeField]
            public string start_time;
        }

        [Header("Pages")]
        public GameObject startPage;
        public GameObject lobbyPage;
        public GameObject roomPage;

        [Header("Spinner")]
        public GameObject loadingSpinner;

        [Header("User info")]
        public TMP_Text usernameText;
        public TMP_Text goldText;
        public TMP_Text scoreText;

        [Header("Game info")]
        public TMP_Text timerInStart;
        public TMP_Text timerInLobby;
        private DateTime startTime;
        public Button playButton;

        [Header("Room List")]
        public GameObject roomListContent;
        private Dictionary<string, Room> rooms;

        void Awake()
        {
            usernameText.text = PlayerPrefs.GetString("Username");
            goldText.text = PlayerPrefs.GetInt("Gold").ToString();
            scoreText.text = PlayerPrefs.GetInt("Score").ToString();

            rooms ??= new Dictionary<string, Room>();
            StartCoroutine(GetStartTimeAndRoomList());
        }

        void Update()
        {
            var timeSpan = startTime.Subtract(DateTime.Now);
            if (timeSpan.Ticks > 0)
            {
                var timeRemaining = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
                timerInStart.text = timeRemaining;
                timerInLobby.text = timeRemaining;
            }
        }

        public void OnPlayClicked()
        {
            var playerName = PlayerPrefs.GetString("Username");
            PhotonNetwork.LocalPlayer.NickName = playerName;
            SetLoading(true);
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.InLobby)
            {
                SetLoading(true);
                PhotonNetwork.JoinLobby();
            }

            SetActivePage("LobbyPage");
        }

        public override void OnJoinedLobby()
        {
            SetLoading(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SetActivePage("StartPage");
        }

        public void OnBackToHomeClicked()
        {
            PhotonNetwork.Disconnect();
            SetActivePage("StartPage");
        }

        private void SetActivePage(string activePage)
        {
            startPage.SetActive(activePage.Equals(startPage.name));
            lobbyPage.SetActive(activePage.Equals(lobbyPage.name));
            roomPage.SetActive(activePage.Equals(roomPage.name));
        }

        private void SetLoading(bool loading)
        {
            loadingSpinner.SetActive(loading);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var roomInfo in roomList)
            {
                if (rooms[roomInfo.Name].infoText) rooms[roomInfo.Name].infoText.text = $"{roomInfo.PlayerCount} / 20";
            }
        }

        private IEnumerator GetStartTimeAndRoomList()
        {
            return WebRequestHandler.Post<RoomListResponse>("/game/get_roomlist/", "", OnWebSuccess, null, loadingSpinner);
        }

        private void OnWebSuccess(RoomListResponse res)
        {
            startTime = DateTime.ParseExact(res.start_time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToLocalTime();
            foreach (var roomData in res.data)
            {
                var prefab = Resources.Load<GameObject>("Room");
                var roomObject = GameObject.Instantiate(prefab, roomListContent.transform);
                var room = roomObject.GetComponent<Room>();
                room.Initialize(roomData.room_id, roomData.map_id, startTime);
                room.button.onClick.AddListener(() =>
                {
                    SetActivePage("RoomPage");
                    SetLoading(true);
                });

                if (!rooms.ContainsKey(roomData.room_id)) rooms.Add(roomData.room_id, room);
            }
        }
    }
}
