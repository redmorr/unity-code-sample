using NaughtyAttributes;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AgentBase))]
[RequireComponent(typeof(RichAI))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class AgentMelee : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip attackHitSound;
    [SerializeField] private AudioClip attackSound;
    
    [Header("Debug")]
    [SerializeField] [ReadOnly] private bool playerSpotted;
    [SerializeField] [ReadOnly] private bool attacking;
    [SerializeField] [ReadOnly] private float attackDuration;
    [SerializeField] [ReadOnly] private float lastAttack;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Swing = Animator.StringToHash("Swing");

    private AgentBase agent;
    private RichAI ai;
    private Animator animator;
    private MeleeZone meleeZone;

    private void Awake()
    {
        agent = GetComponent<AgentBase>();
        meleeZone = GetComponentInChildren<MeleeZone>();
        animator = GetComponent<Animator>();
        ai = GetComponent<RichAI>();
        agent.OnDamaged += HandleDamage;
    }
    
    private void HandleDamage()
    {
        playerSpotted = true;
    }
    
    private void Update()
    {
        if (agent.Dead) return;

        if (!playerSpotted)
        {
            playerSpotted = agent.CheckPlayerDetected();
            return;
        }
        
        if (meleeZone.PlayerInRange && !attacking)
        {
            attacking = true;
            ai.isStopped = true;
            animator.SetTrigger(Swing);
            agent.AudioSource.PlayOneShot(attackSound);
            lastAttack = Time.time;
        }

        if (attacking)
        {
            Vector3 lookPosition = Game.PlayerTransform.position;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);
            if (Time.time > lastAttack + attackDuration)
            {
                attacking = false;
                ai.isStopped = false;
            }
        }

        if (Game.PlayerTransform != null && !attacking &&
            Vector3.Distance(transform.position, Game.PlayerTransform.position) > ai.endReachedDistance)
        {
            ai.destination = Game.PlayerTransform.position;
        }
    }
    
    private void LateUpdate()
    {
        animator.SetFloat(Speed, ai.velocity.sqrMagnitude);
    }

    private void Attack()
    {
        if (agent.Dead && meleeZone.playerHealth)
        {
            agent.AudioSource.PlayOneShot(attackHitSound);
            meleeZone.playerHealth.DealDamage(10f, Vector3.zero);
        }
    }
}