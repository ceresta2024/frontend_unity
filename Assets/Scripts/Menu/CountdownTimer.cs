// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
// You can do this from Unity's OnDisable function for example.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ceresta
{
    /// <summary>This is a basic, network-synced CountdownTimer based on properties.</summary>
    /// <remarks>
    /// In order to start the timer, the MasterClient can call SetStartTime() to set the timestamp for the start.
    /// The property 'StartTime' then contains the server timestamp when the timer has been started.
    /// 
    /// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired
    /// += OnCountdownTimerIsExpired;
    /// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired
    /// -= OnCountdownTimerIsExpired;.
    /// 
    /// You can do this from Unity's OnEnable and OnDisable functions.
    /// </remarks>
    public class CountdownTimer : MonoBehaviourPunCallbacks
    {
        class ServerTimeResponse
        {
            public long unixtime;
        }

        /// <summary>
        ///     OnCountdownTimerHasExpired delegate.
        /// </summary>
        public delegate void CountdownTimerHasExpired();

        public const string CountdownStartTime = "StartTime";

        private bool isTimerRunning;

        private long startTime;
        private long currentTime;

        [Header("Reference to a Text component for visualizing the countdown")]
        public TMP_Text Text;


        /// <summary>
        ///     Called when the timer has expired.
        /// </summary>
        public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;


        public void Start()
        {
            if (this.Text == null) Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
        }

        public override void OnEnable()
        {
            Debug.Log("OnEnable CountdownTimer");
            base.OnEnable();

            // the starttime may already be in the props. look it up.
            Initialize();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Debug.Log("OnDisable CountdownTimer");
        }


        public void Update()
        {
            if (!this.isTimerRunning) return;

            float countdown = TimeRemaining();

            if (countdown > 0.0f)
            {
                var timeSpan = TimeSpan.FromSeconds(countdown);

                this.Text.text = new DateTime(timeSpan.Ticks).ToString("HH:mm:ss");
                return;
            }

            OnTimerEnds();
        }


        private void OnTimerRuns()
        {
            this.isTimerRunning = true;
            this.enabled = true;
        }

        private void OnTimerEnds()
        {
            this.isTimerRunning = false;
            this.enabled = false;

            Debug.Log("Emptying info text.", this.Text);
            this.Text.text = string.Empty;

            if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();
        }

        private void Initialize()
        {
            StartCoroutine(GetCurrentTime());
        }


        private float TimeRemaining()
        {
            long timer = this.startTime - currentTime;
            return timer;
        }


        public static bool TryGetStartTime(out long startTimestamp)
        {
            startTimestamp = PhotonNetwork.ServerTimestamp;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out object startTimeFromProps))
            {
                startTimestamp = (long)startTimeFromProps;
                return true;
            }

            return false;
        }

        private IEnumerator GetCurrentTime()
        {
            var request = UnityWebRequest.Get("https://worldtimeapi.org/api/timezone/utc");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && TryGetStartTime(out long propStartTime))
            {
                var responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);
                var res = JsonUtility.FromJson<ServerTimeResponse>(responseText);
                this.currentTime = res.unixtime;

                this.startTime = propStartTime;
                Debug.Log("Initialize sets StartTime " + this.startTime + " server time now: " + this.currentTime + " remain: " + TimeRemaining());


                this.isTimerRunning = TimeRemaining() > 0;

                if (this.isTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();

                StartCoroutine(UpdateCurrentTime());
            }
        }

        private IEnumerator UpdateCurrentTime()
        {
            while (true)
            {
                this.currentTime += 1;
                yield return new WaitForSeconds(1);
            }
        }
    }
}