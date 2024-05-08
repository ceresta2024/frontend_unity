using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class LobbyController : MonoBehaviourPunCallbacks
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

        public GameObject lobbyPage;
        public GameObject startPage;
        public GameObject roomList;
        public GameObject roomPage;
        public GameObject loadingSpinner;

        private Dictionary<string, Room> rooms;

        void Awake()
        {
            StartCoroutine(GetRoomList());
        }

        public void OnBackClicked()
        {
            lobbyPage.SetActive(false);
            startPage.SetActive(true);
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
                rooms = new Dictionary<string, Room>();
                foreach (var roomData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("Room");
                    var roomObject = GameObject.Instantiate(prefab, roomList.transform);
                    var room = roomObject.GetComponent<Room>();
                    room.loadingSpinner = loadingSpinner;
                    room.roomPage = roomPage;
                    room.roomId = roomData.room_id;
                    room.mapId = roomData.map_id;

                    rooms.Add(roomData.room_id, room);
                }
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var roomInfo in roomList)
            {
                if (rooms[roomInfo.Name].infoText) rooms[roomInfo.Name].infoText.text = $"{roomInfo.PlayerCount} / 20";
            }
        }
    }
}
