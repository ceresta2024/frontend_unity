using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleObstacles : MonoBehaviour
{
    PlayerManager playerManager;
    //public Text testText;


    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grass")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 1f;
            spriteOpacity.color = spriteColor;

            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                playerManager.movePixelValue = 0.005f;
            }
            else
            {
                playerManager.movePixelValue = 0.5f;
            }            
        }
        if (other.tag == "Thorn")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 1f;
            spriteOpacity.color = spriteColor;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                playerManager.movePixelValue = 0.005f;
            }
            else
            {
                playerManager.movePixelValue = 0.5f;
            }
        }
        if (other.tag == "Pit")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 1f;
            spriteOpacity.color = spriteColor;
            playerManager.movePixelValue = 0f;
            StartCoroutine(PitWait());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grass")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 0.2f;
            spriteOpacity.color = spriteColor;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                playerManager.movePixelValue = 0.01f;
            }
            else
            {
                playerManager.movePixelValue = 1f;
            }
        }
        if (other.tag == "Thorn")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 0.2f;
            spriteOpacity.color = spriteColor;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                playerManager.movePixelValue = 0.01f;
            }
            else
            {
                playerManager.movePixelValue = 1f;
            }
        }

        if (other.tag == "Pit")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 0.2f;
            spriteOpacity.color = spriteColor;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                playerManager.movePixelValue = 0.01f;
            }
            else
            {
                playerManager.movePixelValue = 1f;
            }
        }
    }

    private IEnumerator PitWait()
    {
        yield return new WaitForSeconds(3f);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            playerManager.movePixelValue = 0.01f;
        }
        else
        {
            playerManager.movePixelValue = 1f;
        }
    }
}
