using System;
using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;


public class AgentBase : MonoBehaviour, IDamagable
{
    [SerializeField] private LayerMask blocksVision;
    [SerializeField] private float maxHealth;
    
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AgentSoundData soundData;
    
    [Header("Debug")]
    [SerializeField] [ReadOnly] private bool dead;
    [SerializeField] [ReadOnly] private float health;
    [SerializeField] [ReadOnly] private bool grounded;
    [SerializeField] [ReadOnly] private int stepsSinceLastGrounded;
    
    public bool Dead => dead;
    public AudioSource AudioSource => audioSource;
    public AgentSoundData SoundData => soundData;
    
    public event Action OnDeath;
    public event Action OnDamaged;

    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Falling = Animator.StringToHash("Falling");
    
    private bool damagedThisFrame;
    private float cachedDamage;
    private Vector3 cachedKnockback;
    
    private Animator animator;
    private RichAI ai;
    private CapsuleCollider coll;
    private Rigidbody rb;
    
    private void Awake()
    {
        ai = GetComponent<RichAI>();
        animator = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        health = maxHealth;
    }

    private void Update()
    {
        ApplyDamage();
    }
    
    private void FixedUpdate()
    {
        stepsSinceLastGrounded += 1;

        if (grounded)
        {
            if (stepsSinceLastGrounded > 1)
            {
                animator.SetBool(Falling, false);
            }

            stepsSinceLastGrounded = 0;
        }
        else
        {
            if (stepsSinceLastGrounded > 200)
            {
                animator.SetBool(Falling, true);
            }
        }

        grounded = true;
    }

    public void DealDamage(float damage, Vector3 force)
    {
        damagedThisFrame = true;
        cachedDamage += damage;
        cachedKnockback += force;
    }
    
    private void ApplyDamage()
    {
        if (!damagedThisFrame) return;

        rb.AddForce(cachedKnockback * rb.mass, ForceMode.Impulse);
        health -= cachedDamage;
        cachedDamage = 0f;
        cachedKnockback = Vector3.zero;
        damagedThisFrame = false;
        
        if (dead) return;

        if (health > 0f)
        {
            audioSource.PlayOneShot(soundData.Hurt);
            OnDamaged?.Invoke();
        }
        else
        {
            HandleDeath();
            OnDeath?.Invoke();
        }
    }
    
    private void HandleDeath()
    {
        dead = true;
        Physics.IgnoreCollision(coll, Game.PlayerCollider);
        ai.canMove = false;
        enabled = false;
        animator.SetTrigger(Die);
        audioSource.PlayOneShot(soundData.Die);

        coll.radius = 0.2f;
        coll.direction = 2;
        coll.center = new Vector3(0f, 0.2f, 0f);
    }
    

    
    private void LateUpdate()
    {
        animator.SetFloat(Speed, ai.velocity.sqrMagnitude);
    }
    
    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = Game.PlayerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, 100f, blocksVision))
        {
            return hit.collider.gameObject.layer == Constants.PlayerHitbox;
        }

        return false;
    }

    public bool CheckPlayerDetected()
    {
        Vector3 dirToPlayer = Game.PlayerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, 100f,
                blocksVision))
        {
            if (hit.collider.gameObject.layer == Constants.PlayerHitbox &&
                Vector3.Dot(transform.forward, dirToPlayer) > 0f)
            {
                return true;
            }
        }

        return false;
    }
    
    private void Footstep()
    {
        footstepAudio.PlayOneShot(soundData.Footsteps[Random.Range(0, soundData.Footsteps.Length)]);
    }
}