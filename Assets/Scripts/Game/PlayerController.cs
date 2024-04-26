using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float speed = 1;
    public int hitPoint = 100;
    public bool paused = false;

    public float distance = 1f;

    private PolygonCollider2D tunnelExit;

    private float timer = 0.0f;
    private Vector2 oldVelocity;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        tunnelExit = GameObject.FindWithTag("TunnelExit").GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            MoveWithGyroscope();
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            MoveWithKeyboard();
        }
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distance = Vector2.Distance(transform.position, pos);
            if (distance < this.distance)
            {
                ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
                ef.DoExplosion(pos);
            }
        }
        if (paused)
        {
            Debug.Log("Paused for " + timer);
            myRigidbody.velocity = Vector2.zero;
            timer += Time.deltaTime;
            if (timer > 5)
            {
                timer = 0;
                paused = false;
                myRigidbody.velocity = oldVelocity;
                Debug.Log("Resumed, velocity: " + myRigidbody.velocity);
            }
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TunnelEntrance"))
        {
            var exitIdx = Random.Range(0, tunnelExit.pathCount);
            var polygon = tunnelExit.GetPath(exitIdx);
            var centerPos = GetCenter(polygon);
            transform.position = centerPos;
        }
        else if (collision.gameObject.CompareTag("Pit"))
        {
            Debug.Log("Player in pit");
            oldVelocity = myRigidbody.velocity;
            paused = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pit"))
        {
            speed = 0.1f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pit"))
        {
            speed = 1;
        }
    }

    private Vector2 GetCenter(Vector2[] vectors)
    {
        float xMax = float.MinValue;
        float yMax = float.MinValue;
        float xMin = float.MaxValue;
        float yMin = float.MaxValue;
        foreach (var vector in vectors)
        {
            if (xMax < vector.x)
            {
                xMax = vector.x;
            }
            if (yMax < vector.y)
            {
                yMax = vector.y;
            }
            if (xMin > vector.x)
            {
                xMin = vector.x;
            }
            if (yMin > vector.y)
            {
                yMin = vector.y;
            }
        }
        var xCenter = (xMax + xMin) / 2;
        var yCenter = (yMax + yMin) / 2;
        return new Vector2(xCenter, yCenter);
    }
}
