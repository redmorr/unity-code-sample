using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform fpsCamera;
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 mouseInput;
    [SerializeField] private float clampPitch;
    [SerializeField] private float sensitivity;
    
    private float pitch;
    private float yaw;

    private void OnEnable()
    {
        playerInput.Player.Look.performed += ReadMouseDelta;
        playerInput.Player.Look.canceled += ResetMouseDelta;
    }
    
    private void ResetMouseDelta(InputAction.CallbackContext ctx) => mouseInput = Vector2.zero;
    
    private void ReadMouseDelta(InputAction.CallbackContext ctx) => mouseInput = ctx.ReadValue<Vector2>();

    private void Update()
    {
        yaw += mouseInput.x * sensitivity;
        pitch += mouseInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -clampPitch, clampPitch);
        player.localRotation = Quaternion.Euler(0f, yaw, 0f);
        fpsCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    
    private void OnDisable()
    {
        playerInput.Player.Look.performed -= ReadMouseDelta;
        playerInput.Player.Look.canceled -= ResetMouseDelta;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = hasFocus ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !hasFocus;
    }
}