using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoomController : MonoBehaviourPunCallbacks
{
    public GameObject roomPage;
    public GameObject lobbyPage;

    public void OnBackClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPage.SetActive(false);
        lobbyPage.SetActive(true);
    }
}
