using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowProjectile : MonoBehaviour
{
    private Rigidbody _rb;

    private Collider _collider;

    [SerializeField] private float launchForce = 30f;

    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private int damage = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _rb.linearVelocity = transform.forward * launchForce;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > float.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _collider.enabled = false;

        _rb.isKinematic = true;

        var damageable = other.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);

        Destroy(gameObject);
    }
}