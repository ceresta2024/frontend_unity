using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
//using UnityEditor.Timeline;

public class UIController : MonoBehaviour
{
    [Header("UI PANELS")]
    public GameObject LoginUI;
    public GameObject SignUpUI;
    public GameObject MainGameUI;
    public GameObject NotificationPanel;
    public GameObject loadingSpinner;

    [Header("UI PANELS Text")]
    public TMP_Text NotifyText;

    [Header("Profile Panel")]
    public TMP_Text profileUsername;
    public GameObject profilePanel;
    public TMP_Text profileGoldText;

    [Header("Main Game Panel")]
    public TMP_Text timeText;
    public TMP_Text playerUsernameText;
    public TMP_Text playerGoldText;

    [Header("Login in Setting Panel")]
    public GameObject LoginBtnSettings;
    public GameObject myProfileBtnSettings;


    [Header("Shop and Inventory Panel")]
    public GameObject shopPanel;
    public GameObject inventoryPanel;
    public TMP_Text shopPlayerGoldText;

    static UIController instance;
    public static UIController Instance => instance;

    public int selectedJobID;

    public int selectedShopItemID;
    public int selectedShopItemQuantity;

    public int selectedInventoryItemID;
    public int selectedInventoryItemQuantity;
    public void Awake()
    {
        //if (instance != null)
        //{ Destroy(gameObject); }
        //else
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //    gameObject.name = GetType().Name;
        //}
        instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.name = GetType().Name;
    }

    public void Start()
    {
        
    }

    /*  public void CheckIfUserLoggedIn()
      {
          if (!String.IsNullOrEmpty(PlayerPrefs.GetString("Email").ToString()))
          {
              UserLoggedInUI();
          }
      }*/
    public void UserRegistered()
    {
        SignUpUI.SetActive(false);
        LoginUI.SetActive(true);
    }

    public void UserLoggedInUI()
    {
        SignUpUI.SetActive(false);
        LoginUI.SetActive(false);

        MainGameUI.SetActive(true);

        string[] strings = PlayerPrefs.GetString("Email").Split('@');
        profileUsername.text = strings[0];
        playerUsernameText.text = strings[0];

       // playerUsernameText.transform.parent.gameObject.SetActive(true);
        LoginBtnSettings.SetActive(false);
        myProfileBtnSettings.SetActive(true);
        UpdateGoldText();
        //TimeShower.Instance.CountdownShow(serverTimeUtc.ToString("HH:mm:ss"));

    }
    public void UpdateGoldText()
    {
        playerGoldText.text = PlayerPrefs.GetInt("UserGold").ToString();
        shopPlayerGoldText.text = PlayerPrefs.GetInt("UserGold").ToString();
        profileGoldText.text = PlayerPrefs.GetInt("UserGold").ToString();
    }
    public void GiveNotification(string notification)
    {
        NotifyText.text = notification;
        NotificationPanel.SetActive(true);

        
    }

    public void UserLoggedOutUI()
    {
        SignUpUI.SetActive(false);
        MainGameUI.SetActive(false);
        profilePanel.SetActive(false);

        LoginUI.SetActive(true);
        LoginBtnSettings.SetActive(true);
    }   
}
