using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    private CharacterController _cc;

    private Vector3 _velocity;// vector3 -> lưu vận tốc tích lũy theo thời gian theo y

    void Awake()
    {
       _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        
    }

    private void HandleGravity()
    {
        if (_cc.isGrounded)
        {
            _velocity.y = -2f;
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
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

        _cc.Move(moveDir * moveSpeed * Time.deltaTime);
    }
}
