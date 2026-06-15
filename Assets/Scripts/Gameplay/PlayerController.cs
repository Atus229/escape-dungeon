using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] 
public class PlayerController : MonoBehaviour
{
    //-------VARIABLES-------
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _mouseSensitivity = 2f; 
    [SerializeField] private Transform _cameraRoot;

    private CharacterController _cc;

    private float _xRotation = 0f; 
    private Vector3 _velocity;

    //-------LIFE CYCLE METHOD-------
    #region LifeCycleMethod
    void Awake()
    {
       _cc = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleRotation();
        HandleCursor();
     
    }
    #endregion

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
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        if(_cameraRoot == null)
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
}
