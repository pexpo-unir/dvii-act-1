using System.Collections;
using AI;
using UI;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BearEnemy : MonoBehaviour, IDamageable
{
    private static readonly int GetHitFrontAnim = Animator.StringToHash("Get Hit Front");
    private static readonly int StunnedLoopAnim = Animator.StringToHash("Stunned Loop");
    private static readonly int WalkForwardAnim = Animator.StringToHash("WalkForward");

    private readonly string[] _attackAnimationNames = { "Attack1", "Attack2", "Attack3", "Attack5", };


    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    private StateMachine _stateMachine;


    [SerializeField] private float timeStunned = 1.5f;

    [SerializeField] private float movementSpeed = 2.5f;

    [SerializeField] private float angularRotationSpeed = 5.0f;

    [SerializeField] private float attackDistance = 3.0f;

    [SerializeField] private float attackCooldownTime = 2.0f;

    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 5.0f;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private int damage = 1;

    [SerializeField] private int maxHealth = 5;

    [SerializeField] private int health = 5;

    [SerializeField] private ResourceBar healthBar;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
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

        float healthPercent = (float)health / maxHealth;
        healthBar.SetPercentValue(healthPercent);

        var context = new StateContext
        {
            AttackDistance = attackDistance,
            Owner = this,
            Player = playerGameObject.GetComponent<CharacterBase>(),
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
        health -= damage;

        float healthPercent = (float)health / maxHealth;
        healthBar.SetPercentValue(healthPercent);

        if (health <= 0)
        {
            StopMovement();
            _animator.SetBool("Death", true);
            _stateMachine.Enabled = false;
            return;
        }

        _stateMachine.Enabled = false;
        _navMeshAgent.isStopped = true;

        _animator.SetTrigger(GetHitFrontAnim);
        _animator.SetBool(StunnedLoopAnim, true);

        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(timeStunned);

        _animator.SetBool(StunnedLoopAnim, false);
        _stateMachine.Enabled = true;
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

        Collider[] colliders = new Collider[5];
        var size = Physics.OverlapSphereNonAlloc(attackPoint.position, attackRadius, colliders, playerLayer);
        foreach (var coll in colliders)
        {
            if (!coll)
            {
                continue;
            }

            var damageable = coll.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }
    }
}