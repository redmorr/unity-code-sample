using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(GroundCheck))]
public class PlayerController : MonoBehaviour
{
    private enum MovementType
    {
        None,
        Quake,
        Forces
    }

    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform fpsCamera;
    [SerializeField] private float gravity;
    
    [Header("Jump")]
    [SerializeField] private bool autoHopEnabled;
    [SerializeField][ReadOnly] private bool jumpQueued;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float coyoteDuration;
    [SerializeField] private float jumpBufferDuration;
    [SerializeField][ReadOnly] private float jumpCooldown;
    [SerializeField] private float sweepDistance;

    [Header("Movement")]
    [SerializeField] private MovementType groundMovementType;
    [SerializeField] private MovementType airMovementType;
    [SerializeField] private QuakeMovement quakeMovement;
    [SerializeField] private ForcesMovement forcesMovement;
    [SerializeField] private PlayerSlide playerSlide;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugGizmos;
    [SerializeField][ReadOnly] private Vector2 move;
    [SerializeField][ReadOnly] private Vector3 velocity;
    [SerializeField][ReadOnly] private Vector3 groundVelocity;
    [SerializeField][ReadOnly] private Vector3 wishDir;
    [SerializeField][ReadOnly] private Vector3 wishVel;
    [SerializeField][ReadOnly] private float jumpBufferTimer;
    [SerializeField][ReadOnly] private float coyoteTimer;

    private CapsuleCollider coll;
    private CameraController cc;
    private GroundCheck gc;
    private Rigidbody rb;

    private void Awake()
    {
        cc = GetComponentInChildren<CameraController>();
        rb = GetComponent<Rigidbody>();
        gc = GetComponent<GroundCheck>();
        coll = GetComponent<CapsuleCollider>();
        quakeMovement.Setup(orientation, rb, gc);
        forcesMovement.Setup(orientation, rb, gc);

        playerSlide.Setup(cc, rb, coll, gc);

        gc.OnGrounded = CheckSlideDesired;
    }

    private void CheckSlideDesired()
    {
        if (input.Player.Slide.IsPressed())
        {
            Vector3 direction = wishDir != Vector3.zero ? wishDir : groundVelocity.normalized;
            playerSlide.Slide(direction);
        }
    }

    private void OnEnable()
    {
        input.Player.Move.performed += ReadMovement;
        input.Player.Move.canceled += ResetMovement;
        input.Player.Jump.performed += Jump;
    }

    
    private void Jump(InputAction.CallbackContext ctx) => jumpBufferTimer = jumpBufferDuration;
    private void ReadMovement(InputAction.CallbackContext ctx) => move = ctx.ReadValue<Vector2>();
    private void ResetMovement(InputAction.CallbackContext ctx) => move = Vector2.zero;

    private void InputUpdate()
    {
        if (input.Player.Slide.WasPerformedThisFrame())
        {
            Vector3 direction = groundVelocity.normalized;
            playerSlide.Slide(direction);
        }
    }
    
    private void Update()
    {
        InputUpdate();
        playerSlide.SliderUpdate();
        
        coyoteTimer = gc.Grounded ? coyoteDuration : Mathf.MoveTowards(coyoteTimer, 0f, Time.deltaTime);
        jumpBufferTimer = Mathf.MoveTowards(jumpBufferTimer, 0f, Time.deltaTime);
        jumpCooldown = Mathf.MoveTowards(jumpCooldown, 0f, Time.deltaTime);
        jumpQueued = (autoHopEnabled && input.Player.Jump.IsPressed()) || jumpBufferTimer > 0f;
        
        bool effectivelyGrounded = gc.Grounded || AlmostGrounded() || coyoteTimer > 0f;
        if (jumpQueued && effectivelyGrounded && jumpCooldown <= 0) Jump();
    }

    private bool AlmostGrounded()
    {
        return rb.SweepTest(Vector3.down, out RaycastHit hit, sweepDistance) && hit.normal.y > gc.MinGroundDotProduct;   
    }
    
    private void Jump()
    {
        jumpCooldown = 0.1f;
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        if (playerSlide.Sliding) playerSlide.StopSliding();
        gc.Unground();
        rb.AddForce(jumpHeight * Vector3.up, ForceMode.Impulse);
    }
    
    private void UpdateState()
    {
        velocity = rb.velocity;
        velocity.y = 0f;
        groundVelocity = Vector3.ProjectOnPlane(velocity, gc.ContactNormal);
        wishDir = orientation.forward * move.y + orientation.right * move.x;
        wishDir = Vector3.ProjectOnPlane(wishDir, gc.ContactNormal);
    }

    private void FixedUpdate()
    {
        UpdateState();

        if (!playerSlide.Sliding)
        {
            if (gc.Grounded && !jumpQueued)
            {
                switch (groundMovementType)
                {
                    case MovementType.None:
                        break;
                    case MovementType.Quake:
                        quakeMovement.MoveGround(move);
                        break;
                    case MovementType.Forces:
                        forcesMovement.MoveGround(move);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (airMovementType)
                {
                    case MovementType.None:
                        break;
                    case MovementType.Quake:
                        quakeMovement.MoveAir(move);
                        break;
                    case MovementType.Forces:
                        forcesMovement.MoveAir(move);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        rb.AddForce(-gc.ContactNormal * gravity);
    }
    
    private void OnDisable()
    {
        input.Player.Move.performed -= ReadMovement;
        input.Player.Move.canceled -= ResetMovement;
        input.Player.Jump.performed -= Jump;
    }
    
    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos || Camera.current != Camera.main) return;
        
        Vector3 center = fpsCamera.position + fpsCamera.forward;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(center, wishVel * 0.01f);

        if (rb)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(center, velocity * 0.01f);
        }
        
        forcesMovement.DrawGizmos(fpsCamera);
    }
}