using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowProjectile : MonoBehaviour
{
    private Rigidbody _rb;

    private Collider _collider;


    public float launchForce = 30f;

    public float lifeTime = 5f;

    public int damage = 1;


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
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
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