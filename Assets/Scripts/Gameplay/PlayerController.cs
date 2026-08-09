using EscapeDungeon.Core;
using EscapeDungeon.Ui;
using System;
using UnityEngine;


namespace EscapeDungeon.GamePlay
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Moving Controller")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _boostMultiplier = 1.5f;
        [SerializeField] private float _boostDuration = 5f;
        [SerializeField] private float _baseMoveSpeed = 5f;
        [SerializeField] private Transform _cameraRoot;


        [Header("Animation & Torch position")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform torchHolder;
        [SerializeField] private Vector3 _idleTorchLocalPos;
        [SerializeField] private Vector3 _idleTorchLocalRot;
        

        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

        private CharacterController _cc;

        private bool isSpeedBoostActive = false;
        private float boostEndTime = 0f;
        private float _xRotation = 0f;
        private Vector3 _velocity;
        public bool canMove = true;
        public bool allowCursorHandling = true;

        //-------LIFE CYCLE METHOD-------
        #region LifeCycleMethod
        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            Debug.Log($"[PlayerController] Awake on {name}");
        }
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _moveSpeed = _baseMoveSpeed;
        }

        private void Update()
        {
            if (allowCursorHandling)
            {
                HandleCursor();
            }

            if (!canMove) return;

            HandleMovement();
            UpdateWalkingAnimation();
            UpdateTorchPose();
            HandleGravity();
            HandleRotation();


            if(isSpeedBoostActive && Time.time >= boostEndTime)
            {
                isSpeedBoostActive = false;
                _moveSpeed = _baseMoveSpeed;
            }

        }

        private void UpdateTorchPose()
        {
            if (torchHolder == null) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            bool isWalking = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

            if (!isWalking)
            {
                torchHolder.localPosition = _idleTorchLocalPos;
                torchHolder.localRotation = Quaternion.Euler(_idleTorchLocalRot);
            }
        }

        private void UpdateWalkingAnimation()
        {
            if (animator == null) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            bool isWalking = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
            animator.SetBool(IsWalkingHash, isWalking);
        }
        #endregion

        public void SetCanMove(bool value)
        {
            this.canMove = value;
            Debug.Log("[PlayerController] SetCanMove = " + canMove + " (caller: " + Time.time + ")");
        }

        //-------PRIVATE METHOD-------
        #region PrivateMethod
        private void HandleCursor()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void HandleGravity()
        {
            // Small negative value keeps CharacterController.isGrounded reliable
            if (_cc.isGrounded)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime;
            }

            _cc.Move(_velocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 moveDir = (transform.right * h) + (transform.forward * v);

            
            if (moveDir.magnitude > 1f)
            {
                moveDir.Normalize();
            }
            _cc.Move(moveDir * _moveSpeed * Time.deltaTime);


        }

        private void HandleRotation()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            if (_cameraRoot == null)
            {
                Debug.LogError("Camera Root is not assigned in the inspector.");
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Negate mouseY: Unity's X-axis rotation is inverted relative to mouse movement
            _xRotation -= mouseY;

            _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

            _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
        #endregion

        public void ResetPosition(Vector3 targetPos)
        {
            _velocity = Vector3.zero;

            //tắt di chuyển để khi thay đổi vị trí không gây lỗi
            _cc.enabled = false;

            targetPos += new Vector3(0, 1.8f, 0);
            transform.position = targetPos;

            _cc.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[Player] OnTriggerEnter with {other.name}, tag = {other.tag}");
            if (other.tag == "Exit")
            {
                Invoke("TriggerGameManagerExit", 2);
            }
        }

        private void TriggerGameManagerExit()
        {
            GameManager.Instance.HandlePlayerReachExit();
        }

        public void TakeDamage(int amount)
        {
            if(GameManager.Instance != null)
            {
                Debug.Log("Decrease 50 HP");
                GameManager.Instance.ApplyDamageToPlayer(amount);
            }
        }

        public void ApplySpeedBoost()
        {
            isSpeedBoostActive = true;
            boostEndTime = Time.time + _boostDuration;
            _moveSpeed = _baseMoveSpeed * _boostMultiplier;
        }


    }
}
