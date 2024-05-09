using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ceresta
{
    public class JobsController : MonoBehaviour
    {
        [Serializable]
        class JobData
        {
            public int id;
            public string name;
            public string description;
            public int speed;
            public int allow_gold;

            [SerializeField]
            public RequiredItemData[] items;
        }

        [Serializable]
        class JobListResponse
        {
            [SerializeField]
            public JobData[] data;
        }

        class SetJobRequest
        {
            public int job_id;
        }

        class SetJobResponse
        {
            public string message;
        }

        public GameObject profilePage;
        public ProfileController profileController;

        public GameObject jobListContent;
        public TMP_Text selectedName;
        public TMP_Text selectedDescription;
        public GameObject itemListContent;
        private int selectedId;

        void Awake()
        {
            StartCoroutine(GetJobs());
        }

        public void SetSelectedJob(int jobId, string jobName, string jobDescription, RequiredItemData[] items)
        {
            selectedId = jobId;
            selectedName.text = jobName;
            selectedDescription.text = jobDescription;

            itemListContent.transform.DetachChildren();
            foreach (var itemData in items)
            {
                var prefab = Resources.Load<GameObject>("RequiredItem");
                var itemObject = GameObject.Instantiate(prefab, itemListContent.transform);
                var requiredItem = itemObject.GetComponent<RequiredItem>();
                requiredItem.image.sprite = Resources.Load<Sprite>($"Items/{itemData.id}");
                requiredItem.nameText.text = itemData.name;
                requiredItem.priceText.text = itemData.price.ToString();
            }
        }

        public void OnConfirmClicked()
        {
            StartCoroutine(SetJob());
        }

        private IEnumerator GetJobs()
        {
            var accessToken = PlayerPrefs.GetString("AccessToken");
            var url = Config.baseUrl + "/user/get_jobs/";
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<JobListResponse>(responseText);

                jobListContent.transform.DetachChildren();
                var jobName = PlayerPrefs.GetString("Job");
                foreach (var jobData in res.data)
                {
                    var prefab = Resources.Load<GameObject>("Job");
                    var itemObject = GameObject.Instantiate(prefab, jobListContent.transform);

                    var toggle = itemObject.GetComponent<Toggle>();
                    toggle.group = jobListContent.GetComponent<ToggleGroup>();
                    toggle.isOn = false;

                    var image = itemObject.GetComponent<Image>();
                    image.sprite = Resources.Load<Sprite>($"Jobs/{jobData.name}");

                    var job = itemObject.GetComponent<Job>();
                    job.jobId = jobData.id;
                    job.jobName = jobData.name;
                    job.description = jobData.description;
                    job.items = jobData.items;
                    job.controller = this;

                    if (jobData.name == jobName)
                    {
                        toggle.isOn = true;
                        profileController.jobText.text = jobData.name;
                        var sp = Resources.Load<Sprite>($"Jobs/{jobData.name}");
                        profileController.avatar.sprite = sp;
                    }
                }
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }

        private IEnumerator SetJob()
        {
            var body = new SetJobRequest
            {
                job_id = selectedId
            };
            var url = Config.baseUrl + "/user/set_job/";
            var accessToken = PlayerPrefs.GetString("AccessToken");

            var request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseText = request.downloadHandler.text;
                var res = JsonUtility.FromJson<SetJobResponse>(responseText);
                Debug.Log(res.message);

                gameObject.SetActive(false);
                profilePage.SetActive(true);

                profileController.jobText.text = selectedName.text;
                profileController.avatar.sprite = Resources.Load<Sprite>($"Jobs/{selectedName.text}");
                PlayerPrefs.SetString("Job", selectedName.text);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.text);
            }
        }
    }
}
