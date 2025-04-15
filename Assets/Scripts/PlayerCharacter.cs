using UI;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : MonoBehaviour, IDamageable
{
    private static readonly int WalkingAnim = Animator.StringToHash("Walking");
    private static readonly int IsAimingAnim = Animator.StringToHash("IsAiming");
    private static readonly int ShootAnim = Animator.StringToHash("Shoot");
    private static readonly int DeathAnim = Animator.StringToHash("Death");
    private static readonly int GetHitAnim = Animator.StringToHash("GetHit");


    private Animator _animator;

    private CharacterController _characterController;

    private AudioSource _footstepAudioSource;


    [Header("Movement")] [SerializeField] private float movementSpeed;

    [SerializeField] private float rotationSpeed;

    [Header("Health")] [SerializeField] private int maxHealth = 5;

    [SerializeField] private int health = 5;

    [Header("Combat")] [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private GameObject projectilePlaceholder;

    [SerializeField] private Transform arrowProjectileSpawnPoint;

    [Header("HUD")] [SerializeField] private HUD hud;

    [SerializeField] private AudioClip[] footstepClips;
    [Header("SFX")] [SerializeField] private Vector2 pitchRange = new(0.85f, 1.15f);

    [Header("SFX | Attack")] [SerializeField]
    private AudioSource attackAudioSource;

    [SerializeField] private AudioClip[] shootClips;

    [Header("SFX | Death")] [SerializeField]
    private AudioClip[] deathClips;

    [Header("SFX | Hit")] [SerializeField] private AudioClip[] hitClips;


    public Vector3 InputDirection { get; set; } = Vector3.zero;

    private Vector3 _moveDirection = Vector3.zero;

    private bool _isAiming = false;

    private bool _isDead = false;

    private Vector3 _aimDirection = Vector3.zero;

    public float stepRate = 0.6f;

    private float _stepTimer;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _footstepAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        hud.UpdateHealth((float)health / maxHealth);
    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }

        Movement();

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

    private void Movement()
    {
        _moveDirection = InputDirection * (movementSpeed * Time.deltaTime);

        if (_moveDirection.sqrMagnitude > float.Epsilon)
        {
            _characterController.Move(_moveDirection);

            _animator.SetBool(WalkingAnim, true);

            _stepTimer -= Time.deltaTime;
            if (!(_stepTimer <= 0f))
            {
                return;
            }

            PlayFootstepAudio();
            _stepTimer = stepRate;
        }
        else
        {
            StopMovement();
            _stepTimer = 0f;
        }
    }

    private void PlayFootstepAudio()
    {
        if (footstepClips.Length == 0)
        {
            return;
        }

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        _footstepAudioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        _footstepAudioSource.PlayOneShot(clip);
    }

    private void StopMovement()
    {
        _animator.SetBool(WalkingAnim, false);
    }

    public void StartAim()
    {
        _isAiming = true;

        StopMovement();

        projectilePlaceholder.SetActive(true);
        _animator.SetBool(IsAimingAnim, true);
    }

    public void EndAim()
    {
        _isAiming = false;

        projectilePlaceholder.SetActive(false);
        _animator.SetBool(IsAimingAnim, false);
    }

    public void Shoot()
    {
        if (!_isAiming)
        {
            return;
        }

        _animator.SetTrigger(ShootAnim);

        PlayBowAudio();

        Instantiate(projectilePrefab, arrowProjectileSpawnPoint.position, arrowProjectileSpawnPoint.rotation);
    }

    private void PlayBowAudio()
    {
        if (shootClips.Length == 0)
        {
            return;
        }

        var clip = shootClips[Random.Range(0, shootClips.Length)];
        attackAudioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        attackAudioSource.PlayOneShot(clip);
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

        if (_aimDirection.sqrMagnitude > float.Epsilon)
        {
            var targetRotation = Quaternion.LookRotation(_aimDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        hud.UpdateHealth((float)health / maxHealth);

        if (health <= 0)
        {
            _isDead = true;
            PlayDeathAudio();
            _animator.SetBool(DeathAnim, true);
            _characterController.enabled = false;
            hud.Death();
            return;
        }

        PlayHitAudio();
        _animator.SetTrigger(GetHitAnim);
    }

    private void PlayDeathAudio()
    {
        if (deathClips.Length == 0)
        {
            return;
        }

        var clip = deathClips[Random.Range(0, deathClips.Length)];
        attackAudioSource.pitch = 1f; // Restore default pitch
        attackAudioSource.PlayOneShot(clip);
    }

    private void PlayHitAudio()
    {
        if (deathClips.Length == 0)
        {
            return;
        }

        var clip = hitClips[Random.Range(0, hitClips.Length)];
        attackAudioSource.pitch = 1f; // Restore default pitch
        attackAudioSource.PlayOneShot(clip);
    }
}