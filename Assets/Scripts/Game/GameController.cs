using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ceresta
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        [Serializable]
        class ItemData
        {
            public int id;
            public string name;
            public int item_id;
            public int price;
            public int quantity;
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
        public GameObject itemsListContent;
        public GameObject rewardBox;
        public TMP_Text scoreText;
        public Image rewardImage;

        private GameObject player;
        private Camera mainCamera;
        private Vector3 offset;
        private float damping;
        private Vector3 vel = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (player)
            {
                Vector3 targetPosition = player.transform.position + offset;
                targetPosition.z = mainCamera.transform.position.z;

                mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref vel, damping);
            }
        }

        public void OnBackButtonClicked()
        {
            StartCoroutine(RemoveUserFromServer(PhotonNetwork.CurrentRoom.Name));
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
                var mapObject = GameObject.Instantiate(mapPrefab, position, rotation);
                var startTransform = mapObject.transform.Find("Start");
                player = PhotonNetwork.Instantiate("Player", startTransform.position, Quaternion.identity);
            }

            StartCoroutine(GetInventoryItems());
        }

        public void EndOfGame()
        {
            StartCoroutine(GetReward(PhotonNetwork.CurrentRoom.Name));
        }

        private IEnumerator GetInventoryItems()
        {
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var url = $"{Config.baseUrl}/shop/get_inventory_list/";
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                itemsListContent.transform.DetachChildren();
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<ItemsResponse>(responseText);
                foreach (var itemData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("Item");
                    var itemObject = GameObject.Instantiate(prefab, itemsListContent.transform);
                    itemObject.SetActive(true);
                    var item = itemObject.GetComponent<ItemInGame>();
                    var sp = Resources.Load<Sprite>($"Items/{itemData.item_id}");
                    item.image.sprite = sp;
                }
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        private IEnumerator RemoveUserFromServer(string roomName)
        {
            var accessToken = PlayerPrefs.GetString("AccessToken", "");
            var url = $"{Config.baseUrl}/game/remove_user/?room_id={roomName}";
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
            var url = $"{Config.baseUrl}/game/get_reward/";
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

                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}
