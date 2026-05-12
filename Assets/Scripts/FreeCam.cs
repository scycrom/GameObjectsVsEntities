using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _fastMoveSpeed = 30f;
    [SerializeField] private float _scrollSpeed = 50f;

    [Header("Look Settings")]
    [SerializeField] private float _lookSensitivity = 0.1f;
    [SerializeField] private bool _invertY = false;

    private float _yaw;
    private float _pitch;

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        _yaw = euler.y;
        _pitch = euler.x;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        
        if (keyboard == null) return;

        bool isSprinting = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        float speed = isSprinting ? _fastMoveSpeed : _moveSpeed;

        Vector3 movement = Vector3.zero;

        // Forward/Backward
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            movement += transform.forward;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            movement -= transform.forward;

        // Left/Right
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            movement -= transform.right;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            movement += transform.right;

        // Up/Down
        if (keyboard.eKey.isPressed || keyboard.spaceKey.isPressed)
            movement += Vector3.up;
        if (keyboard.qKey.isPressed || keyboard.leftCtrlKey.isPressed)
            movement -= Vector3.up;

        // Scroll wheel for forward/backward
        if (mouse != null)
        {
            float scroll = mouse.scroll.y.ReadValue() / 120f; // Normalize scroll value
            if (scroll != 0)
            {
                movement += transform.forward * scroll * _scrollSpeed;
            }
        }

        if (movement.sqrMagnitude > 0)
        {
            transform.position += movement.normalized * speed * Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        // Only rotate when holding right mouse button
        if (mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            
            float mouseX = mouseDelta.x * _lookSensitivity;
            float mouseY = mouseDelta.y * _lookSensitivity;

            _yaw += mouseX;
            _pitch += _invertY ? mouseY : -mouseY;

            // Clamp pitch to prevent camera flipping
            _pitch = Mathf.Clamp(_pitch, -90f, 90f);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}
