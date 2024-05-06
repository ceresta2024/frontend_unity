using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceresta
{
    [Serializable]
    public class RequiredItemData
    {
        public int id;
        public string name;
        public int price;
    }

    public class Job : MonoBehaviour
    {
        public RectTransform rectTransform;

        public int jobId;
        public string jobName;
        public string description;
        public RequiredItemData[] items;
        public JobsController controller;

        public void OnJobSelected(bool isSelected)
        {
            if (isSelected)
            {
                rectTransform.sizeDelta = new Vector2(250, 250);
                controller.SetSelectedJob(jobId, jobName, description, items);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(200, 200);
            }
        }
    }
}
