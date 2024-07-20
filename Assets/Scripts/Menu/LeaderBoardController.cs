using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ceresta
{
    public class LeaderBoardController : MonoBehaviour
    {
        [Serializable]
        class ItemData
        {
            public string name;
            public string job;
            public int score;
        }

        [Serializable]
        class RankingResponse
        {
            [SerializeField]
            public ItemData[] data;
        }

        public GameObject boardContent;

        void OnEnable()
        {
            StartCoroutine(GetRankings());
        }

        private IEnumerator GetRankings()
        {
            return WebRequestHandler.Get<RankingResponse>($"/user/get_rankings", OnRankingItemsSuccess);
        }

        private void OnRankingItemsSuccess(RankingResponse res)
        {
            boardContent.transform.ClearChildren();
            int no = 1;
            foreach (var itemData in res.data)
            {
                var prefab = Resources.Load<GameObject>("RankingItem");
                var rankingObject = GameObject.Instantiate(prefab, boardContent.transform);
                var ranking = rankingObject.GetComponent<Ranking>();
                ranking.Initialize(no, itemData.name, itemData.job, itemData.score);
                no += 1;
            }
        }
    }

}