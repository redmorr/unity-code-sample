using UnityEngine;

public class WeaponCamera : MonoBehaviour
{
    [SerializeField] private Transform weaponCamera;
    [SerializeField] private float maxOffsetDistance;
    [SerializeField] private float offsetChangeSpeed;
    [SerializeField] private float velocityMultiplier;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GameObject.FindWithTag(Constants.PlayerTag).GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        OffsetWeaponCamera();
    }
    
    private void OffsetWeaponCamera()
    {
        Vector3 weaponCameraOffset = Vector3.ClampMagnitude(weaponCamera.InverseTransformDirection(rb.velocity) * velocityMultiplier, maxOffsetDistance);
        float distanceToOffset = Vector3.Distance(weaponCameraOffset, weaponCamera.localPosition);
        weaponCamera.localPosition = Vector3.MoveTowards(weaponCamera.localPosition, weaponCameraOffset, offsetChangeSpeed * distanceToOffset * Time.deltaTime);
    }
}