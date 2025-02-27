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
        public GameObject loadingSpinner;

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
            selectedDescription.fontSize = 46;
            selectedDescription.alignment = TextAlignmentOptions.TopLeft;

            itemListContent.transform.ClearChildren();
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
            return WebRequestHandler.Get<JobListResponse>("/user/get_jobs/", OnGetJobsSuccess);
        }

        private void OnGetJobsSuccess(JobListResponse res)
        {
            jobListContent.transform.ClearChildren();
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
                job.nameTxt.text = jobData.name;
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

        private IEnumerator SetJob()
        {
            var body = new SetJobRequest
            {
                job_id = selectedId
            };
            return WebRequestHandler.Post<SetJobResponse>("/user/set_job/", body, OnSetJobSuccess, null, loadingSpinner);
        }

        private void OnSetJobSuccess(SetJobResponse res)
        {
            gameObject.SetActive(false);
            profilePage.SetActive(true);

            profileController.jobText.text = selectedName.text;
            profileController.avatar.sprite = Resources.Load<Sprite>($"Jobs/{selectedName.text}");
            PlayerPrefs.SetString("Job", selectedName.text);
        }
    }
}
