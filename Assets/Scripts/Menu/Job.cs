using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        public TMP_Text nameTxt;

        public void OnJobSelected(bool isSelected)
        {
            if (isSelected)
            {
                nameTxt.fontSize = 54;
                rectTransform.sizeDelta = new Vector2(250, 250);
                controller.SetSelectedJob(jobId, jobName, description, items);
            }
            else
            {
                nameTxt.fontSize = 42;
                rectTransform.sizeDelta = new Vector2(200, 200);
            }
        }
    }
}
