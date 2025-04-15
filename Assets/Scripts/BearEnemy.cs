using System;
using System.Collections;
using AI;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class BearEnemy : MonoBehaviour, IDamageable
{
    private static readonly int GetHitFrontAnim = Animator.StringToHash("Get Hit Front");
    private static readonly int StunnedLoopAnim = Animator.StringToHash("Stunned Loop");
    private static readonly int WalkForwardAnim = Animator.StringToHash("WalkForward");
    private static readonly int DeathAnim = Animator.StringToHash("Death");

    private readonly string[] _attackAnimationNames = { "Attack1", "Attack2", "Attack3", "Attack5", };

    public event Action<BearEnemy> OnBearDied;

    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    private StateMachine _stateMachine;

    [Header("VFX")] [SerializeField] Renderer meshRenderer;

    [SerializeField] private float dissolveEffectTime = 10.0f;

    [Header("Combat")] [SerializeField] private float timeStunned = 1.5f;

    [Header("Movement")] [SerializeField] private float movementSpeed = 2.5f;

    [SerializeField] private float angularRotationSpeed = 5.0f;

    [Header("Attack")] [SerializeField] private float attackDistance = 3.0f;

    [SerializeField] private float attackCooldownTime = 2.0f;

    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 5.0f;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private int damageDone = 1;

    [Header("Health")] [SerializeField] private int maxHealth = 5;

    [SerializeField] private int health = 5;

    [SerializeField] private ResourceBar healthBar;

    private Coroutine _stunRoutine;

    [Header("SFX | Attack")] [SerializeField]
    private AudioClip[] attackClips;

    [SerializeField] private Vector2 attackPitchRange = new(0.85f, 1.15f);

    [Header("SFX | Death")] [SerializeField]
    private AudioClip[] deathClips;

    [SerializeField] private Vector2 deathPitchRange = new(0.35f, 0.55f);

    [Header("SFX | Hit")] [SerializeField] private AudioClip[] hitClips;

    [SerializeField] private Vector2 hitPitchRange = new(0.6f, 0.7f);

    private AudioSource _audioSource;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (!playerGameObject)
        {
            Debug.LogError("Player not found!");
            return;
        }

        _navMeshAgent.speed = movementSpeed;
        _navMeshAgent.angularSpeed = angularRotationSpeed;
        _navMeshAgent.stoppingDistance = attackDistance;

        healthBar.SetPercentValue((float)health / maxHealth);

        var context = new StateContext
        {
            AttackDistance = attackDistance,
            Owner = this,
            Player = playerGameObject.GetComponent<PlayerCharacter>(),
            AttackCooldownTime = attackCooldownTime,
        };

        _stateMachine = new StateMachine();
        _stateMachine.Initialize(new ChaseState(_stateMachine, context));
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0)
        {
            return;
        }

        health -= damage;

        healthBar.SetPercentValue((float)health / maxHealth);

        if (health <= 0)
        {
            StopAllCoroutines();

            StopStunned();
            StopMovement();

            _stateMachine.Enabled = false;

            StartCoroutine(StartDeath());

            return;
        }

        _animator.SetTrigger(GetHitFrontAnim);

        PlayHitSound();

        _stateMachine.Enabled = false;
        StopMovement();

        _animator.SetBool(StunnedLoopAnim, true);

        _stunRoutine = StartCoroutine(StunCoroutine());
    }

    private IEnumerator StartDeath()
    {
        _animator.SetBool(DeathAnim, true);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            yield return null;
        }

        PlayDeathSound();
        OnBearDied?.Invoke(this);

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .85f)
        {
            yield return null;
        }

        FinishDeath();
    }

    private void FinishDeath()
    {
        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScale(0.01f, dissolveEffectTime).SetEase(Ease.InBack));
        seq.Join(meshRenderer.material.DOFloat(-0.6f, "_DissolveAmount", dissolveEffectTime));
        seq.AppendCallback(() => Destroy(gameObject));
    }

    private IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(timeStunned);

        StopStunned();
    }

    private void StopStunned()
    {
        _animator.SetBool(StunnedLoopAnim, false);
        _stateMachine.Enabled = true;
        _stunRoutine = null;
    }

    public void MoveTo(Vector3 position)
    {
        _animator.SetBool(WalkForwardAnim, true);

        _navMeshAgent.destination = position;
        _navMeshAgent.isStopped = false;

        //  TODO: Not working...
        if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        _animator.SetBool(WalkForwardAnim, false);

        _navMeshAgent.isStopped = true;
    }

    public void Attack(Vector3 direction)
    {
        var attackDirection = Quaternion.LookRotation(direction);
        gameObject.transform.rotation = attackDirection;

        int index = Random.Range(0, _attackAnimationNames.Length);
        _animator.SetTrigger(_attackAnimationNames[index]);

        PlayAttackSound();

        var colliders = new Collider[5];
        Physics.OverlapSphereNonAlloc(attackPoint.position, attackRadius, colliders, playerLayer);
        foreach (var coll in colliders)
        {
            if (!coll)
            {
                continue;
            }

            var damageable = coll.GetComponent<IDamageable>();
            damageable?.TakeDamage(damageDone);
        }
    }

    private void PlayAttackSound()
    {
        if (attackClips.Length == 0)
        {
            return;
        }

        var clip = attackClips[Random.Range(0, attackClips.Length)];
        _audioSource.pitch = Random.Range(attackPitchRange.x, attackPitchRange.y);
        _audioSource.PlayOneShot(clip);
    }

    private void PlayDeathSound()
    {
        if (deathClips.Length == 0)
        {
            return;
        }

        var clip = deathClips[Random.Range(0, deathClips.Length)];
        _audioSource.pitch = Random.Range(deathPitchRange.x, deathPitchRange.y);
        _audioSource.PlayOneShot(clip);
    }

    private void PlayHitSound()
    {
        if (hitClips.Length == 0)
        {
            return;
        }

        var clip = hitClips[Random.Range(0, hitClips.Length)];
        _audioSource.pitch = Random.Range(hitPitchRange.x, hitPitchRange.y);
        _audioSource.PlayOneShot(clip);
    }
}