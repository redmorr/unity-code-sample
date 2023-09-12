using System;
using NaughtyAttributes;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] [Range(0f, 90f)] private float maxGroundAngle = 25f;
    [SerializeField] private float maxFallVelocity;
    [SerializeField] private AudioSource landingSound;
    [SerializeField] private ParticleSystem landingPuff;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private bool grounded;
    [SerializeField] [ReadOnly] private int skipSteps;
    [SerializeField] [ReadOnly] private float minGroundDotProduct;
    [SerializeField] [ReadOnly] private int stepsSinceLastGrounded;
    [SerializeField] [ReadOnly] private int groundContactCount;
    [SerializeField] [ReadOnly] private Vector3 contactNormal;
    [SerializeField] [ReadOnly] private Vector3 tempContactNormal;

    private Rigidbody rb;

    public Action OnGrounded;

    public Vector3 ContactNormal => contactNormal;
    public float MinGroundDotProduct => minGroundDotProduct;
    public bool Grounded => grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempContactNormal = Vector3.up;
        contactNormal = Vector3.up;
        groundContactCount = 0;
    }

    private void OnValidate() => minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

    public void Unground()
    {
        grounded = false;
        tempContactNormal = Vector3.up;
        contactNormal = Vector3.up;
        groundContactCount = 0;
        skipSteps = 15;
    }

    private void FixedUpdate()
    {
        if (skipSteps > 0) skipSteps--;
        stepsSinceLastGrounded++;

        if (groundContactCount > 0)
        {
            grounded = true;
            if (stepsSinceLastGrounded > 3) Ground();
            stepsSinceLastGrounded = 0;
        }
        else
        {
            maxFallVelocity = Mathf.Min(maxFallVelocity, rb.velocity.y);
            grounded = false;
            tempContactNormal = Vector3.up;
        }

        ClearState();
    }

    private void ClearState()
    {
        contactNormal = tempContactNormal;
        tempContactNormal = Vector3.zero;
        groundContactCount = 0;
    }

    private void Ground()
    {
        if (maxFallVelocity < -20f)
        {
            landingSound.Play();
            landingPuff.Play();
        }

        maxFallVelocity = 0;
        OnGrounded?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        if (skipSteps > 0) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                tempContactNormal += normal;
            }
        }
    }
}