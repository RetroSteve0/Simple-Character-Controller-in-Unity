using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform target;
    private float _distanceToPlayer;
    private Vector2 _input;

    [SerializeField] private MouseSensitivity mouseSensitivity;
    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation _cameraRotation;
    private bool _isRightMouseButtonPressed;

    #endregion

    private void Awake() => _distanceToPlayer = Vector3.Distance(transform.position, target.position);

    public void Look(InputAction.CallbackContext context)
    {
        if (_isRightMouseButtonPressed)
        {
            _input = context.ReadValue<Vector2>();
        }
        else
        {
            _input = Vector2.zero;
        }
    }

    public void OnRightMouseButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isRightMouseButtonPressed = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (context.canceled)
        {
            _isRightMouseButtonPressed = false;
            _input = Vector2.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        if (_isRightMouseButtonPressed)
        {
            _cameraRotation.Yaw += _input.x * mouseSensitivity.horizontal * BoolToInt(mouseSensitivity.invertHorizontal) * Time.deltaTime;
            _cameraRotation.Pitch += _input.y * mouseSensitivity.vertical * BoolToInt(mouseSensitivity.invertVertical) * Time.deltaTime;
            _cameraRotation.Pitch = Mathf.Clamp(_cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
        }
    }

    private void LateUpdate()
    {
        // Always update the camera's position to follow the player
        transform.position = target.position - (Quaternion.Euler(_cameraRotation.Pitch, _cameraRotation.Yaw, 0.0f) * Vector3.forward * _distanceToPlayer);

        // Only update the camera's rotation if the right mouse button is pressed
        if (_isRightMouseButtonPressed)
        {
            transform.rotation = Quaternion.Euler(_cameraRotation.Pitch, _cameraRotation.Yaw, 0.0f);
        }
        else
        {
            // Maintain the rotation to always look at the target
            transform.LookAt(target);
            _cameraRotation.Yaw = transform.eulerAngles.y;
            _cameraRotation.Pitch = transform.eulerAngles.x;
        }
    }

    private static int BoolToInt(bool b) => b ? 1 : -1;
}

[Serializable]
public struct MouseSensitivity
{
    public float horizontal;
    public float vertical;
    public bool invertHorizontal;
    public bool invertVertical;
}

public struct CameraRotation
{
    public float Pitch;
    public float Yaw;
}

[Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}
