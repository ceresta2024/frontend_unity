using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VFX;
using WebSocketSharp;

namespace Ceresta
{
    public class PlayerController : MonoBehaviour
    {
        class UseItemRequest
        {
        }

        class UseItemResponse
        {

        }

        public Rigidbody2D myRigidbody;
        public Animator animator;
        public PhotonView photonView;

        public float speed = 1;
        private float oldSpeed = 0;
        public float speedInPit = 0.1f;
        public float speedInThorn = 0.3f;
        public int hitPoint = 100;
        public bool paused = false;

        public float distance = 1f;

        private bool usingSpeedItem = false;
        private float itemDuration = 0;
        private float itemTimer = 0;

        private PolygonCollider2D tunnelExit;

        private float timer = 0.0f;
        private Vector2 oldVelocity;

        private GameController gameController;
        private VisualEffect vfxRenderer;

        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            tunnelExit = GameObject.FindWithTag("TunnelExit").GetComponent<PolygonCollider2D>();
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            gameController.playerController = this;
            // vfxRenderer = GameObject.Find("Fog").GetComponent<VisualEffect>();
            // vfxRenderer.SetBool("IsCollideWithSphere", true);
            if (photonView.IsMine)
            {
                var jobName = PlayerPrefs.GetString("Job");
                if (jobName.IsNullOrEmpty())
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/doctor/doctor");
                }
                else
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/{jobName}/{jobName}");
                }
            }
            else
            {
                if (photonView.Owner.CustomProperties.TryGetValue("Job", out object job))
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/{(string)job}/{(string)job}");
                }
                else
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/doctor/doctor");
                }
            }
        }

        void Update()
        {
            gameController.speedText.text = $"Speed: {myRigidbody.velocity.magnitude}";
            gameController.hpText.text = $"HP: {hitPoint}";
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }

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
            if (usingSpeedItem)
            {
                Debug.Log("Used item for " + itemTimer);
                itemTimer += Time.deltaTime;
                if (itemTimer > itemDuration)
                {
                    itemTimer = 0;
                    usingSpeedItem = false;
                    speed = oldSpeed;
                }
            }
            if (myRigidbody.velocity.magnitude > 0)
            {
                animator.Play("Walk");
            }
            else
            {
                animator.Play("Idle");
            }
            myRigidbody.SetRotation(Mathf.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg - 90);
            // vfxRenderer.SetVector3("ColliderPos", transform.position);
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
                Debug.Log("Player entered in pit");
                oldVelocity = myRigidbody.velocity;
                paused = true;
                hitPoint -= 5;
            }
            else if (collision.gameObject.CompareTag("Thorn"))
            {
                Debug.Log("Player entered in thorn");
                hitPoint -= 3;
            }
            else if (collision.gameObject.CompareTag("Goal"))
            {
                Debug.Log("you reached to the goal");
                myRigidbody.velocity = Vector2.zero;
                gameController.EndOfGame();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Pit"))
            {
                speed = speedInPit;
            }
            else if (collision.gameObject.CompareTag("Thorn"))
            {
                speed = speedInThorn;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Pit") || collision.gameObject.CompareTag("Thorn"))
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

        public void OnUseItem(GameController.ItemData itemData, ItemInGame item)
        {
            if (itemData.function == 1)
            {
                hitPoint += (int)float.Parse(itemData.hp);
            }
            else if (itemData.function == 2)
            {
                oldSpeed = speed;
                speed = float.Parse(itemData.sp);
                itemDuration = float.Parse(itemData.duration);
                usingSpeedItem = true;
            }
            item.qtyText.text = $"{itemData.quantity - 1}";
            StartCoroutine(WebRequestHandler.Post<UseItemResponse>($"/game/use_item/?item_id={itemData.item_id}", "", OnUseItemSuccess));
        }

        private void OnUseItemSuccess(UseItemResponse res)
        {

        }
    }

}
