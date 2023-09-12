using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class QuakeMovement
{
    public float airAccelerate;
    public float friction;
    public float groundAccelerate;
    public float maxSpeed;
    public float maxAirSpeed;
    public float extraMaxSpeed;

    [Header("Debug")]
    [SerializeField][ReadOnly] public Vector3 wishDir;
    [SerializeField][ReadOnly] public Vector3 vel;
    [SerializeField][ReadOnly] public Vector3 horizontalVel;
    [SerializeField][ReadOnly] public Vector3 verticalVel;
    [SerializeField][ReadOnly] public float verticalSpeed;

    private Transform orientation;
    private Rigidbody rb;
    private GroundCheck gc;

    public void Setup(Transform orientation, Rigidbody rigidbody, GroundCheck groundCheck)
    {
        this.orientation = orientation;
        rb = rigidbody;
        gc = groundCheck;
    }
    
    private void UpdateState(Vector2 move)
    {
        vel = rb.velocity;
        verticalSpeed = vel.y;
        vel.y = 0f;
        
        horizontalVel = Vector3.ProjectOnPlane(vel, gc.ContactNormal);
        wishDir = orientation.forward * move.y + orientation.right * move.x;
        wishDir = Vector3.ProjectOnPlane(wishDir, gc.ContactNormal);
    }
    
    public void MoveGround(Vector2 move)
    {
        UpdateState(move);
        Vector3 newVelocity = MoveGround(wishDir, vel);
        newVelocity += verticalVel;
        rb.velocity = newVelocity;
    }
    
    public void MoveAir(Vector2 move)
    {
        UpdateState(move);
        Vector3 newVelocity = MoveAir(wishDir, vel);
        newVelocity += verticalVel;
        rb.velocity = newVelocity;
    }
    
    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        float speed = prevVelocity.magnitude;
        if (speed != 0) 
        {
            float drop = speed * friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }
        
        return Accelerate(accelDir, maxSpeed + extraMaxSpeed, prevVelocity, groundAccelerate);
    }

    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        return Accelerate(accelDir, maxAirSpeed, prevVelocity, airAccelerate);
    }
    
    private Vector3 Accelerate(Vector3 wishDir, float wishSpeed, Vector3 prevVelocity, float accelerate)
    {
        float currentSpeed = Vector3.Dot(prevVelocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0)
            return prevVelocity;

        float accelSpeed = accelerate * maxSpeed * Time.deltaTime;
        
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;
        
        return prevVelocity + wishDir * accelSpeed;
    }
}