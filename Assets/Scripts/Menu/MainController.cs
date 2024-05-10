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
        }

        [Header("Pages")]
        public GameObject startPage;
        public GameObject lobbyPage;
        public GameObject roomPage;

        [Header("Spinner")]
        public GameObject loadingSpinner;

        [Header("User info")]
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

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            goldText.text = PlayerPrefs.GetInt("Gold").ToString();
            scoreText.text = PlayerPrefs.GetInt("Score").ToString();

            rooms ??= new Dictionary<string, Room>();
            SetLoading(true);
            StartCoroutine(GetStartTime());
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
                PhotonNetwork.JoinLobby();
                SetLoading(true);
            }

            SetActivePage("LobbyPage");
            StartCoroutine(GetRoomList());
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

        private IEnumerator GetRoomList()
        {
            var url = Config.baseUrl + "/game/get_roomlist/";
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var request = UnityWebRequest.Post(url, "", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<RoomListResponse>(responseText);
                foreach (var roomData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("Room");
                    var roomObject = GameObject.Instantiate(prefab, roomListContent.transform);
                    var room = roomObject.GetComponent<Room>();
                    room.Initialize(roomData.room_id, roomData.map_id, startTime);
                    room.button.onClick.AddListener(() => {
                        SetActivePage("RoomPage");
                        SetLoading(true);
                    });

                    if (!rooms.ContainsKey(roomData.room_id)) rooms.Add(roomData.room_id, room);
                }
                SetLoading(false);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
                SetLoading(false);
            }
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

        private IEnumerator GetStartTime()
        {
            var url = Config.baseUrl + "/game/get_starttime/";
            var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var dateTimeText = request.downloadHandler.text;
                dateTimeText = dateTimeText.Replace("\"", "");
                startTime = DateTime.ParseExact(dateTimeText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToLocalTime();

                SetLoading(false);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}
