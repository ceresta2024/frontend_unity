using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleObstacles : MonoBehaviour
{
    MS_PlayerManager mS_PlayerManager;


    private void Start()
    {
        mS_PlayerManager = GetComponent<MS_PlayerManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grass")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 0.2f;
            spriteOpacity.color = spriteColor;
            mS_PlayerManager.movePixelValue = 1.5f;
        }
        if (other.tag == "Thorn")
        {
            mS_PlayerManager.movePixelValue = 0.5f;
        }
        if (other.tag == "Pit")
        {
            mS_PlayerManager.movePixelValue = 0f;
            StartCoroutine(PitWait());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grass")
        {
            SpriteRenderer spriteOpacity = other.gameObject.GetComponent<SpriteRenderer>();
            Color spriteColor = spriteOpacity.color;
            spriteColor.a = 1f;
            spriteOpacity.color = spriteColor;
            mS_PlayerManager.movePixelValue = 1f;

        }
        if (other.tag == "Thorn")
        {
            mS_PlayerManager.movePixelValue = 1f;
        }
    }

    private IEnumerator PitWait()
    {
        yield return new WaitForSeconds(3);
        mS_PlayerManager.movePixelValue = 1f;
    }
}
