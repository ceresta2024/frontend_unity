using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class GameUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Serializable]
    public class ItemInfos
    {
        public List<string> name;
        public List<int> itemID;
    }

    public GameObject gameUI;
    public GameObject loadingUI;
    public Text timeText;
    public float timeRemaining;
    //public GameObject playerprefab;
    string getTimeUrl = "https://backend-test-k12i.onrender.com/game/get_starttime/";
    public Text chatText;
    public GameObject chatTextDialog;
    public InputField chatInput;
    public ItemInfos itemInventory;

    void Start()
    {
        //StartCoroutine(GetServerTime(getTimeUrl));
        StartCoroutine(ShowLoadingUI());
        timeRemaining = 602f;
        
        //PhotonNetwork.Instantiate(this.playerprefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
    }

    void Update()
    {
        gameUI.transform.localScale = new Vector3(Screen.width / 720f, Screen.height / 1440f, 1f);

        if(timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            PhotonNetwork.Disconnect();
        }
    }

    public void BackBtnClick()
    {
        if (PhotonNetwork.IsConnected)
        {
            MainPun.isPlaying = false;
            PhotonNetwork.Disconnect();
        }
    }

    public void SendBtnClick()
    {
        string sendString = PhotonNetwork.NickName + ": " + chatInput.text + " " + DateTime.UtcNow;
        chatInput.text = "";
        
        SyncMessage(new object[] { sendString });
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

    public void ShowItem()
    {
        string itemLists = PlayerPrefs.GetString("ITEM_INVENTORY");
        itemInventory = JsonUtility.FromJson<ItemInfos>(itemLists);
    }

    public void HelpBtnClick()
    {
        SyncHelp(new object[] { PlayerManager.instance.playerName });
    }

    public void SyncLeaveRoom(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncMessage(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncHelp(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);
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
            PhotonNetwork.Disconnect();
        }  
        else if(eventCode == 3)
        {
            object[] infos = (object[])photonEvent.CustomData;

            chatText.text = infos[0].ToString();
            StopAllCoroutines();
            StartCoroutine(ShowChatText());
        }
        else if(eventCode == 4)
        {
            if(PlayerManager.instance.jobIndex == 4)
            {

            }
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

    IEnumerator ShowChatText()
    {
        chatTextDialog.SetActive(true);

        yield return new WaitForSeconds(3f);

        chatTextDialog.SetActive(false);
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
