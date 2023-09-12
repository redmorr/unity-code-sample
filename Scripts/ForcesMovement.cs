using System;
using UnityEngine;

[Serializable]
public class ForcesMovement
{
    [Header("Ground")]
    public bool useDotAccel;
    public float startAccel;
    public float endAccel;
    public float decel;
    public float maxSpeed;
    public float extraMaxSpeed;

    [Header("Air")]
    public float startAccelAir;
    public float endAccelAir;
    public float startDecelAir;
    public float endDecelAir;

    [Header("Debug")]
    public bool enableDebugGizmos;

    public Vector3 velocity;
    public Vector3 wishDir;
    public float accelSpeed;
    public float decelSpeed;
    public Vector3 accelForce;
    public Vector3 decelForce;
    public Vector3 breakForce;

    private Transform orientation;
    private Rigidbody rb;
    private GroundCheck gc;

    public void Setup(Transform orientation, Rigidbody rigidbody, GroundCheck groundCheck)
    {
        this.orientation = orientation;
        rb = rigidbody;
        gc = groundCheck;
    }

    public void MoveGround(Vector2 move)
    {
        UpdateState(move);

        if (move.sqrMagnitude > 0.25f)
        {
            float projectedSpeed = Vector3.Dot(velocity, wishDir);
            float speed = velocity.magnitude;

            accelSpeed = useDotAccel ? projectedSpeed : speed;
            float accelFactor = accelSpeed / (maxSpeed + extraMaxSpeed);
            
            accelForce = wishDir * Mathf.Lerp(startAccel, endAccel, accelFactor * accelFactor);
            decelForce = -velocity * decel;

            rb.AddForce(accelForce);
            rb.AddForce(decelForce);
        }
        else if (gc.Grounded && velocity.sqrMagnitude != 0f)
        {
            rb.AddForce(-velocity * 30f);
        }
    }

    public void MoveAir(Vector2 move)
    {
        UpdateState(move);

        if (move.sqrMagnitude > 0.25f)
        {
            float projectedSpeed = Vector3.Dot(velocity, wishDir);
            float speed = velocity.magnitude;

            accelSpeed = useDotAccel ? projectedSpeed : speed;
            decelSpeed = speed;

            accelForce = wishDir * Mathf.Lerp(startAccelAir, endAccelAir, accelSpeed / (maxSpeed + extraMaxSpeed));
            decelForce = velocity.normalized *
                         Mathf.Lerp(startDecelAir, endDecelAir, decelSpeed / (maxSpeed + extraMaxSpeed));

            rb.AddForce(accelForce);
            rb.AddForce(decelForce);
        }
    }

    private void UpdateState(Vector2 move)
    {
        velocity = rb.velocity;
        velocity.y = 0f;
        velocity = Vector3.ProjectOnPlane(velocity, gc.ContactNormal);
        wishDir = orientation.forward * move.y + orientation.right * move.x;
        wishDir = Vector3.ProjectOnPlane(wishDir, gc.ContactNormal);
    }

    public void DrawGizmos(Transform fpsCamera)
    {
        if (!enableDebugGizmos || Camera.current != Camera.main) return;
        Vector3 center = fpsCamera.position + fpsCamera.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(center, accelForce * 0.001f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, decelForce * 0.001f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, breakForce * 0.001f);
    }
}