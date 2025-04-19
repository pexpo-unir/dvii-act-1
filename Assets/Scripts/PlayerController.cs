using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _inputDirection = Vector3.zero;
    private Vector3 _movementDirection = Vector3.zero;

    [SerializeField] private PlayerCharacter playerCharacter;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private CinemachineCamera aimingCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        _inputActions.Player.Look.performed += LookInput;

        _inputActions.Player.Move.performed += MoveInput;
        _inputActions.Player.Move.canceled += CanceledMoveInput;

        _inputActions.Player.Aim.started += StartedAimInput;
        _inputActions.Player.Aim.canceled += CanceledAimInput;

        _inputActions.Player.Attack.performed += AttackInput;
    }

    private void OnDisable()
    {
        _inputActions.Player.Attack.performed -= AttackInput;

        _inputActions.Player.Aim.canceled -= CanceledAimInput;
        _inputActions.Player.Aim.started -= StartedAimInput;

        _inputActions.Player.Move.canceled -= CanceledMoveInput;
        _inputActions.Player.Move.performed -= MoveInput;

        _inputActions.Player.Look.performed -= LookInput;

        _inputActions.Player.Disable();

        playerCharacter.OnTakeDamage -= PlayerTakeDamage;
    }

    private void Start()
    {
        cinemachineCamera.Priority = 100;
        aimingCamera.Priority = 0;

        playerCharacter.OnTakeDamage += PlayerTakeDamage;
    }

    private static void PlayerTakeDamage(PlayerCharacter pCharacter, float healthPercent)
    {
        GameManager.Instance.UpdateChromaticAberration(healthPercent);
    }

    private void LookInput(InputAction.CallbackContext obj)
    {
        RecalculateMovement();
    }

    private void MoveInput(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
        RecalculateMovement();
    }

    private void RecalculateMovement()
    {
        if (_inputDirection.sqrMagnitude < float.Epsilon)
        {
            playerCharacter.InputDirection = Vector3.zero;
            return;
        }

        var forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        var right = cameraTransform.right;
        right.y = 0;
        right.Normalize();

        _movementDirection = forward * _inputDirection.y + right * _inputDirection.x;

        playerCharacter.InputDirection = _movementDirection;
    }

    private void CanceledMoveInput(InputAction.CallbackContext context)
    {
        _inputDirection = Vector3.zero;
        playerCharacter.InputDirection = Vector3.zero;
    }

    private void StartedAimInput(InputAction.CallbackContext context)
    {
        cinemachineCamera.Priority = 0;
        aimingCamera.Priority = 100;

        playerCharacter.StartAim();
    }

    private void CanceledAimInput(InputAction.CallbackContext context)
    {
        cinemachineCamera.Priority = 100;
        aimingCamera.Priority = 0;

        playerCharacter.EndAim();
    }

    private void AttackInput(InputAction.CallbackContext context)
    {
        playerCharacter.Shoot();
    }
}