using System;
using UnityEngine;

[Serializable]
public class PlayerSlide
{
    [SerializeField] private Vector3 cameraPos;
    [SerializeField] private float slidingDuration;
    [SerializeField] private CameraController cc;
    [SerializeField] private ParticleSystem slidingSparks;
    [SerializeField] private ParticleSystem speedStreaks;
    [SerializeField] private AudioSource slidingSound;

    [Header("Debug")]
    [SerializeField] private bool sliding;
    [SerializeField] private float slideTimer;

    private Rigidbody rb;
    private CapsuleCollider coll;
    private GroundCheck gc;
    
    public bool Sliding => sliding;

    public void Setup(CameraController cameraController, Rigidbody rigidbody, CapsuleCollider capsuleCollider, GroundCheck groundCheck)
    {
        cc = cameraController;
        rb = rigidbody;
        coll = capsuleCollider;
        gc = groundCheck;
    }

    public void Slide(Vector3 direction)
    {
        if (gc.Grounded && !sliding)
        {
            slideTimer = slidingDuration;
            sliding = true;
            coll.height = 1f;
            coll.center = new Vector3(0f, -0.5f, 0f);
            cc.ChangeHeadPosition(cameraPos);
            slidingSparks.Play();
            speedStreaks.Play();
            slidingSound.Play();
            speedStreaks.transform.localRotation = Quaternion.LookRotation(direction);
            slidingSparks.transform.localRotation = Quaternion.LookRotation(direction);
            float magnitude = rb.velocity.magnitude;
            float boost = 1f - Mathf.Clamp01((magnitude - 24f) / 10f);
            
            rb.velocity = direction * rb.velocity.magnitude;
            rb.AddForce(direction * boost, ForceMode.Impulse);
        }
    }
    
    public void StopSliding()
    {
        slideTimer = 0f;
        sliding = false;
        coll.height = 2f;
        coll.center = Vector3.zero;
        cc.ResetHeadPosition();
        slidingSparks.Stop();
        speedStreaks.Stop();
        slidingSound.Stop();
    }

    public void SliderUpdate()
    {
        slideTimer = Mathf.MoveTowards(slideTimer, 0f, Time.deltaTime);

        if (slideTimer <= 0 && sliding)
        {
            sliding = false;
            coll.height = 2f;
            coll.center = Vector3.zero;
            cc.ResetHeadPosition();
            slidingSparks.Stop();
            speedStreaks.Stop();
            slidingSound.Stop();
        }
    }
}