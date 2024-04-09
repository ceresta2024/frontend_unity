using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Networking;

public class GameUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject gameUI;
    public GameObject loadingUI;
    //public GameObject playerprefab;
    string getTimeUrl = "https://backend-test-k12i.onrender.com/maze/get_starttime/";

    void Start()
    {
        //StartCoroutine(GetServerTime(getTimeUrl));
        StartCoroutine(ShowLoadingUI());        
        
        //PhotonNetwork.Instantiate(this.playerprefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
    }

    void Update()
    {
        gameUI.transform.localScale = new Vector3(Screen.width / 720f, Screen.height / 1440f, 1f);
    }

    public void BackBtnClick()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MenuScene");
    }

    IEnumerator ShowLoadingUI()
    {
        loadingUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        loadingUI.SetActive(false);
    }

    public void CloseRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SyncLeaveRoom(new object[] { 1 });
        }
    }

    public void SyncLeaveRoom(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 1)
        {
            object[] infos = (object[])photonEvent.CustomData;

            PhotonNetwork.Disconnect();
        }        
    }

    IEnumerator GetServerTime(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);   
        }
    }
}
