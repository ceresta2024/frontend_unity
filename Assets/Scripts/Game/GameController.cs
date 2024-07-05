using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ceresta
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        [Serializable]
        public class ItemData
        {
            public int id;
            public string name;
            public int item_id;
            public int price;
            public int quantity;
            public int function;
            public string hp;
            public string sp;
            public string duration;
        }

        [Serializable]
        class ItemsResponse
        {
            [SerializeField]
            public ItemData[] data;
        }

        class RewardRequest
        {
            public string room_id;
            public int map_id;
        }

        class RewardResponse
        {
            public int user_score;
            public int item_id;
            public string item_name;
            public int gold;
        }

        public Weather weather;
        public GameObject itemsListWrapper;
        public GameObject itemsListContent;
        public GameObject rewardBox;
        public TMP_Text scoreText;
        public Image rewardImage;
        public GameObject confirmModal;
        public GameObject expandButton;
        public GameObject collapseButton;

        private GameObject player;
        private Camera mainCamera;
        private Vector3 offset;
        private float damping;
        private Vector3 vel = Vector3.zero;

        private GameObject map;

        public TMP_Text speedText;
        public TMP_Text hpText;
        public TMP_Text playersText;
        public PlayerController playerController;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            if (PhotonNetwork.IsMasterClient && !CountdownTimer.TryGetStartTime(out int startTimestamp))
            {
                CountdownTimer.SetStartTime();
            }

            Initialize();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        // Update is called once per frame
        void Update()
        {
            if (player)
            {
                Vector3 targetPosition = player.transform.position + offset;
                targetPosition.z = mainCamera.transform.position.z;
                Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
                Vector2 screenOrigo = Camera.main.ScreenToWorldPoint(Vector2.zero);
                var startTransform = map.transform.Find("Start");
                var endTransform = map.transform.Find("Goal");
                float width = screenBounds.x - screenOrigo.x;
                float height = screenBounds.y - screenOrigo.y;
                float diffXFromStart = player.transform.position.x + (float)16.1;
                float diffYFromStart = player.transform.position.y - startTransform.position.y;
                float diffXFromGoal = (float)16.1 - player.transform.position.x;
                float diffYFromGoal = endTransform.position.y - player.transform.position.y;
                // this.speedText.text = $"Speed: {player.transform.position.x} --- {diffXFromStart} --- {width} --- {startTransform.position.y}";
                if (diffXFromStart < width)
                {
                    targetPosition.x = targetPosition.x + (width / 2 - diffXFromStart);
                }
                if (diffYFromStart < height * 0.8)
                {
                    targetPosition.y = targetPosition.y + (height / 2 - diffYFromStart - (float)0.99);
                }
                if (diffXFromGoal < width)
                {
                    targetPosition.x = targetPosition.x - (width / 2 - diffXFromGoal);
                }
                if (diffYFromGoal < height * 0.8)
                {
                    targetPosition.y = targetPosition.y - (height / 2 - diffYFromGoal - (float)0.99);
                }
                mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref vel, damping);
            }
        }

        public void OnBackButtonClicked()
        {
            confirmModal.SetActive(true);
        }

        public void OnYesButtonClicked()
        {
            StartCoroutine(RemoveUserFromServer(PhotonNetwork.CurrentRoom.Name));
        }

        public void OnNoButtonClicked()
        {
            confirmModal.SetActive(false);
        }

        public void OnItemCollapseButton()
        {
            expandButton.SetActive(true);
            collapseButton.SetActive(false);
            itemsListWrapper.SetActive(false);
        }

        public void OnItemExpandButton()
        {
            expandButton.SetActive(false);
            collapseButton.SetActive(true);
            itemsListWrapper.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MainUI");
        }

        private void Initialize()
        {
            var position = Vector2.zero;
            var rotation = Quaternion.Euler(Vector2.zero);
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Map", out object mapId))
            {
                var mapPrefab = Resources.Load<GameObject>($"Maps/{(int)mapId}");
                map = GameObject.Instantiate(mapPrefab, position, rotation);
            }
            playersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/20";

            StartCoroutine(WebRequestHandler.Get<ItemsResponse>("/shop/get_inventory_list/", OnGetInventorySuccess));
            // need to connect with photon
            OnGetUseItemsSuccess();
        }

        public void EndOfGame()
        {
            StartCoroutine(GetReward(PhotonNetwork.CurrentRoom.Name));
        }

        private void OnGetUseItemsSuccess()
        {
            itemsListContent.transform.ClearChildren();
            var prefab = Resources.Load<GameObject>("UseItem");
            var itemObject = GameObject.Instantiate(prefab, itemsListContent.transform);

            var useItem = itemObject.GetComponent<UseItem>();
            useItem.image.sprite = Resources.Load<Sprite>($"Items/3");
            useItem.qtyText.text = "1";
        }

        private void OnGetInventorySuccess(ItemsResponse res)
        {
            itemsListContent.transform.ClearChildren();
            foreach (var itemData in res.data)
            {
                var prefab = Resources.Load<GameObject>("Item");
                var itemObject = GameObject.Instantiate(prefab, itemsListContent.transform);
                itemObject.SetActive(true);
                var item = itemObject.GetComponent<ItemInGame>();
                var sp = Resources.Load<Sprite>($"Items/{itemData.item_id}");
                item.image.sprite = sp;
                item.qtyText.text = $"{itemData.quantity}";
                item.button.onClick.AddListener(() =>
                {
                    OnItemClicked(itemData, item);
                });
            }
        }

        private IEnumerator RemoveUserFromServer(string roomName)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = $"{CerestaGame.baseUrl}/game/remove_user/?room_id={roomName}";
            var request = UnityWebRequest.Post(url, "", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                Debug.Log(responseText);

                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        private IEnumerator GetReward(string roomName)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = $"{CerestaGame.baseUrl}/game/get_reward/";
            var body = new RewardRequest
            {
                room_id = roomName,
                map_id = 3,
            };
            var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<RewardResponse>(responseText);
                rewardImage.sprite = Resources.Load<Sprite>($"Items/{res.item_id}");
                scoreText.text = $"Score: {res.user_score}";
                rewardBox.SetActive(true);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            playersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/20";
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            playersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/20";
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            if (CountdownTimer.TryGetStartTime(out int startTimestamp))
            {
                CountdownTimer.SetStartTime();
            }
        }

        public void OnItemClicked(ItemData itemData, ItemInGame item)
        {

            playerController.UseItem(itemData, item);

        }

        private void OnCountdownTimerIsExpired()
        {
            Debug.Log("StartGame!");
            var startTransform = map.transform.Find("Start");
            player = PhotonNetwork.Instantiate("Player", startTransform.position, Quaternion.identity);
        }
    }
}
