using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ConnectionStatus : MonoBehaviour
{
    public TMP_Text connectionStatusText;

    private readonly string connectionStatusMessage = "Connection Status: ";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }
}
