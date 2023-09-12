using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SimpleController : MonoBehaviour
{
    [FormerlySerializedAs("input")] [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform player;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Vector2 move;
    [SerializeField] private Vector3 wishDir;
    [SerializeField] private LayerMask mask;
    
    private Rigidbody rb;
    private GroundCheck gc;
    private Caster caster;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerInput.Player.Move.performed += ReadMovement;
        playerInput.Player.Move.canceled += ResetMovement;
    }
    
    private void ReadMovement(InputAction.CallbackContext ctx) => move = ctx.ReadValue<Vector2>();
    private void ResetMovement(InputAction.CallbackContext ctx) => move = Vector2.zero;

    
    private void FixedUpdate()
    {
        wishDir = player.forward * move.y + player.right * move.x;

        Vector3 delta = wishDir * maxSpeed * Time.deltaTime;
        Vector3 firstCastOrigin = rb.position + delta + Vector3.up * 0.5f;
        Vector3 firstCapsuleUp = firstCastOrigin + Vector3.up * 0.5f;
        Vector3 firstCapsuleDown = firstCastOrigin - Vector3.up * 0.5f;

        RaycastHit hit;

        if (!Physics.CheckCapsule(firstCapsuleUp, firstCapsuleDown, 0.5f, mask))
        {
            if (Physics.CapsuleCast(firstCapsuleUp, firstCapsuleDown, 0.5f, Vector3.down, out hit, 2f))
            {
                Vector3 newPos = firstCastOrigin + Vector3.down * hit.distance;
                rb.MovePosition(newPos);
            }
        }
    }
    
    private void OnDisable()
    {
        playerInput.Player.Move.performed -= ReadMovement;
        playerInput.Player.Move.canceled -= ResetMovement;
    }
}