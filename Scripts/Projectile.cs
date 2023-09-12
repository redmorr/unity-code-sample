using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class DummyProjectile : MonoBehaviour
{
    public float Speed;
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private ParticleSystem part;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Speed * Time.deltaTime * transform.forward);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (colliderMask == (colliderMask | (1 << other.gameObject.layer)))
        {
            Debug.Log(other.gameObject.name);
            foreach (var contact in other.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.green, 3f);
            }

            var c = other.GetContact(0);
            Instantiate(part, c.point, Quaternion.LookRotation(-c.normal));
            enabled = false;
            rb.detectCollisions = false;
            Destroy(gameObject,1f);
        }
    }
}