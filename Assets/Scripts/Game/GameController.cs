using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class GameController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // string prefab = "Prefabs/Objects/" + substationData.objList[count];
    }

    // Update is called once per frame
    void Update()
    {

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

    private void StartGame()
    {
        Debug.Log("Start game");
    }

    private void OnCountdownTimerIsExpired()
    {
        StartGame();
    }
}
