using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    public TMP_Text goldText;
    public TMP_Text scoreText;

    public GameObject loadingSpinner;
    public GameObject lobbyPage;
    public GameObject startPage;

    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        goldText.text = PlayerPrefs.GetString("Gold");
        scoreText.text = PlayerPrefs.GetString("Score");
        playerName = PlayerPrefs.GetString("Username");
    }

    public void OnPlayClicked()
    {
        loadingSpinner.SetActive(true);
        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        loadingSpinner.SetActive(false);
        lobbyPage.SetActive(true);
        startPage.SetActive(false);
    }
}
