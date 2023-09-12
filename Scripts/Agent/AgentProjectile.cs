using System;
using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AgentBase))]
[RequireComponent(typeof(RichAI))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class AgentProjectile : MonoBehaviour
{
    private enum State
    {
        Idle,
        ChaseEnter,
        Chase,
        AimEnter,
        Aim,
        AimObstructed,
        StunEnter,
        Stun
    }
    
    [FormerlySerializedAs("projectilePrefab")]
    [Header("Projectile")]
    [SerializeField] private DummyProjectile dummyProjectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private ParticleSystem gunshotParticle;
    [SerializeField] private float speed;
    [SerializeField] private State state;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private float chaseCooldown;

    private static readonly int TakeAim = Animator.StringToHash("TakeAim");
    private static readonly int TakeAimReverse = Animator.StringToHash("TakeAimReverse");
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    
    private AgentBase agent;
    private RichAI ai;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<AgentBase>();
        animator = GetComponent<Animator>();
        ai = GetComponent<RichAI>();

        agent.OnDamaged += HandleDamage;
    }
    
    private void HandleDamage()
    {
        state = State.StunEnter;
    }
    
    private void Update()
    {
        if (agent.Dead) return;

        switch (state)
        {
            case State.Idle:
                if (agent.CheckPlayerDetected())
                {
                    agent.AudioSource.PlayOneShot(agent.SoundData.Spawn);
                    state = State.ChaseEnter;
                }
                break;

            case State.StunEnter:
                ai.isStopped = true;
                animator.Play(TakeDamage, -1, 0f);
                state = State.Stun;
                break;

            case State.Stun:
                break;

            case State.ChaseEnter:
                ai.isStopped = false;
                chaseCooldown = 2f;

                state = agent.CanSeePlayer() ? State.AimEnter : State.Chase;
                break;

            case State.Chase:
                chaseCooldown = Mathf.MoveTowards(chaseCooldown, 0f, Time.deltaTime);

                if (Game.PlayerTransform != null && Vector3.Distance(transform.position, Game.PlayerTransform.position) > ai.endReachedDistance)
                {
                    ai.destination = Game.PlayerTransform.position;
                }

                if (chaseCooldown <= 0f && agent.CanSeePlayer()) state = State.AimEnter;
                break;

            case State.AimEnter:
                ai.isStopped = true;
                animator.Play(TakeAim);
                LookAtPlayer();
                state = State.Aim;
                break;

            case State.Aim:
                LookAtPlayer();
                if (!agent.CanSeePlayer())
                {
                    float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    animator.Play(TakeAimReverse, -1, 1f - normalizedTime);
                    state = State.AimObstructed;
                }

                break;
            case State.AimObstructed:
                break;

            default:
                Debug.LogError("Unhandled case in switch!");
                break;
        }
    }
    
    private void LookAtPlayer()
    {
        Vector3 lookPosition = Game.PlayerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);
    }
    
    private void SpawnProjectile()
    {
        gunshotParticle.Play();
        DummyProjectile proj = Instantiate(dummyProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        proj.Speed = speed;
    }
    
    private static class AnimCode
    {
        public const int Default = 0;
        public const int Shoot = 1;
        public const int ShootEnd = 3;
        public const int DrawBegin = 4;
        public const int StunEnd = 2;
    }
    
    public void AnimationEvent(int code)
    {
        switch (code)
        {
            case AnimCode.Default:
                Debug.LogError("Default code returned");
                break;
            case AnimCode.Shoot:
                SpawnProjectile();
                break;
            case AnimCode.StunEnd:
                state = State.ChaseEnter;
                break;
            case AnimCode.ShootEnd:
                state = State.ChaseEnter;
                break;
            case AnimCode.DrawBegin:
                if (state == State.AimObstructed) state = state = State.ChaseEnter;
                break;
            default:
                Debug.LogError("Unhandled animation code");
                break;
        }
    }
    
    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 130, 100),
            "AI State: " + Enum.GetName(typeof(State), state) + "\n");
    }
}