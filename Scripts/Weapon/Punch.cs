using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Punch : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform fpsCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float punchRange;
    [SerializeField] private float punchRadius;
    [SerializeField] private float force;
    [SerializeField] private int damage;
    [SerializeField] private AudioSource punchSwing;
    [SerializeField] private AudioSource punchHitMetal;
    [SerializeField] private AudioSource punchHitFlesh;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool enableDebugGizmos;

    private static readonly int KnifeStab = Animator.StringToHash("KnifeStab");

    private void OnEnable()
    {
        playerInput.Player.Punch.performed += PerformPunch;
    }

    private void PerformPunch(InputAction.CallbackContext context)
    {
        animator.Play(KnifeStab, 0, 0f);
        punchSwing.Play();
    }

    public void CheckPunchHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.position, fpsCamera.forward, out hit, punchRange, layerMask, QueryTriggerInteraction.Ignore)
            || Physics.SphereCast(fpsCamera.position, punchRadius, fpsCamera.forward, out hit, punchRange, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out IDamagable agentHealth))
            {
                agentHealth.DealDamage(damage, fpsCamera.forward * force);
                punchHitFlesh.Play();
            }
            else
            {
                punchHitMetal.Play();
            }

            Debug.DrawLine(fpsCamera.position, hit.point, Color.cyan, 5f);
        }
        else
        {
            Debug.DrawRay(fpsCamera.position, fpsCamera.forward * punchRange, Color.red, 5f);
        }
    }

    private void OnDisable()
    {
        playerInput.Player.Punch.performed -= PerformPunch;
    }

    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos || Camera.current != Camera.main) return;

        RaycastHit hitInfo;
        if (Physics.Raycast(fpsCamera.position, fpsCamera.forward, out hitInfo, punchRange, layerMask,
                QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
        else if (Physics.SphereCast(fpsCamera.position, punchRadius, fpsCamera.forward * punchRange, out hitInfo,
                     punchRange, layerMask, QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.green;
            Vector3 sphereCastMidpoint = fpsCamera.position + (fpsCamera.forward * hitInfo.distance);
            Gizmos.DrawWireSphere(sphereCastMidpoint, punchRadius);
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = fpsCamera.position + (fpsCamera.forward * (punchRange - punchRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, punchRadius);
            Debug.DrawLine(fpsCamera.position, sphereCastMidpoint, Color.red);
        }
    }
}