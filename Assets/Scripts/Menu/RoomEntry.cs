using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviourPunCallbacks
{
    public string roomName;
    public int maxPlayers = 20;
    public Text roomInfoText;
    public GameObject loadingSpinner;

    public void OnClicked()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayers
        };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        loadingSpinner.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        loadingSpinner.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room: " + message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            if (roomInfo.Name == roomName)
            {
                roomInfoText.text = roomInfo.PlayerCount.ToString() + " / " + roomInfo.MaxPlayers.ToString();
            }
        }
    }

}
