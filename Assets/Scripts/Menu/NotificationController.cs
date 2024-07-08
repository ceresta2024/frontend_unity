using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ceresta
{
    public class NotificationController : MonoBehaviour
    {
        [Serializable]
        class NotificationData
        {
            public int id;
            public string name;
            public string contents;
            public string start_date;
            public string end_date;
        }

        [Serializable]
        class NotificationResponse
        {
            [SerializeField]
            public NotificationData[] data;
        }

        public GameObject boardContent;

        void OnEnable()
        {
            StartCoroutine(GetNotifications());
        }

        private IEnumerator GetNotifications()
        {
            return WebRequestHandler.Get<NotificationResponse>($"/user/get_notice", OnNotificationItemsSuccess);
        }

        private void OnNotificationItemsSuccess(NotificationResponse res)
        {
            boardContent.transform.ClearChildren();
            foreach (var itemData in res.data)
            {
                var prefab = Resources.Load<GameObject>("NotificationItem");
                var notificationObject = GameObject.Instantiate(prefab, boardContent.transform);
                var notification = notificationObject.GetComponent<Notification>();
                notification.Initialize(itemData.name, itemData.contents, itemData.start_date);
            }
        }
    }

}