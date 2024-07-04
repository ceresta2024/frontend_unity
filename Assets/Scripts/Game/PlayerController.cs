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
        public CircleCollider2D myCollider;
        public Animator animator;
        public PhotonView photonView;

        public float speed = 1;
        private float oldSpeed = 0;
        public float speedRateInPit = 0.3f;
        public float speedRateInThorn = 0.5f;
        public int hitPoint = 100;
        public bool paused = false;

        public float distance = 1f;

        private float itemDuration = 0;
        private bool isInPit = false;
        private bool isInThorn = false;

        private PolygonCollider2D tunnelExit;

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
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/{jobName}/{jobName}");
            }
            else if (photonView.Owner.CustomProperties.TryGetValue("Job", out object job))
            {
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Characters/{(string)job}/{(string)job}");
            }
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            int speed = (int)myRigidbody.velocity.magnitude * 100;
            gameController.speedText.text = $"{speed}/100";
            gameController.hpText.text = $"{hitPoint}/100";
            Debug.Log("Speed: " + myRigidbody.velocity.magnitude);
            Debug.Log("In pit: " + isInPit);
            Debug.Log("In thorn: " + isInThorn);
            if (myRigidbody.velocity.magnitude > 0 || paused)
            {
                if (paused)
                {
                    animator.Rebind();
                }
                else
                {
                    animator.Play("Walk");
                }
                myRigidbody.SetRotation(Mathf.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg - 90);
            }
            else
            {
                animator.Play("Idle");
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (paused)
            {
                myRigidbody.velocity = Vector2.zero;
                return;
            }

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                MoveWithGyroscope();
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                MoveWithKeyboard();
            }
            // if (Input.GetMouseButtonDown(0))
            // {
            //     var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     var distance = Vector2.Distance(transform.position, pos);
            //     if (distance < this.distance)
            //     {
            //         ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            //         ef.DoExplosion(pos);
            //     }
            // }
            // vfxRenderer.SetVector3("ColliderPos", transform.position);
        }

        private void MoveWithKeyboard()
        {
            var xInput = Input.GetAxis("Horizontal");
            var yInput = Input.GetAxis("Vertical");

            SetVelocity(xInput, yInput);
        }

        private void MoveWithGyroscope()
        {
            float xAcceleration = Input.acceleration.x;
            float yAcceleration = Input.acceleration.y;
            SetVelocity(xAcceleration, yAcceleration);
        }

        private void SetVelocity(float xInput, float yInput)
        {
            var inputVector = new Vector2(xInput, yInput);
            var length = inputVector.magnitude;

            if (length > 0)
            {
                if (isInPit)
                {
                    myRigidbody.velocity = new Vector2(xInput / length * speed * speedRateInPit, yInput / length * speed * speedRateInPit);
                }
                else if (isInThorn)
                {
                    myRigidbody.velocity = new Vector2(xInput / length * speed * speedRateInThorn, yInput / length * speed * speedRateInThorn);
                }
                else
                {
                    myRigidbody.velocity = new Vector2(xInput / length * speed, yInput / length * speed);
                }
            }
            else
            {
                myRigidbody.velocity = Vector2.zero;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (collision.gameObject.CompareTag("TunnelEntrance"))
            {
                var exitIdx = Random.Range(0, tunnelExit.pathCount);
                var polygon = tunnelExit.GetPath(exitIdx);
                var centerPos = GetCenter(polygon);
                transform.position = centerPos;
                Invoke("LoadExitEffect", 0);
                Invoke("UnloadExitEffect", 5);
            }
            else if (collision.gameObject.CompareTag("Goal"))
            {
                Debug.Log("you reached to the goal");
                paused = true;
                gameController.EndOfGame();
            }
        }

        void LoadStarEffect()
        {
            GameObject newObject = new GameObject("StarEffect");
            SpriteRenderer renderer = newObject.AddComponent<SpriteRenderer>();
            Animator animator = newObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Effects/pit/star1");
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            renderer.sprite = Resources.Load<Sprite>($"Effects/pit/star1");
            newObject.transform.localScale = new Vector3(0.2f, 0.2f);
            newObject.transform.position = transform.position;
        }

        void UnloadStarEffect()
        {
            GameObject rsObj = GameObject.Find("StarEffect");
            Destroy(rsObj);
        }
        void LoadBloodEffect()
        {
            GameObject newObject = new GameObject("BloodEffect");
            SpriteRenderer renderer = newObject.AddComponent<SpriteRenderer>();
            Animator animator = newObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Effects/thorn/blood");
            animator.Play("BloodEffect");
            //animator.cullingMode = AnimatorCullingMode.CullCompletely;
            renderer.sprite = Resources.Load<Sprite>($"Effects/thorn/1");
            newObject.transform.localScale = new Vector3(0.2f, 0.2f);
            newObject.transform.position = transform.position;
        }

        void UnloadBloodEffect()
        {
            GameObject rsObj = GameObject.Find("BloodEffect");
            Destroy(rsObj);
            if (isInThorn)
            {
                Invoke("LoadBloodEffect", 0);
                Invoke("UnloadBloodEffect", 1);
            }
        }

        void LoadExitEffect()
        {
            GameObject newObject = new GameObject("ExitEffect");
            SpriteRenderer renderer = newObject.AddComponent<SpriteRenderer>();
            Animator animator = newObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Effects/exit");
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            renderer.sprite = Resources.Load<Sprite>($"Effects/1");
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2(1, 1);
            newObject.transform.position = transform.position;
        }

        void UnloadExitEffect()
        {
            GameObject rsObj = GameObject.Find("ExitEffect");
            Destroy(rsObj);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (IsInside(other) && other.gameObject.CompareTag("Pit"))
            {
                if (!isInPit)
                {
                    paused = true;
                    hitPoint -= 5;
                    Invoke("LoadStarEffect", 0);
                    Invoke("UnloadStarEffect", 5);
                    StartCoroutine(ResumeInSeconds(5));
                }
                Debug.Log("In pit");
                isInPit = true;
            }
            else if (IsInside(other) && other.gameObject.CompareTag("Thorn"))
            {
                if (!isInThorn)
                {
                    hitPoint -= 3;
                    Invoke("LoadBloodEffect", 0);
                    Invoke("UnloadBloodEffect", 1);
                }
                Debug.Log("In thorn");
                isInThorn = true;
            }
            else
            {
                isInPit = false;
                isInThorn = false;
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

        public void UseItem(GameController.ItemData itemData, ItemInGame item)
        {
            if (itemData.function == 1)
            {
                hitPoint += (int)float.Parse(itemData.hp);
            }
            else if (itemData.function == 2)
            {
                oldSpeed = speed;
                if (itemData.sp.StartsWith("*"))
                {
                    speed *= float.Parse(itemData.sp[1..]);
                }
                else
                {
                    speed += float.Parse(itemData.sp[1..]);
                }
                itemDuration = float.Parse(itemData.duration);
                StartCoroutine(ResetSpeedAfterUsage(itemDuration));
            }
            if (itemData.quantity == 1)
            {
                GameObject.Destroy(item.gameObject);
            }
            else
            {
                item.qtyText.text = $"{itemData.quantity - 1}";
            }

            StartCoroutine(WebRequestHandler.Post<UseItemResponse>($"/game/use_item/?item_id={itemData.item_id}", "", null));
        }

        private IEnumerator ResumeInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            paused = false;
        }

        private IEnumerator ResetSpeedAfterUsage(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            speed = oldSpeed;
        }

        private bool IsInside(Collider2D other)
        {
            if (other.OverlapPoint(myCollider.bounds.center))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
