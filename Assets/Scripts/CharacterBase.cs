using UI;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterBase : MonoBehaviour, IDamageable
{
    private Animator _animator;

    private CharacterController _characterController;


    [SerializeField] private float movementSpeed;

    [SerializeField] private float rotationSpeed;

    [SerializeField] private float gravity = 20f;

    [SerializeField] private float jumpHeight = 2.5f;

    [SerializeField] private int maxHealth = 5;

    [SerializeField] private int health = 5;

    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private GameObject projectilePlaceholder;

    [SerializeField] private Transform arrowProjectileSpawnPoint;

    [SerializeField] private HUD hud;

    private float _yVelocity = 0f;

    public Vector3 InputDirection { get; set; } = Vector3.zero;

    private Vector3 _moveDirection = Vector3.zero;

    public bool ShouldJump { get; set; } = false;

    private bool _isAiming = false;

    private bool _isDead = false;

    private Vector3 _aimDirection = Vector3.zero;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        float healthPercent = (float)health / maxHealth;
        hud.UpdateHealth(healthPercent);
    }

    private void Update()
    {
        if (_isDead) return;

        _moveDirection = InputDirection * movementSpeed;
        _moveDirection *= Time.deltaTime;

        // Gravity
        if (_characterController.isGrounded)
        {
            if (ShouldJump)
            {
                ShouldJump = false;
                _yVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
                _animator.SetTrigger("Jump");
            }
        }
        else
        {
            _yVelocity -= gravity * Time.deltaTime;
        }

        _moveDirection.y = _yVelocity * Time.deltaTime;

        // CharacterController
        _characterController.Move(_moveDirection);

        // Animaciones
        _animator.SetFloat("Speed", new Vector3(_moveDirection.x, 0, _moveDirection.z).magnitude / Time.deltaTime);
        _animator.SetFloat("JumpSpeed", _moveDirection.y);

        if (_isAiming)
        {
            RotateTowardsCameraFacingDirection();
        }
        else
        {
            if (InputDirection != Vector3.zero)
            {
                RotateTowardsInputDirection();
            }
        }
    }

    public void StartAim()
    {
        _isAiming = true;

        projectilePlaceholder.SetActive(true);
        _animator.SetBool("IsAiming", true);
    }

    public void EndAim()
    {
        _isAiming = false;

        projectilePlaceholder.SetActive(false);
        _animator.SetBool("IsAiming", false);
    }

    public void Shoot()
    {
        if (!_isAiming)
        {
            return;
        }

        _animator.SetTrigger("Shoot");

        Instantiate(projectilePrefab, arrowProjectileSpawnPoint.position, arrowProjectileSpawnPoint.rotation);
    }

    private void RotateTowardsInputDirection()
    {
        var rotation = Quaternion.LookRotation(InputDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    private void RotateTowardsCameraFacingDirection()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        _aimDirection = ray.GetPoint(1000f) - transform.position;


        var spawnRotation = Quaternion.LookRotation(_aimDirection);
        arrowProjectileSpawnPoint.transform.rotation = spawnRotation;


        _aimDirection.y = 0f;

        if (_aimDirection.sqrMagnitude > 0.01f)
        {
            var targetRotation = Quaternion.LookRotation(_aimDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        float healthPercent = (float)health / maxHealth;
        hud.UpdateHealth(healthPercent);

        if (health <= 0)
        {
            _isDead = true;
            _animator.SetBool("Death", true);
            _characterController.enabled = false;
            hud.Death();
            return;
        }

        _animator.SetTrigger("GetHit");
    }
}