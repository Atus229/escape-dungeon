using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _mouseSensitivity = 2f; // tốc độ xoay theo 2 trục
    [SerializeField] private Transform _cameraRoot;

    private float _xRotation = 0f; //Xoay theo trục x
    private CharacterController _cc;

    private Vector3 _velocity;// vector3 -> lưu vận tốc tích lũy theo thời gian theo y

    void Awake()
    {
       _cc = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // khóa chuột trong màn hình trò chơi
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleRotation();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; //Mở khóa chuột khi nhấn ESC
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked; //Khóa chuột khi nhấn chuột trái
        }
    }

    private void HandleGravity()
    {
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
            moveDir.Normalize(); //Normalize -> độ dài vector = 1
        }
        _cc.Move(moveDir * _moveSpeed * Time.deltaTime);

    }
    private void HandleRotation()
    {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        // 1. Đọc input chuột
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;


        // 2. Tích lũy góc dọc -> lên xuông, đảo dấu vì unity có trục x xoay từ trên xuống
        _xRotation -= mouseY;

        // 3. Giới hạn góc dọc
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        // 4. Gán rotation cho CameraRoot (chỉ trục X)
        _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // 5. Xoay Player theo chiều ngang
        transform.Rotate(Vector3.up * mouseX);
    }
}
