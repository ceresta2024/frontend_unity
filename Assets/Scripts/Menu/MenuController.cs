using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviourPunCallbacks
{
    public TMP_Text goldText;
    public TMP_Text scoreText;

    public GameObject loadingSpinner;
    public GameObject lobbyPage;
    public GameObject startPage;

    [Header("Room List Panel")]
    public GameObject roomListContent;
    public GameObject roomListEntryPrefab;

    // private Dictionary<string, RoomInfo> cachedRoomList;
    // private Dictionary<string, GameObject> roomListEntries;

    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        goldText.text = PlayerPrefs.GetString("Gold");
        scoreText.text = PlayerPrefs.GetString("Score");
        playerName = PlayerPrefs.GetString("Username");

        // PhotonNetwork.AutomaticallySyncScene = true;

        // cachedRoomList = new Dictionary<string, RoomInfo>();
        // roomListEntries = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

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
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ClearRoomListView();
        // UpdateCachedRoomList(roomList);
        // UpdateRoomListView();
    }

    // private void ClearRoomListView()
    // {
    //     foreach (GameObject entry in roomListEntries.Values)
    //     {
    //         Destroy(entry.gameObject);
    //     }

    //     roomListEntries.Clear();
    // }

    // private void UpdateCachedRoomList(List<RoomInfo> roomList)
    // {
    //     foreach (RoomInfo info in roomList)
    //     {
    //         // Remove room from cached room list if it got closed, became invisible or was marked as removed
    //         if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
    //         {
    //             if (cachedRoomList.ContainsKey(info.Name))
    //             {
    //                 cachedRoomList.Remove(info.Name);
    //             }

    //             continue;
    //         }

    //         // Update cached room info
    //         if (cachedRoomList.ContainsKey(info.Name))
    //         {
    //             cachedRoomList[info.Name] = info;
    //         }
    //         // Add new room info to cache
    //         else
    //         {
    //             cachedRoomList.Add(info.Name, info);
    //         }
    //     }
    // }

    // private void UpdateRoomListView()
    // {
    //     foreach (RoomInfo info in cachedRoomList.Values)
    //     {
    //         GameObject entry = Instantiate(roomListEntryPrefab);
    //         entry.transform.SetParent(roomListContent.transform);
    //         entry.transform.localScale = Vector3.one;
    //         // entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

    //         roomListEntries.Add(info.Name, entry);
    //     }
    // }

    public override void OnJoinedLobby()
    {
        lobbyPage.SetActive(true);
        startPage.SetActive(false);
    }
}
