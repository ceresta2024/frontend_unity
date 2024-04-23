using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float speed = 1;
    public int hitPoint = 100;
    public bool paused = false;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            myRigidbody.velocity = Vector2.zero;
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                MoveWithGyroscope();
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                MoveWithKeyboard();
            }
        }
    }

    public void OnMouseUpAsButton()
    {
        paused = !paused;
    }

    private void MoveWithKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            myRigidbody.velocity = Vector2.up * speed;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            myRigidbody.velocity = Vector2.left * speed;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            myRigidbody.velocity = Vector2.down * speed;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            myRigidbody.velocity = Vector2.right * speed;
        }
    }

    private void MoveWithGyroscope()
    {
        // Get the acceleration along the X-axis
        float xAcceleration = Input.acceleration.x;
        float yAcceleration = Input.acceleration.y;
        float d = Mathf.Sqrt(Mathf.Pow(xAcceleration, 2) + Mathf.Pow(yAcceleration, 2));
        myRigidbody.velocity = new Vector2(xAcceleration / d, yAcceleration / d) * speed;
    }
}
