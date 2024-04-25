using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPage;
    public GameObject startPage;
    
    public void OnBackClicked()
    {
        PhotonNetwork.Disconnect();
        lobbyPage.SetActive(false);
        startPage.SetActive(true);
    }
}
