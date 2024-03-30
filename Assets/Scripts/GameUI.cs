using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameUI;

    void Start()
    {
        
    }

    void Update()
    {
        gameUI.transform.localScale = new Vector3(Screen.width / 720f, Screen.height / 1440f, 1f);
    }

    public void BackBtnClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
