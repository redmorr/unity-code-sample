using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;


public class Hitscan : Weapon
{
    [Header("Hitscan")]
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private Transform fpsCamera;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float damage;
    [SerializeField] private float force;
    [SerializeField] private int pellets;
    [SerializeField] private float spreadAngle;
    [SerializeField] private ParticleSystem gunfireParticle;
    [SerializeField] private ParticleSystem impactParticlePrefab;
    [SerializeField] private ParticleSystem fleshParticlePrefab;
    [SerializeField] private ParticleSystem shellEjectionParticle;
    [SerializeField] private GameObject bulletHoleDecalPrefab;
    [SerializeField] private AudioClip gunshotClip;

    [Header("Debug")]
    [SerializeField] private bool enableDebugGizmos;

    [SerializeField] [ReadOnly] private float lastShotTime = Mathf.NegativeInfinity;

    private static readonly int ShotgunIdle = Animator.StringToHash("ShotgunIdle");
    private static readonly int ShotgunShoot = Animator.StringToHash("ShotgunShoot");
    
    private Animator animator;
    private CameraController cc;
    private WeaponSwitch weaponSwitch;
    private IDamagable[] damagablesCache;
    private Collider[] collidersCache;
    private int hitsThisFrame;

    private void OnEnable()
    {
        animator.Play(ShotgunIdle, -1, 1f);
    }

    private void Awake()
    {
        cc = GetComponentInParent<CameraController>();
        animator = GetComponent<Animator>();
        weaponSwitch = GetComponentInParent<WeaponSwitch>();

        damagablesCache = new IDamagable[pellets];
        collidersCache = new Collider[pellets];
    }
    
    private void Update()
    {
        if (playerInput.Player.Shoot.IsPressed() && weaponSwitch.Ready && Time.time >= lastShotTime + timeBetweenShots)
        {
            lastShotTime = Time.time;
            cc.Shake();
            gunfireParticle.Play();
            Game.Sounds.PlayClip(gunshotClip);
            animator.Play(ShotgunShoot, -1, 0f);

            for (int i = 0; i < pellets; i++)
            {
                Vector3 shotDirection = Vector3.Slerp(fpsCamera.forward, Random.insideUnitSphere, spreadAngle / 180f);
                RaycastHit hit;
                if (Physics.Raycast(fpsCamera.position, shotDirection, out hit, 9999f, raycastMask))
                {
                    var impactForward = Quaternion.LookRotation(-hit.normal);
                    var impactPoint = hit.point + 0.01f * hit.normal;

                    bool cachedFound = false;

                    for (int j = 0; j < hitsThisFrame; j++)
                    {
                        if (hit.collider == collidersCache[j])
                        {
                            //Debug.Log("Found");
                            damagablesCache[j].DealDamage(damage, shotDirection * force);
                            cachedFound = true;
                            break;
                        }
                    }

                    if (!cachedFound)
                    {
                        //Debug.Log("0");
                        if (hit.collider.TryGetComponent(out IDamagable damagable))
                        {
                            collidersCache[hitsThisFrame] = hit.collider;
                            damagablesCache[hitsThisFrame] = damagable;
                            hitsThisFrame++;
                            damagable.DealDamage(damage, shotDirection * force);
                            Instantiate(fleshParticlePrefab, impactPoint, impactForward);
                        }
                        else
                        {
                            Instantiate(impactParticlePrefab, impactPoint, impactForward);
                            Instantiate(bulletHoleDecalPrefab, impactPoint, impactForward);
                        }
                    }
                }
            }

            hitsThisFrame = 0;
        }
    }

    public void EjectShell()
    {
        shellEjectionParticle.Play();
    }

    private void OnDrawGizmosSelected()
    {
        if (!enableDebugGizmos || Camera.current != Camera.main) return;

        if (Physics.Raycast(fpsCamera.position, fpsCamera.forward, out RaycastHit hit))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(fpsCamera.position, hit.point);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(fpsCamera.position, fpsCamera.forward * 1000f);
        }
    }
}